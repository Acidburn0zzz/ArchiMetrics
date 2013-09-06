// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TooHighCyclomaticComplexityRule.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TooHighCyclomaticComplexityRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using ArchiMetrics.Analysis.Metrics;
	using ArchiMetrics.Common.CodeReview;
	using ArchiMetrics.Common.Metrics;
	using Roslyn.Compilers.CSharp;

	internal class TooHighCyclomaticComplexityRule : CodeEvaluationBase
	{
		private const int Limit = 10;
		readonly CyclomaticComplexityCounter _counter = new CyclomaticComplexityCounter();

		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.MethodDeclaration; }
		}
		public override string Title
		{
			get
			{
				return "Method Too Complex.";
			}
		}
		public override string Suggestion
		{
			get
			{
				return "Refactor to reduce number of code paths through method.";
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var methodDeclaration = (MethodDeclarationSyntax)node;
			var complexity = _counter.Calculate(new MemberNode(string.Empty, string.Empty, MemberKind.Method, 0, methodDeclaration, null));
			if (complexity >= Limit)
			{
				return new EvaluationResult
						   {
							   ErrorCount = 1,
							   ImpactLevel = ImpactLevel.Member,
							   Quality = CodeQuality.NeedsRefactoring,
							   QualityAttribute = QualityAttribute.Testability | QualityAttribute.Maintainability | QualityAttribute.Modifiability,
							   Snippet = node.ToFullString()
						   };
			}

			return null;
		}
	}
}