using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace FindAndReplace.App
{
	public class RegistryData
	{
		public string Dir { get; set; }
		public string FileMask { get; set; }
		public bool IncludeSubDirectories { get; set; }
		public string FindText { get; set; }
		public string ReplaceText { get; set; }
		public bool IsCaseSensitive { get; set; }
		public bool FindTextHasRegEx { get; set; }
		public string ExcludeFileMask { get; set; }
	}
	
	public static class RegistryUtils
	{
		public static void SaveToRegistry(Finder finder)
		{
			SaveCommonData();

			SaveValueToRegistry("Dir", finder.Dir);
			SaveValueToRegistry("Filemask", finder.FileMask);
			SaveValueToRegistry("Excludemask", finder.ExcludeFileMask);
			SaveValueToRegistry("FindText", finder.FindText);
			SaveValueToRegistry("IncludesubDir", finder.IncludeSubDirectories.ToString());
			SaveValueToRegistry("CaseSensitive", finder.IsCaseSensitive.ToString());
			SaveValueToRegistry("UseRegularExpressions", finder.FindTextHasRegEx.ToString());
			SaveValueToRegistry("ReplaceText", "");
		}

		public static void SaveToRegistry(Replacer replacer)
		{
			SaveCommonData();

			SaveValueToRegistry("Dir", replacer.Dir);
			SaveValueToRegistry("Filemask", replacer.FileMask);
			SaveValueToRegistry("Excludemask", replacer.ExcludeFileMask);
			SaveValueToRegistry("FindText", replacer.FindText);
			SaveValueToRegistry("IncludesubDir", replacer.IncludeSubDirectories.ToString());
			SaveValueToRegistry("CaseSensitive", replacer.IsCaseSensitive.ToString());
			SaveValueToRegistry("UseRegularExpressions", replacer.FindTextHasRegEx.ToString());
			SaveValueToRegistry("ReplaceText", replacer.ReplaceText);
		}

		public static RegistryData LoadFromRegistry()
		{
			var data = new RegistryData();

			data.Dir = GetValueFromRegistry("Dir");
			data.FileMask = GetValueFromRegistry("Filemask");
			data.ExcludeFileMask = GetValueFromRegistry("Excludemask");
			data.FindText = GetValueFromRegistry("FindText");
			data.IncludeSubDirectories = GetValueFromRegistry("IncludesubDir") == "True" ? true : false;
			data.IsCaseSensitive = GetValueFromRegistry("CaseSensitive") == "True" ? true : false;
			data.FindTextHasRegEx = GetValueFromRegistry("UseRegularExpressions") == "True" ? true : false;
			data.ReplaceText = GetValueFromRegistry("ReplaceText");
			
			return data;
		}

		private static void SaveCommonData()
		{
			SaveValueToRegistry("Company", "Entech Solutions");
			SaveValueToRegistry("Application", "fnr.exe");
		}

		private static void SaveValueToRegistry(string name, string value)
		{
			Application.UserAppDataRegistry.SetValue(name, value, RegistryValueKind.String);
		}

		private static string GetValueFromRegistry(string name)
		{
			var value = Application.UserAppDataRegistry.GetValue(name);

			if (value!=null) return value.ToString();

			return "";
		}
	}
}
