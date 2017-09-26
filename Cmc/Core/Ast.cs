using System;
using System.Collections.Generic;
using Cmc.Expr;
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
		[CanBeNull] public StatementList OptimizedStatementList;

		protected Ast(MetaData metaData) => MetaData = metaData;

		public virtual void SurroundWith([NotNull] Environment environment) => Env = Env ?? environment;

		public virtual void Transform()
		{
		}

		/// <summary>
		///   FEATURE #15
		/// </summary>
		/// <returns>compilation information</returns>
		[NotNull]
		public abstract IEnumerable<string> Dump();

		[NotNull]
		public abstract IEnumerable<string> DumpCode();

		public void PrintDumpInfo() => Console.WriteLine(string.Join("", Dump()));
		public void PrintCode() => Console.WriteLine(string.Join("", DumpCode()));
	}
}