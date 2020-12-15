using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ForTheRecord.Refactorings.ClassToRecord
{
    public class ClassToRecordCreator 
    {

        public RecordDeclarationSyntax GenerateRecordDeclaration(ClassDeclarationSyntax classDeclaration)
        {
            var record = RecordDeclaration(
                Token(SyntaxKind.RecordKeyword),
                classDeclaration.Identifier).WithModifiers(classDeclaration.Modifiers);

            var parameterList = ParameterList();

            foreach (var property in classDeclaration.Members.OfType<PropertyDeclarationSyntax>())
            {
                parameterList = parameterList.AddParameters(Parameter(property.Identifier).WithType(property.Type)
                    .WithModifiers(property.Modifiers));
            }

            return record.WithParameterList(parameterList)
                .WithOpenBraceToken(
                    Token(SyntaxKind.OpenBraceToken))
                .WithCloseBraceToken(
                    Token(SyntaxKind.CloseBraceToken)).NormalizeWhitespace();
        }
    }
}