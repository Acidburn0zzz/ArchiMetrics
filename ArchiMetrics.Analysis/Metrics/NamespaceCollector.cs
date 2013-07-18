// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamespaceCollector.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NamespaceCollector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Analysis.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;

	internal sealed class NamespaceCollector : SyntaxWalker
	{
		private readonly IList<NamespaceDeclarationSyntax> _namespaces;

		public NamespaceCollector()
			: base(SyntaxWalkerDepth.Node)
		{
			_namespaces = new List<NamespaceDeclarationSyntax>();
		}

		public IEnumerable<T> GetNamespaces<T>(CommonSyntaxNode commonNode) where T : CommonSyntaxNode
		{
			var node = commonNode as SyntaxNode;
			if (node != null)
			{
				Visit(node);
			}

			return _namespaces.Cast<T>().ToList<T>().AsReadOnly();
		}

		public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
		{
			base.VisitNamespaceDeclaration(node);
			_namespaces.Add(node);
		}
	}
}