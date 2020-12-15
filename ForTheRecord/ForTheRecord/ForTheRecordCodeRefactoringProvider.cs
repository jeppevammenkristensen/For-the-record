using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using ForTheRecord.Refactorings;
using ForTheRecord.Refactorings.RecordToClass;

namespace ForTheRecord
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(ForTheRecordCodeRefactoringProvider)), Shared]
    internal class ForTheRecordCodeRefactoringProvider : CodeRefactoringProvider
    {
        public sealed override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            RecordToClassRefactoringChecker checker = new();

            await foreach (var refactoring in checker.GetRefactoring(context))
            {
                refactoring.RegisterRefactoring();
            }
        }
        
        public void Test(IEnumerable<IRegisterRefactoring> registerRefactoring)
        {
            foreach (var refactoring in registerRefactoring.Where(x => x != null))
            {
                refactoring!.RegisterRefactoring();
            }
        }
    }
    
  
}
