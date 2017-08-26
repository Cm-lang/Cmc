using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using static System.StringComparison;

namespace bCC
{
	public class Environment
	{
		[NotNull] public readonly IList<Declaration> Declarations = new List<Declaration>();

		public static readonly Environment TopEnvironment = new Func<Environment>(() =>
		{
			var ret = new Environment();
			// Do something
			return ret;
		})();

		// FEATURE #18
		[CanBeNull] public readonly Environment Outer;

		public Environment(Environment outer = null) => Outer = outer;

		/// FEATURE #3
		[NotNull]
		public IList<Declaration> FindAllDeclarationsByName([NotNull] string name)
		{
			var env = this;
			var list = new List<Declaration>();
			do
				list.AddRange(env.Declarations.Where(declaration => declaration.Name == name)); while ((env = env.Outer) != null);
			return list;
		}

		/// <summary>
		///   If there's no such declaration, this funciton will return null.
		///   FEATURE #5
		/// </summary>
		/// <param name="name">the name of the required declaration</param>
		/// <returns></returns>
		[CanBeNull]
		public Declaration FindDeclarationByName([NotNull] string name)
		{
			var env = this;
			do
				foreach (var declaration in env.Declarations.Where(declaration => string.Equals(declaration.Name, name, Ordinal)))
					return declaration; while ((env = env.Outer) != null);
			return null;
		}
	}
}