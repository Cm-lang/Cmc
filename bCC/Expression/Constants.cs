using System.Collections.Generic;

namespace bCC.Expression
{
	public static class Constants
	{
		private static readonly ISet<string> StringConstants = new HashSet<string>();

		public static int AllocateStringConstant(string value)
		{
			StringConstants.Add(value);
			return StringConstants.Count - 1;
		}
	}
}