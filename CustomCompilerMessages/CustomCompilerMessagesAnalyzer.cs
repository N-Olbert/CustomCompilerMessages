using CustomCompilerMessages.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace CustomCompilerMessages
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CustomCompilerMessagesAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(WarningAnalyzer.Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterOperationAction(WarningAnalyzer.AnalyzeInvocation, OperationKind.Invocation);
            context.RegisterOperationAction(WarningAnalyzer.AnalyzeObjectCreation, OperationKind.ObjectCreation);
            context.RegisterOperationAction(WarningAnalyzer.AnalyzeParameterReference, OperationKind.ParameterReference);
            context.RegisterOperationAction(WarningAnalyzer.AnalyzePropertyReference, OperationKind.PropertyReference);
            context.RegisterOperationAction(WarningAnalyzer.AnalyzeFieldReference, OperationKind.FieldReference);
        }
    }
}
