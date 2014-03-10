using System;
using System.Text;
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

		public static string FormatArg(string original, bool isRegularExpression = false, bool useEscapeChars = false)
		{
			var argValue = EncodeText(original, isRegularExpression, useEscapeChars);

			//windows arg can't end with odd count of '\'
			//args can ends with even count of '\'
			//if arg ends with odd count of '\' we add one more to the end
			var regexPattern = @"\\+$";

			var regex = new Regex(regexPattern);

			var matches = regex.Matches(argValue);

			if (matches.Count > 0)
			{
				var match = matches[0];

				if ((match.Length % 2) != 0)
					argValue += @"\";
			}

			return argValue;
		}

		public static string GenerateCommandLine(
												string dir,
												string fileMask,
												string excludeFileMask,
												bool includeSubDirectories,
												bool isCaseSensitive,
												bool isRegEx,
												bool skipBinaryFileDetection,
												bool showEncoding,
												bool includeFilesWithoutMatches,
												bool useEscapeChars,
												Encoding encoding,
												string find,
												string replace)
		{
			return
				String.Format(
					"--cl --dir \"{0}\" --fileMask \"{1}\"{2}{3}{4}{5}{6}{7}{8}{9}{10} --find \"{11}\" {12}",
					dir.TrimEnd('\\'),
					fileMask,
					String.IsNullOrEmpty(excludeFileMask)
						? ""
						: String.Format(" --excludeFileMask \"{0}\"", CommandLineUtils.EncodeText(excludeFileMask)),
					includeSubDirectories ? " --includeSubDirectories" : "",
					isCaseSensitive ? " --caseSensitive" : "",
					isRegEx ? " --useRegEx" : "",
					skipBinaryFileDetection ? " --skipBinaryFileDetection" : "",
					showEncoding ? " --showEncoding" : "",
					includeFilesWithoutMatches ? " --includeFilesWithoutMatches" : "",
					useEscapeChars ? " --useEscapeChars" : "",
					(encoding != null) ? String.Format(" --alwaysUseEncoding \"{0}\"", encoding.BodyName) : "",
					CommandLineUtils.FormatArg(find, isRegEx, useEscapeChars),
					(replace != null) ? String.Format("--replace \"{0}\"", CommandLineUtils.FormatArg(replace, false, useEscapeChars)) : ""
				);
		}
	}
}


