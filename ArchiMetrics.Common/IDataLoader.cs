// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataLoader.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IDataLoader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Common
{
	using System;
	using System.Threading.Tasks;

	public interface IDataLoader : IDisposable
	{
		Task Load(ProjectSettings settings);
	}
}