using CustomCompilerMessages.Definitions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomCompilerMessages.Analyzers
{
    internal class WarningAnalyzer
    {
        public const string DiagnosticId = "CustomCompilerMessages";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Custom compiletime message";

        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public static void AnalyzeMethodInvocation(SyntaxNodeAnalysisContext context)
        {
            /*
             * The code in this method was inspired by the answer of user johnny 5 on stackoverflow.com
             * See: https://stackoverflow.com/a/45419471
             */
            var invocation = context.Node as InvocationExpressionSyntax;
            if (invocation != null)
            {
                var declarationOfInvokedMethod = context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol as IMethodSymbol;
                if (declarationOfInvokedMethod != null)
                {
                    var diagnostic = GetDiagnosticForSymbol(declarationOfInvokedMethod, invocation);
                    if (diagnostic != null)
                    {
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }

        public static void AnalyzeInvocation(OperationAnalysisContext context)
        {
            var method = (context.Operation as IInvocationOperation)?.TargetMethod;
            if (method != null)
            {
                var diagnostic = GetDiagnosticForSymbol(method, context.Operation.Syntax);
                if (diagnostic != null)
                {
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        public static void AnalyzePropertyReference(OperationAnalysisContext context)
        {
            var accessedProperty = (context.Operation as IPropertyReferenceOperation)?.Property;
            if (accessedProperty != null)
            {
                var diagnostic = GetDiagnosticForSymbol(accessedProperty, context.Operation.Syntax);
                if (diagnostic != null)
                {
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        public static void AnalyzeFieldReference(OperationAnalysisContext context)
        {
            var accessedField = (context.Operation as IFieldReferenceOperation)?.Field;
            if (accessedField != null)
            {
                var diagnostic = GetDiagnosticForSymbol(accessedField, context.Operation.Syntax);
                if (diagnostic != null)
                {
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private static Diagnostic GetDiagnosticForSymbol(ISymbol symbol, SyntaxNode syntaxNode)
        {
            var attributes = symbol.GetAttributes();
            var typedAttribute = attributes.GetAttributeData<WarningAttribute>();
            if (typedAttribute != null)
            {
                var message = typedAttribute.ConstructorArguments.FirstOrDefault().Value as string;
                return Diagnostic.Create(Rule, syntaxNode.GetLocation(), message);
            }

            return null;
        }
    }
}
