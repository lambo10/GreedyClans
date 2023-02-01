/**
 * This source file is part of VSLmaker.
 * Copyright © 2017-2018 UASoftware LLC. All rights reserved.
 */

using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FastVSL
{
	public class LogMessage
	{
		private readonly string _message;
		private readonly string _stack;

		public LogMessage(string m, string s)
		{
			_message = m;
			_stack = s;
		}

		public override string ToString()
		{
			var returnString = string.Empty;
			returnString += _message + "\n";
			returnString += _stack + "\n\n";
			return returnString;
		}
	}

	public class InterceptLogMessages : MonoBehaviour
	{
		private readonly List<LogMessage> _messages = new List<LogMessage>();
		private string _logFileName = string.Empty;
		private string _output = string.Empty;

		private Vector2 _scrollViewVector;
		public bool useGUI;

		public bool useInterceptLog;

		private void Start()
		{
			if (!Debug.isDebugBuild) return;
			if (!useInterceptLog)
				return;

			if (_logFileName == string.Empty)
				_logFileName = Application.identifier + ".log.txt";

			var root = Application.persistentDataPath;
			_logFileName = string.Format("{0}/{1}", root, _logFileName);
			DontDestroyOnLoad(this);
		}

		private void OnEnable()
		{
			if (Debug.isDebugBuild)
				Application.logMessageReceived += HandleLog;
		}

		private void OnDisable()
		{
			if (Debug.isDebugBuild)
				Application.logMessageReceived -= HandleLog;
		}

		private void HandleLog(string logString, string stackTrace, LogType type)
		{
			if (string.IsNullOrEmpty(_logFileName))
			{
				return;
			}
			if (_messages.Count == 0 && File.Exists(_logFileName))
				File.Delete(_logFileName);

			var source = new StreamWriter(_logFileName, true);
			source.WriteLine(logString);
			source.WriteLine(stackTrace);
			source.WriteLine(string.Empty);
			source.Flush();
			source.Close();

			if (_messages.Count > 14)
				_messages.RemoveAt(0);

			_messages.Add(new LogMessage(logString, stackTrace));
			_output = string.Empty;
			foreach (var aMessage in _messages)
				_output += aMessage.ToString();
		}

		private void OnGUI()
		{
			if (!Debug.isDebugBuild || !useInterceptLog) return;
			if (GUI.Button(new Rect(Screen.width * 0.01f, Screen.height - 36, 100, 35), "Log Window"))
				useGUI = !useGUI;

			if (GUI.Button(new Rect(Screen.width * 0.99f - 100, Screen.height - 36, 100, 35), "Clear"))
			{
				_output = string.Empty;
				_messages.Clear();
			}

			if (!useGUI) return;
			GUI.Box(new Rect(5, 5, Screen.width - 10, Screen.height - 10), "");
			GUILayout.BeginArea(new Rect(5, 5, Screen.width - 10, Screen.height - 10));
			_scrollViewVector = GUILayout.BeginScrollView(_scrollViewVector);
			GUILayout.Label(_output);
			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}
	}
}