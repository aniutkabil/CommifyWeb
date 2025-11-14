namespace Automation.Core.Exceptions
{
	public class WaitException : Exception
	{
		private readonly string _message = "Wait completed, but actual result does not satisfy expectation.";

		public WaitException() { }

		public WaitException(string? message)
		{
			if (!string.IsNullOrEmpty(message))
			{
				_message = message;
			}
		}

		public override string Message => _message;
	}
}
