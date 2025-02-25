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

namespace HearThis
{
	public class ExternalClipEditorInfo
	{
		public const string kUseDefaultAssociatedApplication = "%default%";
		public const string kClipPathPlaceholder = "{path}";

		private static ExternalClipEditorInfo s_singleton;

		private string _applicationPath;
		private string _commandLineParameters;
		private string _applicationName;

		public string ApplicationPath
		{
			get => _applicationPath;
			set
			{
				_applicationPath = value?.Trim();
				if (_applicationPath?.Length == 0 ||
					!Path.IsPathRooted(_applicationPath) ||
					!File.Exists(_applicationPath))
				{
					_applicationPath = null;
				}

				if (ApplicationName == null && _applicationPath != null)
				{
					SetApplicationNameFromPath();
				}
			}
		}

		public bool UseAssociatedDefaultApplication
		{
			get => _applicationPath == kUseDefaultAssociatedApplication;
			set
			{
				if (value)
				{
					_applicationPath = kUseDefaultAssociatedApplication;
					ApplicationName = null;
				}
				else if (_applicationPath == kUseDefaultAssociatedApplication)
				{
					_applicationPath = null;
					ApplicationName = null;
				}
			}
		}

		public string CommandLineParameters
		{
			get => _commandLineParameters;
			set
			{
				if (UseAssociatedDefaultApplication)
					throw new InvalidOperationException("Parameters cannot be specified when using associated default application.");
				_commandLineParameters = value?.Trim();
				if (_commandLineParameters?.Length == 0)
					_commandLineParameters = null;
			}
		}

		public string ApplicationName
		{
			get => _applicationName;
			set
			{
				_applicationName = value?.Trim();
				if (_applicationName?.Length == 0)
					SetApplicationNameFromPath();
			}
		}

		public string CommandWithParameters
		{
			get
			{
				if (!IsSpecified)
					return null;

				var sb = new StringBuilder(ApplicationPath);
				if (CommandLineParameters != null)
				{
					sb.Append(" ").Append(CommandLineParameters);
				}
				return sb.ToString();
			}
		}

		public bool IsSpecified => ApplicationPath != null;

		public static ExternalClipEditorInfo Singleton =>
			s_singleton ?? (s_singleton = new ExternalClipEditorInfo());

		private ExternalClipEditorInfo() : this(true)
		{
		}

		internal static ExternalClipEditorInfo GetTestInstance() =>
			new ExternalClipEditorInfo(false);

		private ExternalClipEditorInfo(bool loadSettings)
		{
			if (loadSettings)
			{
				ApplicationPath = Settings.Default.ExternalClipEditorPath;
				CommandLineParameters = Settings.Default.ExternalClipEditorArguments;
				ApplicationName = Settings.Default.ExternalClipEditorName;
			}
		}

		public void UpdateSettings()
		{
			Settings.Default.ExternalClipEditorPath = ApplicationPath;
			Settings.Default.ExternalClipEditorArguments = CommandLineParameters;
			Settings.Default.ExternalClipEditorName = ApplicationName;
		}

		public string GetCommandToOpen(string path, out string arguments)
		{
			if (string.IsNullOrWhiteSpace(path))
				throw new ArgumentNullException(nameof(path));

			var pathInQuotes = $"\"{path}\"";
			if (UseAssociatedDefaultApplication)
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

		private void SetApplicationNameFromPath()
		{
			_applicationName =
				Path.GetFileNameWithoutExtension(ApplicationPath)?.ConvertToTitleCase();
		}
	}
}
