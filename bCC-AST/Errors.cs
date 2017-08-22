using System;
using System.Collections.Generic;

namespace bCC
{
	public static class Errors
	{
		public static IList<string> ErrList = new List<string>();
		public static Action<string> Add = ErrList.Add;
		public static Predicate<string> Remove = ErrList.Remove;
	}

	public class CompilerException : Exception
	{
		public CompilerException(string message = "") : base(message)
		{
		}
	}
}