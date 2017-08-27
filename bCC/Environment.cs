using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using static System.StringComparison;
using static bCC.PrimaryType;

namespace bCC
{
	public class Environment
	{
		public static readonly Environment Earth = new Func<Environment>(() =>
		{
			var ret = new Environment();
			// FEATURE #0
			foreach (var typeDeclaration in new[]
					{"i8", "i16", "i32", "i64", "u8", "u16", "u32", "u64", StringType, NullType, BoolType}
				.Select(i => new TypeDeclaration(MetaData.BuiltIn, i, new PrimaryType(MetaData.BuiltIn, i))))
				ret.Declarations.Add(typeDeclaration);
			return ret;
		})();

		[NotNull] public readonly IList<Declaration> Declarations = new List<Declaration>();

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
		/// <returns>the declaration</returns>
		[CanBeNull]
		public Declaration FindDeclarationByName([NotNull] string name)
		{
			var env = this;
			do
				foreach (var declaration in env.Declarations.Where(declaration => string.Equals(declaration.Name, name, Ordinal)))
					return declaration; while ((env = env.Outer) != null);
			return null;
		}

		/// <summary>
		/// Find a declaration satisfying one constraint
		/// </summary>
		/// <param name="predicate">the constraint</param>
		/// <returns>the declaration</returns>
		[CanBeNull]
		public Declaration FindDeclarationSatisfies([NotNull] Predicate<Declaration> predicate)
		{
			var env = this;
			do
				foreach (var declaration in env.Declarations.Where(declaration => predicate(declaration)))
					return declaration; while ((env = env.Outer) != null);
			return null;
		}
	}
}