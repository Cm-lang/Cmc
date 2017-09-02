using System;
using System.Collections.Generic;
using Cmc.Stmt;
using JetBrains.Annotations;

namespace Cmc.Core
{
	public abstract class Ast
	{
		[NotNull] public static Func<string, string> MapFunc = i => $"  {i}";
		[NotNull] public static Func<string, string> MapFunc2 = i => $"    {i}";
		public Environment Env;
		public MetaData MetaData;

		/// <summary>
		///  inline/constant folding/etc.
		/// </summary>
		[CanBeNull] public Statement OptimizedStatementList = null;

		/// <summary>
		///  sometimes you need to convert those complex expressions
		///  or statements into a statement list.
		///
		///  in order to express them as a list of simple expressions
		/// </summary>
		[CanBeNull] public Statement ConvertedStatementList = null;

		protected Ast(MetaData metaData) => MetaData = metaData;

		public virtual void SurroundWith([NotNull] Environment environment) => Env = environment;

		/// <summary>
		///   FEATURE #15
		/// </summary>
		/// <returns>compilation information</returns>
		[NotNull]
		public virtual IEnumerable<string> Dump() => new[] {ToString()};

		public void PrintDumpInfo() => Console.WriteLine(string.Join("", Dump()));
	}
}