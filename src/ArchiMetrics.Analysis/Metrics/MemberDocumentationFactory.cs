// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberDocumentationFactory.cs" company="Reimers.dk">
//   Copyright � Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MemberDocumentationFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Xml;

namespace ArchiMetrics.Analysis.Metrics
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Common;
    using Common.Metrics;
    using Microsoft.CodeAnalysis;

    internal class MemberDocumentationFactory : IAsyncFactory<ISymbol, IMemberDocumentation>
    {
        private static readonly MethodKind[] ChildMethods = {MethodKind.PropertyGet, MethodKind.PropertySet, MethodKind.EventAdd, MethodKind.EventRemove};

        /// <summary>
        /// Creates the requested instance as an asynchronous operation.
        /// </summary>
        /// <param name="memberSymbol">The memberSymbol to pass to the object creation.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to use for cancelling the object creation.</param>
        /// <returns>Returns a <see cref="Task{T}"/> which represents the instance creation task.</returns>
        public Task<IMemberDocumentation> Create(ISymbol memberSymbol, CancellationToken cancellationToken)
        {
            var doc = GetDocumentationText(memberSymbol);
            if (string.IsNullOrWhiteSpace(doc))
            {
                return Task.FromResult<IMemberDocumentation>(null);
            }

            var docRoot = TryParseXmlComment(doc);
            if (docRoot == null)
            {
                return Task.FromResult<IMemberDocumentation>(new FaultMemberDocumentation(doc));
            }

            var summaryElement = docRoot.Element("summary");
            var summary = summaryElement?.Value.Trim() ?? string.Empty;
            var codeElement = docRoot.Element("code");
            var code = codeElement?.Value.Trim() ?? string.Empty;
            var exampleElement = docRoot.Element("example");
            var example = exampleElement?.Value.Trim() ?? string.Empty;
            var remarksElement = docRoot.Element("remarks");
            var remarks = remarksElement?.Value.Trim() ?? string.Empty;
            var returnsElement = docRoot.Element("returns");
            var returns = returnsElement?.Value.Trim() ?? string.Empty;
            var typeParameterElements = docRoot.Elements("typeparam");
            var parameterElements = docRoot.Elements("param")
                .Select(_ => new KeyValuePair<string, string>(_.Attribute("name").Value.Trim(), _.Value.Trim()))
                .ToDictionary(_ => _.Key, _ => _.Value);
            var typeConstraints = GetTypeContraints(memberSymbol);
            var typeParameters =
                typeParameterElements.Select(
                    x =>
                    {
                        var name = x.Attribute("name").Value.Trim();
                        return new TypeParameterDocumentation(
                            name,
                            typeConstraints.ContainsKey(name) ? typeConstraints[name] : null,
                            x.Value.Trim());
                    });
            var parameters = GetParameters(memberSymbol as IMethodSymbol, parameterElements);
            var exceptionElements = docRoot.Elements("exception");
            var exceptions =
                exceptionElements.Select(
                    x => new ExceptionDocumentation(x.Attribute("cref").Value.Trim(), x.Value.Trim()));

            var documentation = new MemberDocumentation(summary, code, example, remarks, returns, typeParameters,
                parameters, exceptions);

            return Task.FromResult<IMemberDocumentation>(documentation);
        }

        private static XElement TryParseXmlComment(string xml)
        {
            try
            {
                var xmldoc = XDocument.Parse(xml);
                return xmldoc.Root;
            }
            catch (XmlException)
            {
                return null;
            }
	    }

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
		}

		private static string GetDocumentationText(ISymbol symbol)
		{
			var methodSymbol = symbol as IMethodSymbol;
			var isChildMethod = methodSymbol != null && methodSymbol.MethodKind.In(ChildMethods);
			return isChildMethod
				? GetDocumentationText(methodSymbol.AssociatedSymbol)
				: symbol.GetDocumentationCommentXml();
		}

		private static IEnumerable<ParameterDocumentation> GetParameters(IMethodSymbol symbol, IDictionary<string, string> parameterDocumentations)
		{
			return symbol == null
					   ? Enumerable.Empty<ParameterDocumentation>()
					   : symbol.Parameters.Select(
						   x =>
						   new ParameterDocumentation(
							   x.Name,
							   x.Type.ToDisplayString(),
							   parameterDocumentations.ContainsKey(x.Name) ? parameterDocumentations[x.Name] : string.Empty)).ToArray();
		}

		private static IDictionary<string, string> GetTypeContraints(ISymbol symbol)
		{
			var method = symbol as IMethodSymbol;
			if (method == null)
			{
				return new Dictionary<string, string>();
			}

			var enumerable = method.TypeParameters.Select(CreateTypeConstraint);
			var typeParameterConstraints = enumerable.ToDictionary(_ => _.Key, _ => _.Value);

			return typeParameterConstraints;
		}

		private static KeyValuePair<string, string> CreateTypeConstraint(ITypeParameterSymbol typeParameter)
		{
			var parts = new List<string>();
			if (typeParameter.HasReferenceTypeConstraint)
			{
				parts.Add("class");
			}

			if (typeParameter.HasValueTypeConstraint)
			{
				parts.Add("struct");
			}

			if (typeParameter.HasConstructorConstraint)
			{
				parts.Add("new()");
			}

			parts.AddRange(typeParameter.ConstraintTypes.Select(constraintType => constraintType.ToDisplayString()));

			return new KeyValuePair<string, string>(typeParameter.Name, string.Join(", ", parts));
		}
	}
}