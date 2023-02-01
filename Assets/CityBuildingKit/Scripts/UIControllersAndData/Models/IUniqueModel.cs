/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

namespace Assets.Scripts.UIControllersAndData.Models
{
	/// <inheritdoc />
	/// <summary>
	/// Interface for model with unique identifier.
	/// </summary>
	public interface IUniqueModel<TData> : IModel<TData>
	{
		/// <summary>
		///     Unique identifier of this object
		/// </summary>
		string UniqueIdentifier{get; set;}
	}
}