using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

#pragma warning disable 659

namespace bCC.Expression
{
	public abstract class Expression : Ast
	{
		protected Expression(MetaData metaData) : base(metaData)
		{
		}

		[NotNull]
		public abstract Type GetExpressionType();

		[CanBeNull]
		public virtual VariableExpression GetLhsExpression() => null;
	}

	public abstract class AtomicExpression : Expression
	{
		protected AtomicExpression(MetaData metaData) : base(metaData)
		{
		}
	}

	public sealed class NullExpression : Expression
	{
		public NullExpression(MetaData metaData) : base(metaData)
		{
		}

		public override Type GetExpressionType() => new PrimaryType(MetaData, PrimaryType.NullType);

		public override IEnumerable<string> Dump() => new[] {"null expression\n"};
	}

	public class LiteralExpression : AtomicExpression
	{
		[NotNull] public readonly Type Type;

		protected LiteralExpression(MetaData metaData, [NotNull] Type type) : base(metaData) => Type = type;

		public override Type GetExpressionType() => Type;
	}

	public class IntLiteralExpression : LiteralExpression
	{
		public static readonly int[] AcceptableLength = {8, 16, 32, 64};

		[NotNull] public readonly string Value;

		public IntLiteralExpression(
			MetaData metaData,
			[NotNull] string value,
			// FEATURE #27
			bool isSigned,
			int length = 32)
			: base(metaData, new PrimaryType(metaData, $"{(isSigned ? "i" : "u")}{length}"))
		{
			Value = value;
			// FEATURE #26
			if (!AcceptableLength.Contains(length))
				Errors.Add($"{MetaData.GetErrorHeader()}integer length of {length} is not supported");
		}

		public override IEnumerable<string> Dump() => new[] {$"literal expression [{Value}]:\n"}
			.Concat(Type.Dump().Select(MapFunc));
	}

	public class BoolLiteralExpression : LiteralExpression
	{
		public readonly bool Value;

		public BoolLiteralExpression(MetaData metaData, bool value) : base(metaData,
			new PrimaryType(metaData, PrimaryType.BoolType)) =>
			Value = value;

		public override IEnumerable<string> Dump() => new[] {$"bool literal expression [{Value}]:\n"};
	}

	public class StringLiteralExpression : LiteralExpression
	{
		private readonly string _debugRepresentation;
		public readonly string Value;

		public StringLiteralExpression(MetaData metaData, string value) : base(metaData,
			new PrimaryType(MetaData.Empty, PrimaryType.StringType))
		{
			Value = value;
			// FEATURE #23
			_debugRepresentation = Value
				.Replace("\\", "\\\\")
				.Replace("\r", "\\r")
				.Replace("\b", "\\b")
				.Replace("\a", "\\a")
				.Replace("\f", "\\f")
				.Replace("\n", "\\n")
				.Replace("\t", "\\t")
				.Replace("\"", "\\\"");
		}

		public override IEnumerable<string> Dump() => new[]
				{$"string literal expression [{_debugRepresentation}]:\n"}
			.Concat(Type.Dump().Select(MapFunc));
	}

	public class MemberAccessExpression : AtomicExpression
	{
		[NotNull] public readonly VariableExpression Member;
		[NotNull] public readonly Expression Owner;

		public MemberAccessExpression(
			MetaData metaData,
			[NotNull] Expression owner,
			[NotNull] VariableExpression member) :
			base(metaData)
		{
			Owner = owner;
			Member = member;
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			Owner.SurroundWith(Env);
			Member.SurroundWith(Env);
			// FEATURE #29
			if (!(Owner.GetExpressionType() is SecondaryType type) || !type.Struct.FieldList.Contains(Member.Declaration))
				Errors.Add(MetaData.GetErrorHeader() + "invalid member access expression");
		}

		public override Type GetExpressionType() => Member.GetExpressionType();

		public override VariableExpression GetLhsExpression() => Member.GetLhsExpression();
	}

	public class VariableExpression : AtomicExpression
	{
		[NotNull] public readonly string Name;
		private Type _type;
		public VariableDeclaration Declaration;

		public VariableExpression(MetaData metaData, [NotNull] string name) : base(metaData) => Name = name;

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			var declaration = Env.FindDeclarationByName(Name);
			if (declaration is VariableDeclaration variableDeclaration)
			{
				Declaration = variableDeclaration;
				_type = Declaration.Type;
			}
			else
				Errors.Add($"{MetaData.GetErrorHeader()}{declaration} isn't a variable");
		}

		public override Type GetExpressionType() => _type ?? throw new CompilerException("unknown type");

		public override IEnumerable<string> Dump() => new[]
			{
				$"variable expression [{Name}]:\n",
				"  type:\n"
			}
			.Concat(_type.Dump().Select(MapFunc2));

		public override VariableExpression GetLhsExpression() => this;
	}

	public class HoleExpression : Expression
	{
		public HoleExpression(MetaData metaData) : base(metaData)
		{
		}

		public override Type GetExpressionType() => throw new CompilerException("cannot get hole's type");
	}
}