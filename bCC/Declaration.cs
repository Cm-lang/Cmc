using System.Collections.Generic;
using System.Linq;
using bCC.Core;
using bCC.Expression;
using JetBrains.Annotations;
using static System.StringComparison;
using static bCC.PrimaryType;

#pragma warning disable 659

namespace bCC
{
	public class Declaration : Statement.Statement
	{
		[NotNull] public readonly string Name;
		public readonly Modifier Modifier;

		public Declaration(
			MetaData metaData,
			[NotNull] string name,
			Modifier modifier) : base(metaData)
		{
			Name = name;
			Modifier = modifier;
		}
	}

	public sealed class VariableDeclaration : Declaration
	{
		[NotNull] public readonly Expression.Expression Expression;
		public readonly bool Mutability;
		public bool IsGlobal = false;
		[CanBeNull] public Type Type;
		public int Align = 8;

		public VariableDeclaration(
			MetaData metaData,
			[NotNull] string name,
			[CanBeNull] Expression.Expression expression = null,
			bool isMutable = false,
			[CanBeNull] Type type = null,
			Modifier modifier = Modifier.Private) :
			base(metaData, name, modifier)
		{
			Expression = expression ?? new NullExpression(MetaData);
			Type = type;
			Mutability = isMutable;
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			Expression.SurroundWith(Env);
			var exprType = Expression.GetExpressionType();
			// FEATURE #8
			Type = Type ?? exprType;
			// FEATURE #30
			Type.SurroundWith(Env);
			if (Type is UnknownType unknownType) Type = unknownType.Resolve();
			if (Type is PrimaryType primaryType) Align = primaryType.Align;
			// FEATURE #11
			if (!string.Equals(exprType.ToString(), NullType, Ordinal) &&
			    !Equals(Type, exprType))
				// FEATURE #9
				Errors.Add($"{MetaData.GetErrorHeader()}type mismatch, expected: {Type}, actual: {exprType}");
		}

		public override bool Equals(object obj) => obj is Declaration declaration && declaration.Name == Name;

		public override IEnumerable<string> Dump() => new[]
			{
				$"variable declaration [{Name}]:\n",
				"  type:\n"
			}
			.Concat(Type?.Dump().Select(MapFunc2) ?? new[] {"    cannot infer!\n"})
			.Concat(new[] {"  initialize expression:\n"})
			.Concat(Expression.Dump().Select(MapFunc2));
	}

	/// <summary>
	///   type aliases
	///   FEATURE #31
	/// </summary>
	public class TypeDeclaration : Declaration
	{
		[NotNull] public readonly Type Type;

		public TypeDeclaration(
			MetaData metaData,
			[NotNull] string name,
			[NotNull] Type type,
			Modifier modifier = Modifier.Private)
			: base(metaData, name, modifier) => Type = type;
	}

	public class StructDeclaration : Declaration
	{
		[NotNull] public readonly IList<VariableDeclaration> FieldList;
		[NotNull] public readonly Type Type;

		public StructDeclaration(
			MetaData metaData,
			[NotNull] string name,
			[NotNull] IList<VariableDeclaration> fieldList,
			Modifier modifier = Modifier.Private) :
			base(metaData, name, modifier)
		{
			FieldList = fieldList;
			Type = new SecondaryType(metaData, name, this);
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			var internalEnv = new Environment(Env);
			foreach (var variableDeclaration in FieldList)
				variableDeclaration.SurroundWith(internalEnv);
		}
	}

	/// <summary>
	///   Probably useless
	/// </summary>
	public class Macro : Declaration
	{
		[NotNull] public string Content;

		public Macro(
			MetaData metaData,
			[NotNull] string name,
			[NotNull] string content,
			Modifier modifier = Modifier.Private) :
			base(metaData, name, modifier) =>
			Content = content;

		public override IEnumerable<string> Dump() => new[] {"macro(this shouldn't appear)\n"};
	}
}