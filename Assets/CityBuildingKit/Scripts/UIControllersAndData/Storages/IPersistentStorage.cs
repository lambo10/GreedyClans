/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using System;
using JetBrains.Annotations;

namespace Assets.Scripts.UIControllersAndData.Storages
{
	/// <summary>
	///     The base interface for all persistent storages
	/// </summary>
	public interface IPersistentStorage
	{
		/// <summary>
		///     A root path for the storage.
		/// </summary>		
		[NotNull]
		string RootPath{get;}
		/// <summary>
		///     A root path for all presentations.
		/// </summary>
		[NotNull]
		string PresentationRootPath{get;}

		/// <summary>
		///     A path for all presentations.
		/// </summary>
		[NotNull]
		string PresentationPath{get;}

		/// <summary>
		///     Stores the given data in specified path. Operation status resurns as an action.
		/// </summary>
		/// <param name="path">A path where to put the data.</param>
		/// <param name="data">Byte array with data to save.</param>
		/// <param name="resultCallback">
		///     Called when putting of the data is finished. String and exeption are null if thre is no
		///     error.
		/// </param>
		[Obsolete]
		void PutData([NotNull] string path, [NotNull] byte[] data, [NotNull] Action<string, Exception> resultCallback);

		/// <summary>
		///     Stores the given data in specified path. Operation status resurns as an action.
		/// </summary>
		/// <param name="data">An object to serialize</param>
		/// <param name="path">A relative path where to put the data.</param>
		/// <param name="succeeded">Called when everything went well.</param>
		/// <param name="failed">Called if there is an error.  String and exeption are null if thre is no error.</param>
		/// <typeparam name="TData">A type of the given object.</typeparam>
		void Put<TData>([NotNull] TData data, [NotNull] string path, [NotNull] Action succeeded, [NotNull] Action<string, Exception> failed) where TData:class;

		/// <summary>
		///     Retrieves the requested data from specified path for object with the given name. Result returnes as an action.
		/// </summary>
		/// <param name="path">A path from data should be loaded.</param>
		/// <param name="succeeded">Called if the operation is succeded. The data resurned as the action argument.</param>
		/// <param name="failed">
		///     Called if the operation is failed. A message for error or exception return through the action
		///     arguments.
		/// </param>
		[Obsolete]
		void GetData([NotNull] string path, [NotNull] Action<byte[]> succeeded, [NotNull] Action<string, Exception> failed);

		/// <summary>
		///     Retrieves the requested data from specified path for object with the given name. Result returnes as an action.
		/// </summary>
		/// <param name="path">A relative path from data should be loaded.</param>
		/// <param name="succeeded">Called if the operation is succeed. The data resurned as the action argument.</param>
		/// <param name="failed">
		///     Called if the operation is failed. A message for error or exception return through the action
		///     arguments.
		/// </param>
		/// <typeparam name="TData">A type of the given object.</typeparam>
		void Get<TData>([NotNull] string path, [NotNull] Action<TData> succeeded, [NotNull] Action<string, Exception> failed) where TData:class;
	}
}