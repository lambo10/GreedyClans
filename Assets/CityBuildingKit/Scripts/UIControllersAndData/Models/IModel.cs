/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using Assets.Scripts.UIControllersAndData.Storages;

namespace Assets.Scripts.UIControllersAndData.Models
{
	/// <summary>
	///     A base interface for models
	/// </summary>
	public interface IModel<TData>
	{
		/// <summary>
		///     A factory used to create the model
		/// </summary>
		IModelFactory<TData> ModelFactory{set;}
	}
}