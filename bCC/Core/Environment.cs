using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Cmc.Core
{
	public class Environment
	{
		public static readonly Environment SolarSystem;

		[NotNull] public readonly IList<Declaration> Declarations = new List<Declaration>();

		// FEATURE #18
		[CanBeNull] public readonly Environment Outer;

		static Environment()
		{
			SolarSystem = new Environment();

			// FEATURE #0
			foreach (var typeDeclaration in from builtinType in new[]
				{
					"i8", "i16", "i32", "i64", "u8", "u16", "u32", "u64", PrimaryType.StringType, PrimaryType.NullType,
					PrimaryType.BoolType
				}
				select new TypeDeclaration(
					MetaData.BuiltIn,
					builtinType,
					new PrimaryType(MetaData.BuiltIn, builtinType)))
				SolarSystem.Declarations.Add(typeDeclaration);
		}

		public Environment(Environment outer = null) => Outer = outer;

		/// FEATURE #3
		[NotNull]
		public IList<Declaration> FindAllDeclarationsByName([NotNull] string name)
		{
			var env = this;
			var list = new List<Declaration>();
			do
				list.AddRange(
					from declaration in env.Declarations
					where string.Equals(declaration.Name, name, StringComparison.Ordinal)
					select declaration); while ((env = env.Outer) != null);
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
				foreach (var declaration in
					from declaration in env.Declarations
					where string.Equals(declaration.Name, name, StringComparison.Ordinal)
					select declaration)
					return declaration; while ((env = env.Outer) != null);
			return null;
		}

		/// <summary>
		///   Find a declaration satisfying one constraint
		/// </summary>
		/// <param name="predicate">the constraint</param>
		/// <returns>the declaration</returns>
		[CanBeNull]
		public Declaration FindDeclarationSatisfies([NotNull] Predicate<Declaration> predicate)
		{
			var env = this;
			do
				foreach (var declaration in
					from declaration in env.Declarations
					where predicate(declaration)
					select declaration)
					return declaration; while ((env = env.Outer) != null);
			return null;
		}
	}
}