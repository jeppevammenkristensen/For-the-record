using System.Threading;
using System.Threading.Tasks;
using ForTheRecord.Refactorings;
using ForTheRecord.Refactorings.ClassToRecord;
using ForTheRecord.Refactorings.RecordToClass;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;
using Xunit.Abstractions;

namespace ForTheRecord.UnitTests
{
    public class TestRefactoring
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public TestRefactoring(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Convert_Record_To_Class()
        {
            var result = TestHelpers.TryGetRefactoringContext(@"namespace Sudra 
{
    public record Su[|m|]ra (int First, int Second) {}

    public class Another 
    {
        public string Property
        {
            get;
            set;
        }
    }    
}");
            var root = await result.Document.GetSyntaxRootAsync();
            if (root.FindNode(result.Span) is not RecordDeclarationSyntax record) return;


            RecordToClassRefactoring refactoring = new RecordToClassRefactoring(record, result);
            var convertToClass = await refactoring.ConvertToClass(result.Document, record, CancellationToken.None);
            var convertedResult = await convertToClass.GetSyntaxRootAsync();
            _testOutputHelper.WriteLine(convertedResult.ToFullString());

        }

        [Fact]
        public async Task Convert_Class_To_Record()
        {
            var result = TestHelpers.TryGetRefactoringContext(@"namespace Sudra 
{
    public record Sumra (int First, int Second) {}

    public class An[|othe|]r 
    {
        public string Property
        {
            get;
            set;
        }

        public Tuca.List<Int> OtherProperty 
        {
            get;set;
        }
    }    
}");
            var root = await result.Document.GetSyntaxRootAsync();
            if (root.FindNode(result.Span) is not ClassDeclarationSyntax classDeclaration) return;


            var refactoring = new ClassToRecordRefactoring(classDeclaration, result);
            var record = await refactoring.ConvertToRecord(result.Document, classDeclaration, CancellationToken.None);
            var convertedResult = await record.GetSyntaxRootAsync();
            _testOutputHelper.WriteLine(convertedResult.ToFullString());

        }
    }
}