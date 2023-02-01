/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using System;
using Assets.Scripts.UIControllersAndData.Models;
using JetBrains.Annotations;

namespace Assets.Scripts.UIControllersAndData.Storages
{
	/// <inheritdoc />
	/// <summary>
	///     Base class for factories for singleton objects.
	/// </summary>
	/// <typeparam name="TData">Type of a model.</typeparam>
	public abstract class SingletonModelFactory<TData>:ModelBasicFactory<TData> where TData:class, IModel<TData>, new()
	{
		/// <summary>
		///     The persistent relative path to the object.
		/// </summary>
		protected abstract string PersistentPath{get;}

		/// <inheritdoc />
		/// <summary>
		///     Construct a new factory.
		/// </summary>
		/// <param name="storage">A persistent storage.</param>
		protected SingletonModelFactory([NotNull] IPersistentStorage storage):base(storage)
		{
		}

		/// <summary>
		/// Create a new instance of a model.
		/// </summary>
		/// <returns>A newly created object.</returns>
		private TData Create()
		{
			return new TData {ModelFactory = this};
		}

		/// <inheritdoc />
		public override void Put(TData data, Action<bool> callback = null)
		{
			PutData(data, PersistentPath, callback);
		}

		/// <summary>
		///     Method loads an object or creates new if it doesn't exist.
		/// </summary>
		/// <param name="succeed">Called if the operation is succeed. The result object returned as the action argument.</param>
		/// <param name="failed">Called if the operation is failed.</param>
		public void Get([NotNull] Action<TData> succeed, [CanBeNull] Action failed = null)
		{
			GetData(PersistentPath, succeed,
				objectExist =>
				{
					if(!objectExist)
					{
						succeed.Invoke(Create());
					}
					else if(failed != null)
					{
						failed.Invoke();
					}
				});
		}
	}
}