﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnreadFieldRuleTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the UnreadFieldRuleTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Tests.Rules.Semantic
{
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.CodeReview.Rules.Semantic;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Xunit;

	public sealed class UnreadFieldRuleTests
	{
		private UnreadFieldRuleTests()
		{
		}

		public class GivenAnUnreadFieldRule : SolutionTestsBase
		{
			private readonly UnreadFieldRule _rule;

			public GivenAnUnreadFieldRule()
			{
				_rule = new UnreadFieldRule();
			}

            [Theory]
			[InlineData(@"namespace MyNamespace
{
	public class MyClass
	{
		private object _field = new object();
	}
}")]
			[InlineData(@"namespace MyNamespace
{
	public class MyClass
	{
		private object _field;

		public MyClass()
		{
			_field = 

new object();
		}
	}
}")]
			[InlineData(@"namespace MyNamespace
{
	public class MyClass
	{
		private object _field;

		public void Something()
		{
			_field = new object();
		}
	}
}")]
			public async Task WhenFieldIsNeverReadThenReturnsError(string code)
			{
				var solution = CreateSolution(code);
				var classDeclaration = (from p in solution.Projects
										from d in p.Documents
										let model = d.GetSemanticModelAsync().Result
										let root = d.GetSyntaxRootAsync().Result
										from n in root.DescendantNodes().OfType<FieldDeclarationSyntax>()
										select new
										{
											semanticModel = model,
											node = n
										}).First();
				var result = await _rule.Evaluate(classDeclaration.node, classDeclaration.semanticModel, solution);

				Assert.NotNull(result);
			}

            [Theory]
			[InlineData(@"namespace MyNamespace
{
	public class MyClass
	{
		private object _field = new object();

		public void Write()
		{
			if(_field == null)
			{
				System.Console.WriteLine(""null"");
			}
		}
	}
}")]
			[InlineData(@"namespace MyNamespace
{
	public class MyClass
	{
		private object _field = new object();

		public object Get()
		{
			return _field;
		}
	}
}")]
			[InlineData(@"namespace MyNamespace
{
	public class MyClass
	{
		private object _field = new object();

		public object Get()
		{
			var obj = _field;
			return obj;
		}
	}
}")]
			public async Task WhenFieldIsReadThenDoesNotReturnError(string code)
			{
				var solution = CreateSolution(code);
				var classDeclaration = (from p in solution.Projects
										from d in p.Documents
										let model = d.GetSemanticModelAsync().Result
										let root = d.GetSyntaxRootAsync().Result
										from n in root.DescendantNodes().OfType<FieldDeclarationSyntax>()
										select new
										{
											semanticModel = model,
											node = n
										}).First();
				var result = await _rule.Evaluate(classDeclaration.node, classDeclaration.semanticModel, solution);

				Assert.Null(result);
			}
		}
	}
}