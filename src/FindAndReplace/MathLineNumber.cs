using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FindAndReplace
{
	public class MathLineNumber
	{
		public int LineNumber { get; set; }

		public bool HasMatch { get; set; }
	}

	public class LineNumberComparer : IEqualityComparer<MathLineNumber>
	{
		public bool Equals(MathLineNumber x, MathLineNumber y)
		{
			return x.LineNumber == y.LineNumber;
		}

		public int GetHashCode(MathLineNumber obj)
		{
			return obj.LineNumber.GetHashCode();
		}
	}
}
