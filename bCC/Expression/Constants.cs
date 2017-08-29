using System.Collections.Generic;

namespace bCC.Expression
{
	public static class Constants
	{
		public static readonly IList<string> StringConstants = new List<string>();

		public static int AllocateStringConstant(string value)
		{
			var index = StringConstants.IndexOf(value);
			if (-1 != index) return index;
			StringConstants.Add(value);
			return StringConstants.Count - 1;
		}
		
	}
}