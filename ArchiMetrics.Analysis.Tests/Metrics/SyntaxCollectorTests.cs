namespace ArchiMetrics.Analysis.Tests.Metrics
{
	using System.Linq;
	using Analysis.Metrics;
	using NUnit.Framework;
	using Roslyn.Compilers.CSharp;

	public class SyntaxCollectorTests
	{
		private SyntaxCollectorTests() { }

		public class GivenASyntaxCollector
		{
			private SyntaxCollector _collector;

			[SetUp]
			public void Setup()
			{
				_collector = new SyntaxCollector();
			}

			[Test]
			public void WhenSnippetRootIsNamespaceThenOnlyFindsNamespace()
			{
				const string Snippet = @"namespace SomeNamespace
{
	public class Foo
	{
		public string Text { get; set; }
	}
}";
				var tree = SyntaxTree.ParseText(Snippet);
				var result = _collector.GetDeclarations(new[] { tree });

				Assert.IsNotEmpty(result.NamespaceDeclarations);
				Assert.IsEmpty(result.MemberDeclarations.Cast<object>().Concat(result.Statements).Concat(result.TypeDeclarations));
			}

			[Test]
			public void WhenSnippetRootIsClassThenOnlyFindsType()
			{
				const string Snippet = @"public class Foo
{
	public string Text { get; set; }
}";
				var tree = SyntaxTree.ParseText(Snippet);
				var result = _collector.GetDeclarations(new[] { tree });

				Assert.IsNotEmpty(result.TypeDeclarations);
				Assert.IsEmpty(result.MemberDeclarations.Cast<object>().Concat(result.Statements).Concat(result.NamespaceDeclarations));
			}

			[Test]
			public void WhenSnippetRootIsPropertyThenOnlyFindsMember()
			{
				const string Snippet = @"public string Text { get; set; }";
				var tree = SyntaxTree.ParseText(Snippet);
				var result = _collector.GetDeclarations(new[] { tree });

				Assert.IsNotEmpty(result.MemberDeclarations);
				Assert.IsEmpty(result.TypeDeclarations.Cast<object>().Concat(result.Statements).Concat(result.NamespaceDeclarations));
			}

			[Test]
			public void WhenSnippetRootIsStatementThenOnlyFindsStatement()
			{
				const string Snippet = @"var x = 1;
var y = 2;";
				var tree = SyntaxTree.ParseText(Snippet);
				var result = _collector.GetDeclarations(new[] { tree });

				Assert.IsNotEmpty(result.Statements);
				Assert.IsEmpty(result.TypeDeclarations.Cast<object>().Concat(result.MemberDeclarations).Concat(result.NamespaceDeclarations));
			}
		}
	}
}