using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FindAndReplace.Tests.ConsoleApp
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			var value = @"\";
			var isRegex = false;

			var encoded = CommandLineUtils.EncodeText(value, isRegex);

			Console.WriteLine("Value: " + value);
			Console.WriteLine("");
			Console.WriteLine("Encoded value: " + encoded);

			Clipboard.SetText(encoded);

			Console.WriteLine("");
			Console.WriteLine("");

			Console.Write("Input or Paste from clipboard: ");

			var dosValue = Console.ReadLine();

			var decodedValue = CommandLineUtils.DecodeText(dosValue, isRegex);

			Console.WriteLine("");
			Console.WriteLine("");

			Console.WriteLine("Decoded value: " + decodedValue);

			Console.WriteLine("");
			Console.WriteLine("");

			Console.WriteLine(decodedValue == value? "Success": "Error");

			Console.ReadKey();
		}
	}
}
