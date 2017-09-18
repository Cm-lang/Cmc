using System;
using System.Collections.Generic;
using Cmc.Core;
using JetBrains.Annotations;

namespace Cmc
{
	/// <summary>
	///   errors
	/// </summary>
	public static class Errors
	{
		[NotNull] public static IList<string> ErrList = new List<string>();
		[NotNull] public static Predicate<string> Remove = ErrList.Remove;

		public static void Add(string s)
		{
			ErrList.Add(s);
			if (Pragma.AbortAtFirst) throw new CompilerException("aborted.");
		}

		public static void AddAndThrow(string s)
		{
			Add(s);
			throw new CompilerException(s);
		}

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
		public CompilerException(
			[NotNull] string message = "compilation aborted due to fatal errors.") : base(message)
		{
		}
	}
}