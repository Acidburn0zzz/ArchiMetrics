﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelNode.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IModelNode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Structure
{
	using System;
	using System.Collections.Generic;
	using ArchiMetrics.Common.CodeReview;

	public interface IModelNode : IEquatable<IModelNode>
	{
		IModelNode Parent { get; }

		string DisplayName { get; }

		string QualifiedName { get; }

		string Type { get; }

		CodeQuality Quality { get; }

		int LinesOfCode { get; }

		double MaintainabilityIndex { get; }

		int CyclomaticComplexity { get; }

		IEnumerable<IModelNode> Children { get; }

		void SetParent(IModelNode parent);

		IEnumerable<IModelNode> Flatten();

		void AddChild(IModelNode child);

		void RemoveChild(IModelNode child);
	}
}