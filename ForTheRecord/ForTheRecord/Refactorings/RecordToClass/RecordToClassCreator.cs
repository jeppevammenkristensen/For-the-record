using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ForTheRecord.Refactorings.RecordToClass
{
    public class RecordToClassCreator
    {
        public ClassDeclarationSyntax GenerateClassDeclaration(RecordDeclarationSyntax record, bool initFromConstructor)
        {
            var leadingClassTrivia = record.GetLeadingTrivia();
            var trailingClassTrivia = record.GetTrailingTrivia();
            var classSyntax = ClassDeclaration(record.Identifier).WithModifiers(record.Modifiers);
            var parameters = record.ParameterList?.Parameters ?? new SeparatedSyntaxList<ParameterSyntax>();
            
            foreach (ParameterSyntax parameter in parameters)
            {
                if (parameter.Type != null)
                {
                    classSyntax = classSyntax.AddMembers(
                        PropertyDeclaration(parameter.Type, parameter.Identifier)
                            .AddAccessorListAccessors(
                                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                                AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)))
                            .AddModifiers(Token(SyntaxKind.PublicKeyword))

                    );
                }
            }

            if (initFromConstructor)
            {
                var constructorDeclaration = ConstructorDeclaration(record.Identifier).AddModifiers(Token(SyntaxKind.PublicKeyword));
                constructorDeclaration = constructorDeclaration.AddParameterListParameters(GenerateConstructorParameters(parameters).ToArray());
                var constructorDeclarationSyntax = constructorDeclaration.WithBody(Block(GenerateAssignmentStatements(parameters)));
                classSyntax = classSyntax.AddMembers(constructorDeclarationSyntax);
            }

            var result = (ClassDeclarationSyntax)classSyntax.NormalizeWhitespacesSingleLineProperties().WithLeadingTrivia(leadingClassTrivia).WithTrailingTrivia(trailingClassTrivia);
            return result!;
        }

        private IEnumerable<StatementSyntax> GenerateAssignmentStatements(SeparatedSyntaxList<ParameterSyntax> parameters)
        {
            foreach (var parameterSyntax in parameters)
            {
                var lowercase = IdentifierName(FirstLetterToLower(parameterSyntax.Identifier.ToString()));
                yield return ExpressionStatement(
                    AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            ThisExpression(),
                            IdentifierName(parameterSyntax.Identifier.ToString())),
                        lowercase));
            }
        }

        private IEnumerable<ParameterSyntax> GenerateConstructorParameters(SeparatedSyntaxList<ParameterSyntax> parameters)
        {
            foreach (var parameterSyntax in parameters)
            {
                var identifier = Identifier(FirstLetterToLower(parameterSyntax.Identifier.ToString()));
                yield return Parameter(identifier).WithType(parameterSyntax.Type);
            }
        }

        private string FirstLetterToLower(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            
            return$"{char.ToLower(value[0])}{new string(value.Skip(1).ToArray())}";
        }
    }
}