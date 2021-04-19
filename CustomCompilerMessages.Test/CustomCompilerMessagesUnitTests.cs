using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = CustomCompilerMessages.Test.CSharpAnalyzerVerifier<CustomCompilerMessages.CustomCompilerMessagesAnalyzer>;

namespace CustomCompilerMessages.Test
{
    [TestClass]
    public class CustomCompilerMessagesUnitTest
    {
        [TestMethod]
        public async Task EmptyFile_DoesNotTriggerAnalyzer()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task ValidClass_DoesNotTriggerAnalyzer()
        {
            var test = @"
    using System;
    using CustomCompilerMessages.Definitions;

    public class TestClass
    {
        public object Foo() => null;

        public void Bar()
        {
            var x = Foo();
        }
    }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task InvocationOfMethodWithAttribute_TriggersViolation_InvocationVariant1()
        {
            var test = @"
    using System;
    using CustomCompilerMessages.Definitions;

    public class TestClass
    {
        [CustomCompilerMessages.Definitions.Warning(""Custom warning"")]
        public object Foo() => null;

        public void Bar()
        {
            var x = Foo();
        }
    }";

            var expected = VerifyCS.Diagnostic("CustomCompilerMessages")
                .WithLocation(12, 21)
                .WithMessage("Custom warning");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task InvocationOfMethodWithAttribute_TriggersViolation_InvocationVariant2()
        {
            var test = @"
    using System;
    using CustomCompilerMessages.Definitions;

    public class TestClass
    {
        [CustomCompilerMessages.Definitions.Warning(""Custom warning"")]
        public object Foo() => new object();

        public void Bar()
        {
            var x = Foo().GetHashCode();
        }
    }";

            var expected = VerifyCS.Diagnostic("CustomCompilerMessages")
                .WithLocation(12, 21)
                .WithMessage("Custom warning");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task RefernceOfPropertyWithAttribute_TriggersViolation()
        {
            var test = @"
    using System;
    using CustomCompilerMessages.Definitions;

    public class TestClass
    {
        [Warning(""Custom warning"")]
        public object Foo => new object();

        public void Bar()
        {
            var x = Foo.GetHashCode();
        }
    }";

            var expected = VerifyCS.Diagnostic("CustomCompilerMessages")
                .WithLocation(12, 21)
                .WithMessage("Custom warning");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task RefernceOfFieldWithAttribute_TriggersViolation()
        {
            var test = @"
    using System;
    using CustomCompilerMessages.Definitions;

    public class TestClass
    {
        [Warning(""Custom warning"")]
        private object foo = new object();

        public void Bar()
        {
            var x = this.foo.GetHashCode();
        }
    }";

            var expected = VerifyCS.Diagnostic("CustomCompilerMessages")
                .WithLocation(12, 21)
                .WithMessage("Custom warning");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
