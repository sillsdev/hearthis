using System;

namespace HearThis.Script
{
	internal class ProjectOpenCancelledException : Exception
	{
		internal ProjectOpenCancelledException(string project, Exception innerException = null) : base($"User cancelled opening of {project}", innerException)
		{
		}
	}
}
