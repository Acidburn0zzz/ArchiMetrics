// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamespaceDeclarationSyntaxInfo.cs" company="Reimers.dk">
//   Copyright � Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NamespaceDeclarationSyntaxInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using Microsoft.CodeAnalysis;

	public sealed class NamespaceDeclarationSyntaxInfo
	{
		public string CodeFile { get; set; }

		public string Name { get; set; }

		public SyntaxNode Syntax { get; set; }
	}
}
