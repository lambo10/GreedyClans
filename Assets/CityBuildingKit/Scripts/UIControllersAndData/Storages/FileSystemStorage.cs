/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Assets.Scripts.UIControllersAndData.Storages
{
	public class FileSystemStorage:IPersistentStorage
	{
		#region PrivateConstants
		protected const string PRESENTATIONS_PATH = "presentations";
		private const string _FILE_EXTENSION = ".xml";
#if UNITY_EDITOR
		private const string _DEBUG_DATA_PATH = "DebugData";
#endif
		#endregion // PrivateConstants

		#region Properties
		public string RootPath{get; protected set;}
		/// <inheritdoc />
		public string PresentationRootPath{get; protected set;}
		/// <inheritdoc />
		public string PresentationPath
		{
			get {return PRESENTATIONS_PATH;}
		}
		#endregion // Properties

		/// <summary>
		///     Constructs a new storage. You should avoid creation of a new storage object, we recommend to use
		///     PersistentStorage.Singleton instead.
		/// </summary>
		public FileSystemStorage()
		{
#if UNITY_EDITOR

			string directoryName = Path.GetDirectoryName(Application.dataPath);
			Debug.Assert(directoryName != null, "directoryName != null");
			RootPath = Path.Combine(Path.Combine(directoryName, "Temp"), _DEBUG_DATA_PATH);
#else
			RootPath = Application.persistentDataPath;
#endif
			PresentationRootPath = Path.Combine(RootPath, PRESENTATIONS_PATH);
		}

		/// <inheritdoc />
		[Obsolete]
		public void PutData(string path, byte[] data, Action<string, Exception> resultCallback)
		{
			try
			{
				string directoryName = Path.GetDirectoryName(path);
				// Create a path for the data
				if(!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}

				File.WriteAllBytes(path, data);
			}
			catch(Exception e)
			{
				resultCallback.Invoke("Failed to write data to file '" + path + "'.", e);
				return;
			}

			resultCallback.Invoke(null, null);
		}

		private string MakeFullPath(string path)
		{
			return Path.ChangeExtension(Path.Combine(RootPath, path), _FILE_EXTENSION);
		}

		/// <inheritdoc />
		public void Put<TData>(TData data, string path, Action succeeded, Action<string, Exception> failed) where TData:class
		{
			// Calculate the full path				
			string fullPath = MakeFullPath(path);

			try
			{
				string directoryName = Path.GetDirectoryName(fullPath);
				// Create a path for the data
				if(!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}

				var xmlSerializer = new XmlSerializer(typeof(TData));
				using(var stream = new FileStream(fullPath, FileMode.Create))
				{
					xmlSerializer.Serialize(stream, data);
				}
			}
			catch(Exception e)
			{
				failed.Invoke("Failed to write data to file '" + fullPath + "'.", e);
				return;
			}

			succeeded.Invoke();
		}

		/// <inheritdoc />
		public void GetData(string path, Action<byte[]> succeeded, Action<string, Exception> failed)
		{
			// Check if the given file exist
			if(!File.Exists(path))
			{
				failed("File '" + path + "' doesn't exist.", null);
				return;
			}

			byte[] result;
			try
			{
				// Read the whole file
				result = File.ReadAllBytes(path);
			}
			catch(Exception e)
			{
				failed("Failed to read file '" + path + "'.", e);
				return;
			}

			// Return the data
			succeeded(result);
		}

		public void Get<TData>(string path, Action<TData> succeeded, Action<string, Exception> failed) where TData:class
		{
			TData result;
			// Calculate the full path				
			string fullPath = MakeFullPath(path);

			// Check if the given file exist
			if(!File.Exists(fullPath))
			{
				failed("File '" + fullPath + "' doesn't exist.", null);
				return;
			}

			try
			{
				var xmlSerializer = new XmlSerializer(typeof(TData));
				using(var stream = new FileStream(fullPath, FileMode.Open))
				{
					result = xmlSerializer.Deserialize(stream) as TData;
				}
			}
			catch(Exception e)
			{
				failed("Failed to read file '" + fullPath + "'.", e);
				return;
			}

			if(result != null)
			{
				succeeded.Invoke(result);
			}
			else
			{
				failed("File '" + fullPath + "' doesn't contain an object with type: '" + typeof(TData).FullName + "'", null);
			}
		}
	}
}