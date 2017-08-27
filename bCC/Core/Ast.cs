using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace bCC.Core
{
	public abstract class Ast
	{
		public static Func<string, string> MapFunc = i => $"  {i}";
		public static Func<string, string> MapFunc2 = i => $"    {i}";
		public Environment Env;
		public MetaData MetaData;

		protected Ast(MetaData metaData) => MetaData = metaData;

		public virtual void SurroundWith([NotNull] Environment environment)
		{
			Env = environment;
		}

		/// <summary>
		///   FEATURE #15
		/// </summary>
		/// <returns>compilation information</returns>
		[NotNull]
		public virtual IEnumerable<string> Dump() => new[] {ToString()};

		public void PrintDumpInfo()
		{
			Console.WriteLine(string.Join("", Dump()));
		}
	}
}