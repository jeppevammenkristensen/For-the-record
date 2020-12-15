using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ForTheRecord.Refactorings.RecordToClass
{
    public class RecordToClassCreator
    {
        public ClassDeclarationSyntax GenerateClassDeclaration(RecordDeclarationSyntax record)
        {
            var leadingClassTrivia = record.GetLeadingTrivia();
            var trailingClassTrivia = record.GetTrailingTrivia();
            var classSyntax = ClassDeclaration(record.Identifier).WithModifiers(record.Modifiers);

            foreach (ParameterSyntax parameter in record.ParameterList?.Parameters ?? new SeparatedSyntaxList<ParameterSyntax>())
            {
                if (parameter.Type != null)
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

            var result = (ClassDeclarationSyntax)classSyntax.NormalizeWhitespacesSingleLineProperties().WithLeadingTrivia(leadingClassTrivia).WithTrailingTrivia(trailingClassTrivia);
            return result!;
        }

        
    }
}