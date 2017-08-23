using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace bCC
{
	public static class Errors
	{
		[NotNull] public static IList<string> ErrList = new List<string>();
		[NotNull] public static Action<string> Add = ErrList.Add;
		[NotNull] public static Predicate<string> Remove = ErrList.Remove;
	}

	public class CompilerException : Exception
	{
		public CompilerException([NotNull] string message = "") : base(message)
		{
		}
	}
}