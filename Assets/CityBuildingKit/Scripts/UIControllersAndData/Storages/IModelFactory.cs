/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using System;

namespace Assets.Scripts.UIControllersAndData.Storages
 {
 	/// <summary>
 	///     The base interface for all model factories.
 	/// </summary>
 	public interface IModelFactory<TData>
 	{
		 /// <summary>
		 ///     A storage for the factory.
		 /// </summary>		 
		 IPersistentStorage Storage{get;}

		 /// <summary>
		 ///     Saves the given object.
		 /// </summary>
		 /// <param name="data">The object that will be saved.</param>
		 /// <param name="callback">
		 ///     An action that should be called when operation if finished. If the operatin is succed, argument
		 ///     will be true.
		 /// </param>
		 void Put(TData data, Action<bool> callback = null);
	 }
 }