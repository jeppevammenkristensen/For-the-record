using System.Threading;
using System.Threading.Tasks;
using ForTheRecord.Refactorings.RecordToClass;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ForTheRecord.Refactorings.ClassToRecord
{
    public class ClassToRecordRefactoring : IRegisterRefactoring
    {
        public ClassDeclarationSyntax ClassDeclaration { get; }
        public CodeRefactoringContext Context { get; }

        public ClassToRecordRefactoring(ClassDeclarationSyntax classDeclaration, CodeRefactoringContext context)
        {
            ClassDeclaration = classDeclaration;
            Context = context;
        }
        
        
        public void RegisterRefactoring()
        {
            Context.RegisterRefactoring(CodeAction.Create("Convert to record (properties initialized in constructor)",
                c => ConvertToRecord(Context.Document, ClassDeclaration, c)));
        }

        public async Task<Document> ConvertToRecord(Document document, ClassDeclarationSyntax record,
            CancellationToken cancellationToken)
        {
            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var creator = new ClassToRecordCreator();
            var newRoot = oldRoot!.ReplaceNode(record, creator.GenerateRecordDeclaration(record)).NormalizeWhitespacesSingleLineProperties();

            return document.WithSyntaxRoot(newRoot);
        }
    }
}