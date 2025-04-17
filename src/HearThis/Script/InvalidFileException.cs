using System;

namespace HearThis.Script
{
	public class InvalidFileException : Exception
	{
		public string Filename { get; }

		public InvalidFileException(string message, string filename) : base(message)
		{
			Filename = filename;
		}
	}
}
