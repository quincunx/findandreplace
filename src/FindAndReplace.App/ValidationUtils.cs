using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FindAndReplace.App
{
	public class ValidationResult
	{
		public bool IsSuccess { get; set; }

		public string ErrorMessage { get; set; }
	}
	
	public static class ValidationUtils
	{
		public static ValidationResult IsDirValid(this string dir)
		{
			var result = new ValidationResult() {IsSuccess = true};

			if (dir.Trim() == "")
			{
				result.IsSuccess = false;
				result.ErrorMessage = "Dir is required";
				return result;
			}

			Regex dirRegex = new Regex(@"^(([a-zA-Z]:)|(\\{2}[^\/\\:*?<>|]+))(\\([^\/\\:*?<>|]*))*(\\)?$");
			if (!dirRegex.IsMatch(dir))
			{
				result.IsSuccess = false;
				result.ErrorMessage = "Dir is invalid";
				return result;
			}

			if (!Directory.Exists(dir))
			{
				result.IsSuccess = false;
				result.ErrorMessage = "Dir not exist";
				return result;
			}

			return result;
		}

		public static ValidationResult IsNotEmpty(this string text, string itemName)
		{
			var result = new ValidationResult() { IsSuccess = true };

			if (text.Trim() == "")
			{
				result.IsSuccess = false;
				result.ErrorMessage = String.Format("{0} is required", itemName);
				return result;
			}

			return result;
		}
	}
}
