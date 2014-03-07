using System;
using System.Text.RegularExpressions;

namespace FindAndReplace
{
	public static class CommandLineUtils
	{
		public static string EncodeText(string original, bool isRegularExpression = false, bool useEscapeChars = false)
		{
			var result = original.Replace(Environment.NewLine, "\\n");
			
			if (!isRegularExpression && !useEscapeChars)
				result = result.Replace(@"\", @"\\");

			return result;
		}

		public static string DecodeText(string original, bool hasRegEx = false, bool useEscapeChars = false)
		{
		    string decoded = original;

            //For RegEx there is no new lines, so we don't want to replace \\n
            //See case https://findandreplace.codeplex.com/workitem/17
		    if (!hasRegEx && !useEscapeChars) 
		        decoded = decoded.Replace("\\n", Environment.NewLine);  
			
			decoded = decoded.Replace(@"\\", @"\");
			return decoded;
		}

		public static string HandleCommandlineArgs(string argValue)
		{
			var regexPattern = @"\\+$";

			var regex = new Regex(regexPattern);

			var matches = regex.Matches(argValue);

			if (matches.Count > 0)
			{
				var match = matches[0];

				if ((match.Length%2) != 0)
					argValue += @"\";
			}

			return argValue;
		}
	}
}


