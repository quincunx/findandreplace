using System;

namespace FindAndReplace.App
{
	public static class CommandLineUtils
	{
		public static string EncodeText(string original)
		{
			return original
				.Replace(Environment.NewLine, "\\n")
				.Replace("\"", "\\\"");
		}

		public static string DecodeText(string original)
		{
			return original
				.Replace("\\n", Environment.NewLine)
				.Replace("\\\"", "\"");
		}
	}
}


