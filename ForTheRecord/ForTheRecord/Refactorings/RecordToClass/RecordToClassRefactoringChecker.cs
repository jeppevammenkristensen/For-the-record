using System.Collections.Generic;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ForTheRecord.Refactorings.RecordToClass
{
    public class RecordToClassRefactoringChecker
    {
        public async IAsyncEnumerable<IRegisterRefactoring> GetRefactoring(CodeRefactoringContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync().ConfigureAwait(false);
            var node = root?.FindNode(context.Span);

            if (node is not RecordDeclarationSyntax record)
            {
                yield break;
            }

            if (record.ParameterList == null || record.ParameterList.Parameters.Count == 0)
            {
                yield break;
            }

            if (record.Members.Count > 0)
            {
                yield break;
            }

            yield return new RecordToClassRefactoring(record, context);
        }
    }
}