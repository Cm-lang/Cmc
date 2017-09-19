using System;
using System.Collections.Generic;
using System.Linq;
using Cmc.Decl;
using Cmc.Expr;
using Cmc.Stmt;
using JetBrains.Annotations;
using static System.StringComparison;

namespace Cmc.Core
{
	public class Environment
	{
		/// <summary>
		///  preload functions/types
		/// </summary>
		public static Environment Galaxy;

		private static readonly MetaData BuiltIn = new MetaData(-1, "[built-in]");

		public static Environment SolarSystem;

		[NotNull] public readonly IList<Declaration> Declarations = new List<Declaration>();

		// FEATURE #18
		[CanBeNull] private readonly Environment _outer;

		public static void Initialize()
		{
			Galaxy = new Environment();
			SolarSystem = new Environment(Galaxy);

			// FEATURE #0
			foreach (var typeDeclaration in
				from builtinType in new[]
				{
					"i8", "i16", "i32", "i64",
					"u8", "u16", "u32", "u64",
					"f32", "f64",
					PrimaryType.StringType,
					PrimaryType.NullType,
					PrimaryType.BoolType
				}
				select new TypeDeclaration(
					BuiltIn,
					builtinType,
					new PrimaryType(BuiltIn, builtinType)))
				Galaxy.Declarations.Add(typeDeclaration);
			var puts = new ExternDeclaration(BuiltIn, "print", null,
				new LambdaType(BuiltIn,
					new List<Type>
					{
						new PrimaryType(BuiltIn, PrimaryType.StringType)
					},
					new PrimaryType(BuiltIn, PrimaryType.NullType)));
			puts.SurroundWith(Galaxy);
			SolarSystem.Declarations.Add(puts);
		}

		static Environment() => Initialize();

		public Environment(Environment outer = null) => _outer = outer;

		/// FEATURE #3
		[NotNull]
		public IList<Declaration> FindAllDeclarationsByName([NotNull] string name)
		{
			var env = this;
			var list = new List<Declaration>();
			do
			{
				list.AddRange(
					from declaration in env.Declarations
					where string.Equals(declaration.Name, name, Ordinal)
					select declaration);
			} while ((env = env._outer) != null);
			return list;
		}

		/// <summary>
		///     If there's no such declaration, this funciton will return null.
		///     FEATURE #5
		/// </summary>
		/// <param name="name">the name of the required declaration</param>
		/// <returns>the declaration</returns>
		[CanBeNull]
		public Declaration FindDeclarationByName([NotNull] string name) =>
			FindDeclarationSatisfies(i => string.Equals(i.Name, name, Ordinal));

		/// <summary>
		///     If there's no such declaration, this funciton will return null.
		/// </summary>
		/// <param name="name">
		///     the name of the required declaration.
		///     if it's null, find the nearest one.
		/// </param>
		/// <returns>the declaration</returns>
		[CanBeNull]
		public ReturnLabelDeclaration FindReturnLabelByName([NotNull] string name) =>
			(ReturnLabelDeclaration) FindDeclarationSatisfies(i =>
				i is ReturnLabelDeclaration && string.Equals(i.Name, name, Ordinal));

		[CanBeNull]
		public JumpLabelDeclaration FindJumpLabelByName([NotNull] string name) =>
			(JumpLabelDeclaration) FindDeclarationSatisfies(i =>
				i is JumpLabelDeclaration && string.Equals(i.Name, name, Ordinal));

		/// <summary>
		///     Find a declaration satisfying one constraint
		/// </summary>
		/// <param name="predicate">the constraint</param>
		/// <returns>the declaration</returns>
		[CanBeNull]
		public Declaration FindDeclarationSatisfies([NotNull] Predicate<Declaration> predicate)
		{
			var env = this;
			do
			{
				foreach (var declaration in
					from declaration in env.Declarations
					where predicate(declaration)
					select declaration)
					return declaration;
			} while ((env = env._outer) != null);
			return null;
		}
	}
}