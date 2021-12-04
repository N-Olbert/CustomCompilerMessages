using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = CustomCompilerMessages.Test.CSharpAnalyzerVerifier<CustomCompilerMessages.CustomCompilerMessagesAnalyzer>;

[assembly:Parallelize(Scope = ExecutionScope.MethodLevel)]
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
        public async Task InvocationOfMethodWithAttribute_TriggersViolation()
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
            Foo();
            this.Foo();
            var x = Foo();
            x = this.Foo();
            Foo().GetHashCode();
            this.Foo().GetHashCode();
            var y = Foo().GetHashCode();
            y = this.Foo().GetHashCode();
        }
    }";

            var expected = new DiagnosticResult[] {
                VerifyCS.Diagnostic().WithSpan(12, 13, 12, 18).WithArguments("Custom warning"),
                VerifyCS.Diagnostic().WithSpan(13, 13, 13, 23).WithArguments("Custom warning"),
                VerifyCS.Diagnostic().WithSpan(14, 21, 14, 26).WithArguments("Custom warning"),
                VerifyCS.Diagnostic().WithSpan(15, 17, 15, 27).WithArguments("Custom warning"),
                VerifyCS.Diagnostic().WithSpan(16, 13, 16, 18).WithArguments("Custom warning"),
                VerifyCS.Diagnostic().WithSpan(17, 13, 17, 23).WithArguments("Custom warning"),
                VerifyCS.Diagnostic().WithSpan(18, 21, 18, 26).WithArguments("Custom warning"),
                VerifyCS.Diagnostic().WithSpan(19, 17, 19, 27).WithArguments("Custom warning"),
            };

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task ReferenceOfPropertyWithAttribute_TriggersViolation()
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

            var expected = VerifyCS.Diagnostic().WithLocation(12, 21).WithMessage("Custom warning");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task ReferenceOfFieldWithAttribute_TriggersViolation()
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

            var expected = VerifyCS.Diagnostic().WithLocation(12, 21).WithMessage("Custom warning");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task InvocationOfCtorWithAttribute_TriggersViolation()
        {
            var test = @"
    using System;
    using CustomCompilerMessages.Definitions;

    public class TestClass
    {
        [Warning(""Custom warning"")]
        public TestClass() { }

        public static void Bar()
        {
            var x = new TestClass();
        }
    }";

            var expected = VerifyCS.Diagnostic().WithLocation(12, 21).WithMessage("Custom warning");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task ReferenceOfParameterWithAttribute_TriggersViolation()
        {
            var test = @"
    using System;
    using CustomCompilerMessages.Definitions;

    public class TestClass
    {
        public static void Bar([Warning(""Custom warning"")] int i)
        {
            var x = i;
        }
    }";

            var expected = VerifyCS.Diagnostic().WithLocation(9, 21).WithMessage("Custom warning");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task AnyUsageOfAClassWithAttribute_TriggersViolation()
        {
            var test = @"
    using System;
    using CustomCompilerMessages.Definitions;

    [Warning(""Custom warning"")]
    public class TestClass
    {
        private int a;
        public int B { get; set; }
        public object Foo() => null;

        public void Bar()
        {
            var x = a;
            x = B;
            x = Foo().GetHashCode();
            new TestClass();
        }
    }";

            var expected = new DiagnosticResult[]
                {
                    VerifyCS.Diagnostic().WithSpan(14, 21, 14, 22).WithArguments("Custom warning"),
                    VerifyCS.Diagnostic().WithSpan(15, 17, 15, 18).WithArguments("Custom warning"),
                    VerifyCS.Diagnostic().WithSpan(16, 17, 16, 22).WithArguments("Custom warning"),
                    VerifyCS.Diagnostic().WithSpan(17, 13, 17, 28).WithArguments("Custom warning"),
                };
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task AnyUsageOfAStructWithAttribute_TriggersViolation()
        {
            var test = @"
    using System;
    using CustomCompilerMessages.Definitions;

    [Warning(""Custom warning"")]
    public struct TestClass
    {
        private int a;
        public int B { get; set; }
        public object Foo() => null;

        public void Bar()
        {
            var x = a;
            x = B;
            x = Foo().GetHashCode();
            new TestClass();
        }
    }";

            var expected = new DiagnosticResult[]
                {
                    VerifyCS.Diagnostic().WithSpan(14, 21, 14, 22).WithArguments("Custom warning"),
                    VerifyCS.Diagnostic().WithSpan(15, 17, 15, 18).WithArguments("Custom warning"),
                    VerifyCS.Diagnostic().WithSpan(16, 17, 16, 22).WithArguments("Custom warning"),
                    VerifyCS.Diagnostic().WithSpan(17, 13, 17, 28).WithArguments("Custom warning"),
                };
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task AnyUsageOfAnInterfaceMemberWithAttribute_TriggersViolation()
        {
            var test = @"
    using System;
    using CustomCompilerMessages.Definitions;

    [Warning(""Custom warning"")]
    public interface ITestClass
    {
        int B { get; set; }
        object Foo();
    }

    public class TestClass
    {
        public void Bar(ITestClass x)
        {
            var _ = x.B;
            x.Foo();
        }
    }";

            var expected = new DiagnosticResult[]
                {
                    VerifyCS.Diagnostic().WithSpan(16, 21, 16, 24).WithArguments("Custom warning"),
                    VerifyCS.Diagnostic().WithSpan(17, 13, 17, 20).WithArguments("Custom warning"),
                };
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
