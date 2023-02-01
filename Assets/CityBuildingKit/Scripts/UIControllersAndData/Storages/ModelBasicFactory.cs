/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using System;
using Assets.Scripts.UIControllersAndData.Models;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.UIControllersAndData.Storages
{
	/// <summary>
	///     A basic factory for models.
	/// </summary>
	/// <typeparam name="TData">Type of model.</typeparam>
	public abstract class ModelBasicFactory<TData>:IModelFactory<TData> where TData:class, IModel<TData>
	{
		/// <inheritdoc />
		public IPersistentStorage Storage{get; private set;}

		/// <summary>
		///     Construct a new factory.
		/// </summary>
		/// <param name="storage">A persistent storage for models.</param>
		protected ModelBasicFactory([NotNull] IPersistentStorage storage)
		{
			Storage = storage;
		}

		/// <inheritdoc />
		public abstract void Put(TData data, Action<bool> callback = null);

		/// <summary>
		///     Save the given model object to the specified path.
		/// </summary>
		/// <param name="data">Object to save.</param>
		/// <param name="path">A string with relative path to save the object.</param>
		/// <param name="callback">
		///     An action that should be called when operation if finished. If the operatin is succed, argument
		///     will be true.
		/// </param>
		protected void PutData([NotNull] TData data, [NotNull] string path, Action<bool> callback = null)
		{
			Storage.Put(data,
				path,
				() =>
				{
					// Invoke result callback
					if(callback != null)
					{
						callback.Invoke(true);
					}
				},
				(message, exception) =>
				{
					WriteLog(message, exception);

					// Invoke result callback
					if(callback != null)
					{
						callback.Invoke(false);
					}
				});
		}

		/// <summary>
		///     Retrieves an object from the specified path.
		/// </summary>
		/// <param name="path">A string with relative path for to the required object.</param>
		/// <param name="succeed">Called if the operation is succeed. The data returned as the action argument.</param>
		/// <param name="failed">Called if the operation is failed. If the argument is false, it means the object doesn't exist.</param>
		protected void GetData([NotNull] string path, [NotNull] Action<TData> succeed, [NotNull] Action<bool> failed)
		{
			Storage.Get<TData>(path,
				result =>
				{
					result.ModelFactory = this;
					succeed.Invoke(result);
				},
				(message, exception) =>
				{
					WriteLog(message, exception);
					failed.Invoke(exception != null);
				});
		}

		private static void WriteLog(string message, Exception exception)
		{
			if(exception != null)
			{
				Debug.LogError(message);
				Debug.LogException(exception);
			}
			else
			{
				Debug.LogWarning(message);
			}
		}
	}
}