using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ForTheRecord.Refactorings.ClassToRecord
{
    public class ClassToRecordRefactoringChecker
    {
        public ClassToRecordRefactoring? GetRefactoring(SyntaxNode? node, CodeRefactoringContext context)
        {
            if (node == null) return null;
            
            if (node is not ClassDeclarationSyntax classDeclaration)
            {
                return null;
            }

            return new ClassToRecordRefactoring(classDeclaration, context);
        }
    }
}