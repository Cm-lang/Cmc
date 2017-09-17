using System;
using System.Collections.Generic;
using Cmc.Stmt;
using JetBrains.Annotations;

namespace Cmc.Core
{
	public abstract class Ast
	{
		[NotNull] public static readonly Func<string, string> MapFunc = i => $"  {i}";
		[NotNull] public static readonly Func<string, string> MapFunc2 = i => $"    {i}";
		public Environment Env;
		public MetaData MetaData;

		/// <summary>
		///  inline/constant folding/etc.
		/// </summary>
		[CanBeNull] public StatementList OptimizedStatementList = null;

		protected Ast(MetaData metaData) => MetaData = metaData;

		public virtual void SurroundWith([NotNull] Environment environment) => Env = Env ?? environment;

		/// <summary>
		///   FEATURE #15
		/// </summary>
		/// <returns>compilation information</returns>
		[NotNull]
		public virtual IEnumerable<string> Dump() => new[] {$"{this}\n"};

		public void PrintDumpInfo() => Console.WriteLine(string.Join("", Dump()));
	}
}