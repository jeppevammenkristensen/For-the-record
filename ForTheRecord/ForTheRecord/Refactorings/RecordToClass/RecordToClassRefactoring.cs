using System.Threading;
using System.Threading.Tasks;
using ForTheRecord.Refactorings.ClassToRecord;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace ForTheRecord.Refactorings.RecordToClass
{
    public class RecordToClassRefactoring : IRegisterRefactoring
    {
        public RecordDeclarationSyntax Record { get; }
        public CodeRefactoringContext Context { get; } 

        public RecordToClassRefactoring(RecordDeclarationSyntax record, CodeRefactoringContext context)
        {
            Record = record;
            Context = context;
        }
        
        public void RegisterRefactoring()
        {
            Context.RegisterRefactoring(CodeAction.Create("Convert to class",
                c => ConvertToClass(Context.Document, Record, c)));
        }

        public async Task<Document> ConvertToClass(Document document, RecordDeclarationSyntax record,
            CancellationToken cancellationToken)
        {
            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var creator = new RecordToClassCreator();
           
            var newRoot = oldRoot!.ReplaceNode(record, creator.GenerateClassDeclaration(record).WithAdditionalAnnotations(Formatter.Annotation));

            return document.WithSyntaxRoot(newRoot);
        }

    }
}