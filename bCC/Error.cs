using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace bCC
{
	/// <summary>
	///   errors
	/// </summary>
	public static class Errors
	{
		[NotNull] public static IList<string> ErrList = new List<string>();
		[NotNull] public static Action<string> Add = ErrList.Add;
		[NotNull] public static Predicate<string> Remove = ErrList.Remove;

		public static void PrintErrorInfo() => Console.WriteLine(string.Join("\n", ErrList));
	}

	/// <summary>
	///   warnings
	/// </summary>
	public static class Warnings
	{
		[NotNull] public static IList<string> WarnList = new List<string>();
		[NotNull] public static Action<string> Add = WarnList.Add;
		[NotNull] public static Predicate<string> Remove = WarnList.Remove;

		public static void PrintErrorInfo() => Console.WriteLine(string.Join("\n", WarnList));
	}

	public class CompilerException : Exception
	{
		public CompilerException([NotNull] string message = "") : base(message)
		{
		}
	}
}