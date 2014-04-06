namespace ArchiMetrics.Analysis.Validation
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using ArchiMetrics.Common.CodeReview;
	using ArchiMetrics.Common.Structure;

	internal class ModelValidator : IModelValidator
	{
		private readonly IVertexRepository _repository;
		private readonly ISyntaxTransformer _syntaxTransformer;

		public ModelValidator(
			ISyntaxTransformer syntaxTransformer,
			IVertexRepository repository)
		{
			_syntaxTransformer = syntaxTransformer;
			_repository = repository;
		}

		public async Task<IEnumerable<IValidationResult>> Validate(string solutionPath, IEnumerable<IModelRule> rules, IEnumerable<TransformRule> transformRules, CancellationToken cancellationToken)
		{
			var model = await _repository.GetVertices(solutionPath, cancellationToken);
			var transformed = await _syntaxTransformer.Transform(model, transformRules, cancellationToken);
			var modelTree = new ModelNode("All", NodeKind.Solution, CodeQuality.Good, 0, 0, 0, transformed.ToList());
			var tasks = rules.Select(r => r.Validate(modelTree));
			var results = await Task.WhenAll(tasks);

			return results.SelectMany(x => x).ToArray();
		}
	}
}
