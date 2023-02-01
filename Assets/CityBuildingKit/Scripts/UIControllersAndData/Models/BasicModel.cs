/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using System.Xml.Serialization;
using Assets.Scripts.UIControllersAndData.Storages;

namespace Assets.Scripts.UIControllersAndData.Models
 {
 	/// <inheritdoc />
	 /// <summary>
	 ///     Basic implementation of the simplest model.
	 /// </summary>
 	public class BasicModel<TData> : IModel<TData>
 	{ 
 		/// <inheritdoc />
 		[XmlIgnore]
 		public IModelFactory<TData> ModelFactory{get; set;}
 	}
 }