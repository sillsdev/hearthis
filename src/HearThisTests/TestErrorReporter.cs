using System;
using System.Collections.Generic;
using System.Linq;
using SIL.Reporting;

namespace HearThisTests
{
	public class TestErrorReporter : IErrorReporter
	{
		internal class ReportedErrorInfo
		{
			public Exception Exception;
			public string Message;
		}

		private readonly List<ReportedErrorInfo> _reportedProblems = new List<ReportedErrorInfo>();

		internal IReadOnlyList<ReportedErrorInfo> ReportedProblems => _reportedProblems;

		public void ReportFatalException(Exception e)
		{
			throw e;
		}

		public void NotifyUserOfProblem(IRepeatNoticePolicy policy, Exception exception, string message)
		{
			_reportedProblems.Add(new ReportedErrorInfo { Exception = exception, Message = message });
		}

		public ErrorResult NotifyUserOfProblem(IRepeatNoticePolicy policy, string alternateButton1Label,
			ErrorResult resultIfAlternateButtonPressed, string message)
		{
			if (policy == null || policy.ShouldShowMessage(message))
				_reportedProblems.Add(new ReportedErrorInfo { Message = message });

			return resultIfAlternateButtonPressed;
		}

		public void ReportNonFatalException(Exception exception, IRepeatNoticePolicy policy)
		{
			if (policy == null || policy.ShouldShowErrorReportDialog(exception))
				_reportedProblems.Add(new ReportedErrorInfo { Exception = exception });
		}

		public void ReportNonFatalExceptionWithMessage(Exception error, string message, params object[] args)
		{
			if (args != null && args.Any())
				message = string.Format(message, args);
			_reportedProblems.Add(new ReportedErrorInfo { Exception = error, Message = message });
		}

		public void ReportNonFatalMessageWithStackTrace(string message, params object[] args)
		{
			if (args != null && args.Any())
				message = string.Format(message, args);
			_reportedProblems.Add(new ReportedErrorInfo { Message = message });
		}

		public void ReportFatalMessageWithStackTrace(string message, object[] args)
		{
			throw new Exception(message);
		}
	}
}
