// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2014-2025, SIL Global.
// <copyright from='2024' to='2025' company='SIL Global'>
//		Copyright (c) 2014-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.IO;
using System.Text;
using HearThis.Properties;
using PtxUtils;
using SIL.Reporting;
using static System.String;

namespace HearThis
{
	public class ExternalClipEditorInfo
	{
		public const string kClipPathPlaceholder = "{path}";

		public delegate void SettingsChangedHandler(ExternalClipEditorInfo sender);
		public event SettingsChangedHandler SettingsChanged;

		/// <summary>
		/// Only instance that is loaded from and persisted to settings.
		/// </summary>
		private static ExternalClipEditorInfo s_persistedSingleton;

		private string _applicationPath;
		private string _commandLineParameters;
		private string _applicationName;
		private bool _updatingSettings;

		public string ApplicationPath
		{
			get => _applicationPath;
			set
			{
				var origValue = _applicationPath;
				var resetAppName = ApplicationName == null ||
					ApplicationName == GetDefaultApplicationNameFromPath();

				_applicationPath = value?.Trim();
				try
				{
					if (_applicationPath?.Length == 0 ||
						!Path.IsPathRooted(_applicationPath) ||
						!File.Exists(_applicationPath))
					{
						_applicationPath = null;
						_applicationName = null;
					}
				}
				catch (Exception e)
				{
					Logger.WriteError(e);
					_applicationPath = null;
					_applicationName = null;
				}

				if (resetAppName && _applicationPath != null)
					SetApplicationNameFromPath();

				if (!_updatingSettings && origValue != _applicationPath)
					SettingsChanged?.Invoke(this);
			}
		}

		public void UseAssociatedDefaultApplication()
		{
			_applicationPath = null;
			ApplicationName = null;
		}

		private bool IsUsingDefaultAssociatedApp => _applicationPath == null;

		public string CommandLineParameters
		{
			get => _commandLineParameters;
			set
			{
				var origValue = _commandLineParameters;
				_commandLineParameters = value?.Trim();
				if (_commandLineParameters?.Length == 0)
					_commandLineParameters = null;
				if (_commandLineParameters != null && IsUsingDefaultAssociatedApp)
					throw new InvalidOperationException("Parameters cannot be specified when using associated default application.");
				if (!_updatingSettings && origValue != _commandLineParameters)
					SettingsChanged?.Invoke(this);
			}
		}

		public string ApplicationName
		{
			get => _applicationName;
			set
			{
				var origValue = _applicationName;
				_applicationName = value?.Trim();
				if (_applicationName?.Length == 0)
					SetApplicationNameFromPath();
				if (_applicationName != null && _applicationName.Length== 0)
					_applicationName = null;
				if (!_updatingSettings && origValue != _applicationName)
					SettingsChanged?.Invoke(this);
			}
		}

		public bool IsSpecified => !IsNullOrEmpty(ApplicationPath);

		/// <summary>
		/// Only instance that is loaded from and persisted to settings.
		/// </summary>
		public static ExternalClipEditorInfo PersistedSingleton
		{
			get
			{
				if (s_persistedSingleton == null)
				{
					s_persistedSingleton = new ExternalClipEditorInfo
					{
						ApplicationPath = Settings.Default.ExternalClipEditorPath,
						CommandLineParameters = Settings.Default.ExternalClipEditorArguments,
						ApplicationName = Settings.Default.ExternalClipEditorName
					};
				}
				return s_persistedSingleton;
			}
		}
	
		/// <summary>
		/// Create a cloned instance that is not persisted to settings.
		/// </summary>
		/// <returns>The cloned instance</returns>
		public ExternalClipEditorInfo Clone()
		{
			return new ExternalClipEditorInfo
			{
				ApplicationPath = ApplicationPath,
				CommandLineParameters = CommandLineParameters,
				ApplicationName = ApplicationName
			};
		}

		public void UpdateSettings(ExternalClipEditorInfo from)
		{
			if (this != PersistedSingleton)
				throw new InvalidOperationException("Only the persisted singleton can update settings.");

			var changed = ApplicationPath != from.ApplicationPath ||
				CommandLineParameters != from.CommandLineParameters ||
				ApplicationName != from.ApplicationName;
			if (!changed)
				return;

			_updatingSettings = true;
			Settings.Default.ExternalClipEditorPath =
				ApplicationPath = from.ApplicationPath;
			Settings.Default.ExternalClipEditorArguments =
				CommandLineParameters = from.CommandLineParameters;
			Settings.Default.ExternalClipEditorName =
				ApplicationName = from.ApplicationName;
			_updatingSettings = false;
			SettingsChanged?.Invoke(this);
		}

		public string GetCommandToOpen(string path, out string arguments)
		{
			if (IsNullOrWhiteSpace(path))
				throw new ArgumentNullException(nameof(path));

			var pathInQuotes = $"\"{path}\"";
			if (IsUsingDefaultAssociatedApp)
			{
				arguments = null;
				return pathInQuotes;
			}

			var sb = new StringBuilder();
			if (CommandLineParameters != null && CommandLineParameters.Contains(kClipPathPlaceholder))
			{
					sb.Append(CommandLineParameters.Replace(kClipPathPlaceholder, pathInQuotes));
			}
			else
			{
				if (CommandLineParameters != null)
				{
					sb.Append(CommandLineParameters);
					sb.Append(" ");
				}

				sb.Append(pathInQuotes);
			}
			arguments = sb.ToString();
			return ApplicationPath;
		}

		public string GetDefaultApplicationNameFromPath()
		{
			return Path.GetFileNameWithoutExtension(ApplicationPath)?.ConvertToTitleCase();
		}

		private void SetApplicationNameFromPath()
		{
			_applicationName = GetDefaultApplicationNameFromPath();
		}
	}
}
