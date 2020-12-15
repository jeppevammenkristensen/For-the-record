using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using ForTheRecord.Refactorings.RecordToClass;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Xunit;
using Xunit.Abstractions;

namespace ForTheRecord.UnitTests.RecordToClass
{
    public class RecordToClassRefactoringTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public RecordToClassRefactoringTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task ConvertToClass_DontInitPropertiesInConstructor_MatchExpected()
        {
            var harness = await TestHarness.Init(@"namespace Custom
{
    public record T[|estRec|]ord(string FirstName, string LastName) {}

    public class ClassWithProperties 
    {
        public string First { get;set; }
    }
}");
            var result = await harness.Subject.ConvertToClass(harness.Subject.Context.Document, harness.Subject.Record, false,CancellationToken.None);

            var textAsync = await result.GetTextAsync(CancellationToken.None);
            textAsync.ToString().Should().Be(@"namespace Custom
{
    public class TestRecord
{
    public string FirstName { get; set; }

    public string LastName { get; set; }
}

    public class ClassWithProperties 
    {
        public string First { get;set; }
    }
}");
            var root = await result.GetSyntaxRootAsync();
            var testRecord = root.DescendantNodes().OfType<ClassDeclarationSyntax>().SingleOrDefault(x => x.Identifier.ToString() == "TestRecord");
            testRecord.Should().NotBeNull();
            testRecord!.ContainsAnnotations.Should().BeTrue();
            testRecord!.HasAnnotation(Formatter.Annotation).Should().BeTrue();
        }

        [Fact]
        public async Task ConvertToClass_InitPropertiesInConstructor_MatchExpected()
        {
            var harness = await TestHarness.Init(@"namespace Custom
{
    public record T[|estRec|]ord(string FirstName, string LastName) {}

    public class ClassWithProperties 
    {
        public string First { get;set; }
    }
}");
            var result = await harness.Subject.ConvertToClass(harness.Subject.Context.Document, harness.Subject.Record, true, CancellationToken.None);

            var textAsync = await result.GetTextAsync(CancellationToken.None);
            textAsync.ToString().Should().Be(@"namespace Custom
{
    public class TestRecord
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public TestRecord(string firstName, string lastName)
    {
        this.FirstName = firstName;
        this.LastName = lastName;
    }
}

    public class ClassWithProperties 
    {
        public string First { get;set; }
    }
}");
            var root = await result.GetSyntaxRootAsync();
            var testRecord = root.DescendantNodes().OfType<ClassDeclarationSyntax>().SingleOrDefault(x => x.Identifier.ToString() == "TestRecord");
            testRecord.Should().NotBeNull();
            testRecord!.ContainsAnnotations.Should().BeTrue();
            testRecord!.HasAnnotation(Formatter.Annotation).Should().BeTrue();
        }



        public class TestHarness : TestHarnessBase<RecordToClassRefactoring>
        {
            public RecordToClassRefactoring Subject { get; }
            
            private TestHarness(CodeRefactoringContext context, RecordDeclarationSyntax record)
            {
                Subject = new RecordToClassRefactoring(record, context);
            }

            public static async Task<TestHarness> Init(string code)
            {
                var context = TestHelpers.TryGetRefactoringContext(code);
                var root = await context.Document.GetSyntaxRootAsync(CancellationToken.None);
                if (root?.FindNode(context.Span) is RecordDeclarationSyntax record)
                {
                    return new TestHarness(context, record);
                }

                throw new InvalidOperationException("Should be able to find record in span");
            }

        }
    }
}