using System.Windows.Forms;
using Microsoft.Win32;

namespace FindAndReplace.App
{
	public class FormData
	{
        public bool IsFindOnly { get; set; }

		public string Dir { get; set; }
		public string FileMask { get; set; }
		public bool IncludeSubDirectories { get; set; }
		public string FindText { get; set; }
		public string ReplaceText { get; set; }
		public bool IsCaseSensitive { get; set; }
		public bool IsRegEx { get; set; }
		public bool SkipBinaryFileDetection { get; set; }
		public bool IncludeFilesWithoutMatches { get; set; }
		public string ExcludeFileMask { get; set; }


		public void SaveToRegistry()
		{
			SaveValueToRegistry("Dir", Dir);
			SaveValueToRegistry("FileMask", FileMask);
			SaveValueToRegistry("ExcludeFileMask", ExcludeFileMask);
			SaveValueToRegistry("FindText", FindText);
			SaveValueToRegistry("IncludeSubDirectories", IncludeSubDirectories.ToString());
			SaveValueToRegistry("IsCaseSensitive", IsCaseSensitive.ToString());
			SaveValueToRegistry("IsRegEx", IsRegEx.ToString());
			SaveValueToRegistry("SkipBinaryFileDetection", SkipBinaryFileDetection.ToString());
			SaveValueToRegistry("IncludeFilesWithoutMatches", IncludeFilesWithoutMatches.ToString());
			SaveValueToRegistry("ReplaceText", ReplaceText);
		}

		public bool IsEmpty()
		{
			//When saved even once dir will have a non null volue
			string dir = GetValueFromRegistry("Dir");
			return dir == null;
		}

		public void LoadFromRegistry()
		{
			Dir = GetValueFromRegistry("Dir");
			FileMask = GetValueFromRegistry("Filemask");
			ExcludeFileMask = GetValueFromRegistry("ExcludeFileMask");
			FindText = GetValueFromRegistry("FindText");
			IncludeSubDirectories = GetValueFromRegistry("IncludeSubDirectories") == "True";
			IsCaseSensitive = GetValueFromRegistry("IsCaseSensitive") == "True";
			IsRegEx = GetValueFromRegistry("IsRegEx") == "True";
			SkipBinaryFileDetection = GetValueFromRegistry("SkipBinaryFileDetection") == "True";
			IncludeFilesWithoutMatches = GetValueFromRegistry("IncludeFilesWithoutMatches") == "True";
			ReplaceText = GetValueFromRegistry("ReplaceText");
		}


		private void SaveValueToRegistry(string name, string value)
		{
			Application.UserAppDataRegistry.SetValue(name, value, RegistryValueKind.String);
		}

		private string GetValueFromRegistry(string name)
		{
			var value = Application.UserAppDataRegistry.GetValue(name);

			if (value != null)
				return value.ToString();

			return null;
		}
	}

}