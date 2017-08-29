using System.Collections.Generic;

namespace bCC.Expression
{
	public static class Constants
	{
		public static readonly IList<string> StringConstants = new List<string>();
		public static readonly IList<int> StringConstantLengths = new List<int>();

		public static int AllocateStringConstant(string value, int len)
		{
			var index = StringConstants.IndexOf(value);
			if (-1 != index) return index;
			StringConstants.Add(value);
			StringConstantLengths.Add(len);
			return StringConstants.Count - 1;
		}
	}
}