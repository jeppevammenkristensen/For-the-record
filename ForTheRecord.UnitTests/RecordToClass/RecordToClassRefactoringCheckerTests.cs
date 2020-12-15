using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ForTheRecord.Refactorings.RecordToClass;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Xunit;

namespace ForTheRecord.UnitTests.RecordToClass
{
    public class RecordToClassRefactoringCheckerTests
    {
        [Fact]
        public async Task GetRefactoring_NotARecord_ReturnsEmpty()
        {
            var harness = new TestHarness(@"public class So[|mething|]
{

}");
            var result = await harness.Subject.GetRefactoring(harness.Context).ToListAsync();
            result.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetRefactoring_RecordNotMarked_ReturnsEmpty()
        {
            var harness = new TestHarness(@"public record Record()
{

}

public cla[|s|]s AnotherClass
{

}");
            var result = await harness.Subject.GetRefactoring(harness.Context).ToListAsync();
            result.Should().HaveCount(0);

        }


        [Fact]
        public async Task GetRefactoring_RecordMarkedNoParameters_ReturnsExpected()
        {
            var harness = new TestHarness(@"public record [|Re|]cord()
{

}

public class AnotherClass
{

}");
            var result = await harness.Subject.GetRefactoring(harness.Context).ToListAsync();
            result.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetRefactoring_RecordMarkedHasMember_ReturnsEmpty()
        {
            var harness = new TestHarness(@"public record [|Re|]cord(int First)
{
    public method DoSomething()
    {

    }
}");
            var result = await harness.Subject.GetRefactoring(harness.Context).ToListAsync();
            result.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetRefactoring_RecordMarkedHasMember_ReturnsMatch()
        {
            var harness = new TestHarness(@"public record [|Re|]cord(int First)
{    
    
}");
            var result = await harness.Subject.GetRefactoring(harness.Context).ToListAsync();
            result.Should().HaveCount(1);
            result.ElementAt(0).Should().BeOfType<RecordToClassRefactoring>();
        }



        public class TestHarness : TestHarnessBase<RecordToClassRefactoringChecker>
        {
            public TestHarness(string code)
            {
                Subject = new RecordToClassRefactoringChecker();
                Context = TestHelpers.TryGetRefactoringContext(code);
            }

            public CodeRefactoringContext Context { get;  }
        }
    }
}