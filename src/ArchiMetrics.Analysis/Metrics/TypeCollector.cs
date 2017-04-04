// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeCollector.cs" company="Reimers.dk">
//   Copyright � Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeCollector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using System.Collections.Generic;
	using Common;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal sealed class TypeCollector
	{
		public IEnumerable<TypeDeclarationSyntax> GetTypes(SyntaxNode namespaceNode)
		{
			var innerCollector = new InnerTypeCollector();
			return innerCollector.GetTypes(namespaceNode);
		}

		private class InnerTypeCollector : CSharpSyntaxWalker
		{
			private readonly IList<TypeDeclarationSyntax> _types;

			public InnerTypeCollector()
				: base(SyntaxWalkerDepth.Node)
			{
				_types = new List<TypeDeclarationSyntax>();
			}

			public IEnumerable<TypeDeclarationSyntax> GetTypes(SyntaxNode namespaceNode)
			{
				var node = namespaceNode as NamespaceDeclarationSyntax;
				if (node != null)
				{
					Visit(node);
				}

				return _types.AsArray();
			}

			public override void VisitClassDeclaration(ClassDeclarationSyntax node)
			{
				base.VisitClassDeclaration(node);
				_types.Add(node);
			}

			public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
			{
				base.VisitInterfaceDeclaration(node);
				_types.Add(node);
			}

			public override void VisitStructDeclaration(StructDeclarationSyntax node)
			{
				base.VisitStructDeclaration(node);
				_types.Add(node);
			}
		}
	}
}
