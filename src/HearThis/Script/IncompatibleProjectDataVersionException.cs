using System;

namespace HearThis.Script
{
	class IncompatibleProjectDataVersionException : Exception
	{
		public string Project { get; }
		public int ProjectVersion { get; }

		public IncompatibleProjectDataVersionException(string project, int projectVersion) :
			base()
		{
			Project = project;
			ProjectVersion = projectVersion;
		}
	}
}
