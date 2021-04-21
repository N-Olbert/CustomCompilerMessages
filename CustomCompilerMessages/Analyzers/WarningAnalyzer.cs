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
        public const string DiagnosticId = "CCM01";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Custom compiletime message";

        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public static void AnalyzeInvocation(OperationAnalysisContext context) => 
            AnalyzeSymbol((context.Operation as IInvocationOperation)?.TargetMethod, context);

        public static void AnalyzePropertyReference(OperationAnalysisContext context) => 
            AnalyzeSymbol((context.Operation as IPropertyReferenceOperation)?.Property, context);

        public static void AnalyzeFieldReference(OperationAnalysisContext context) => 
            AnalyzeSymbol((context.Operation as IFieldReferenceOperation)?.Field, context);

        public static void AnalyzeParameterReference(OperationAnalysisContext context) =>
            AnalyzeSymbol((context.Operation as IParameterReferenceOperation)?.Parameter, context);

        public static void AnalyzeObjectCreation(OperationAnalysisContext context) => 
            AnalyzeSymbol((context.Operation as IObjectCreationOperation)?.Constructor, context);
        private static void AnalyzeSymbol(ISymbol symbol, OperationAnalysisContext context)
        {
            if (symbol != null)
            {
                if (AnalyzeSymbolInternal(symbol, context))
                {
                    var assembly = symbol.ContainingAssembly;
                    if (AnalyzeHierachySymbolInternal(assembly, context))
                    {
                        var classOrStruct = symbol.ContainingType;
                        if (AnalyzeHierachySymbolInternal(classOrStruct, context))
                        {
                            var interfaces = classOrStruct.AllInterfaces;
                            if (interfaces != null)
                            {
                                foreach (var @interface in classOrStruct.AllInterfaces)
                                {
                                    if (!AnalyzeHierachySymbolInternal(@interface, context))
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static bool AnalyzeHierachySymbolInternal(ISymbol symbol, OperationAnalysisContext context)
        {
            if (!string.IsNullOrEmpty(symbol?.Name) &&
                !symbol.Name.StartsWith("System") &&
                !symbol.Name.StartsWith("Microsoft"))
            {
                return AnalyzeSymbolInternal(symbol, context);
            }

            return true;
        }

        private static bool AnalyzeSymbolInternal(ISymbol symbol, OperationAnalysisContext context)
        {
            if (context.CancellationToken.IsCancellationRequested)
            {
                return false;
            }

            if (symbol != null)
            {
                var attributes = symbol.GetAttributes();
                var typedAttribute = attributes.GetAttributeData<WarningAttribute>();
                if (typedAttribute != null)
                {
                    var message = typedAttribute.ConstructorArguments.FirstOrDefault().Value as string;
                    var diagnostic = Diagnostic.Create(Rule, context.Operation?.Syntax?.GetLocation(), message);
                    context.ReportDiagnostic(diagnostic);
                    return false;
                }
            }

            return true;
        }
    }
}
