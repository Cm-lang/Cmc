using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace bCC
{
	public abstract class Ast
	{
		public Environment Env;
		public MetaData MetaData;

		public virtual void SurroundWith([NotNull] Environment environment) => Env = environment;

		protected Ast(MetaData metaData) => MetaData = metaData;

		/// <summary>
		/// FEATURE #15
		/// </summary>
		/// <returns>compilation information</returns>
		[NotNull]
		public virtual IEnumerable<string> Dump() => new[] {ToString()};

		public void PrintDumpInfo() => Console.WriteLine(string.Join("", Dump()));

		public static Func<string, string> MapFunc = i => $"  {i}";
		public static Func<string, string> MapFunc2 = i => $"    {i}";
	}
}