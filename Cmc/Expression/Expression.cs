using System;
using System.Collections.Generic;
using System.Linq;
using Cmc.Core;
using JetBrains.Annotations;
using Environment = Cmc.Core.Environment;

#pragma warning disable 659

namespace Cmc.Expression
{
	public abstract class Expression : Ast
	{
		protected Expression(MetaData metaData) : base(metaData)
		{
		}

		[NotNull]
		public abstract Type GetExpressionType();

		[CanBeNull]
		public virtual VariableExpression GetLhsExpression()
		{
			return null;
		}
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

		public override Type GetExpressionType()
		{
			return new PrimaryType(MetaData, PrimaryType.NullType);
		}

		public override IEnumerable<string> Dump()
		{
			return new[] {"null expression\n"};
		}
	}

	public class LiteralExpression : AtomicExpression
	{
		[NotNull] public readonly Type Type;

		protected LiteralExpression(MetaData metaData, [NotNull] Type type) : base(metaData)
		{
			Type = type;
		}

		public override Type GetExpressionType()
		{
			return Type;
		}
	}

	public class IntLiteralExpression : LiteralExpression
	{
		public static readonly int[] AcceptableLength = {8, 16, 32, 64};
		public readonly int Length;

		[NotNull] public readonly string Value;

		public IntLiteralExpression(
			MetaData metaData,
			[NotNull] string value,
			// FEATURE #27
			bool isSigned,
			int length = 32)
			: base(metaData, new PrimaryType(metaData, $"{(isSigned ? "i" : "u")}{length}", length / 8))
		{
			Value = value;
			Length = length;
			// FEATURE #26
			if (!AcceptableLength.Contains(length))
				Errors.Add($"{MetaData.GetErrorHeader()}integer length of {length} is not supported");
		}

		public override IEnumerable<string> Dump()
		{
			return new[] {$"literal expression [{Value}]:\n"}
				.Concat(Type.Dump().Select(MapFunc));
		}
	}

	public class BoolLiteralExpression : LiteralExpression
	{
		public readonly bool Value;

		public BoolLiteralExpression(MetaData metaData, bool value) : base(metaData,
			new PrimaryType(metaData, PrimaryType.BoolType, 1))
		{
			Value = value;
		}

		public int ValueToInt()
		{
			return Value ? 1 : 0;
		}

		public override IEnumerable<string> Dump()
		{
			return new[] {$"bool literal expression [{Value}]:\n"};
		}
	}

	public class StringLiteralExpression : LiteralExpression
	{
		public readonly int ConstantPoolIndex;
		public readonly int Length;
		public readonly string Value;

		public StringLiteralExpression(MetaData metaData, string value) : base(metaData,
			new PrimaryType(MetaData.Empty, PrimaryType.StringType))
		{
			Length = value.Length + 1;
			Value = string.Concat(
				        from i in value
				        select char.IsLetterOrDigit(i)
					        ? i.ToString()
					        : $"\\{Convert.ToString((byte) i, 16)}") + "\\00";
			ConstantPoolIndex = Constants.AllocateStringConstant(Value, Length);
		}

		/// <summary>
		///     FEATURE #23
		/// </summary>
		/// <returns>dump msg</returns>
		public override IEnumerable<string> Dump()
		{
			return new[]
					{$"string literal expression <{ConstantPoolIndex}>[{Value}]:\n"}
				.Concat(Type.Dump().Select(MapFunc));
		}
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
			if (!(Owner.GetExpressionType() is SecondaryType type) ||
			    !type.Struct.FieldList.Contains(Member.Declaration))
				Errors.Add(MetaData.GetErrorHeader() + "invalid member access expression");
		}

		public override Type GetExpressionType()
		{
			return Member.GetExpressionType();
		}

		public override VariableExpression GetLhsExpression()
		{
			return Member.GetLhsExpression();
		}
	}

	public class VariableExpression : AtomicExpression
	{
		[NotNull] public readonly string Name;
		[CanBeNull] private Type _type;
		[CanBeNull] public VariableDeclaration Declaration;

		public VariableExpression(
			MetaData metaData,
			[NotNull] string name) :
			base(metaData)
		{
			Name = name;
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			var declaration = Env.FindDeclarationByName(Name);
			if (declaration is VariableDeclaration variableDeclaration)
			{
				Declaration = variableDeclaration;
				Declaration.Used = true;
				_type = Declaration.Type;
			}
			else
			{
				Errors.Add($"{MetaData.GetErrorHeader()}{declaration} isn't a variable");
			}
		}

		public override Type GetExpressionType()
		{
			return _type ?? throw new CompilerException("unknown type");
		}

		public override IEnumerable<string> Dump()
		{
			return new[]
				{
					$"variable expression [{Name}]:\n",
					"  type:\n"
				}
				.Concat(_type?.Dump().Select(MapFunc2) ?? new[] {"    cannot infer!\n"});
		}

		public override VariableExpression GetLhsExpression()
		{
			return this;
		}
	}

	public class HoleExpression : Expression
	{
		public HoleExpression(MetaData metaData) : base(metaData)
		{
		}

		public override Type GetExpressionType()
		{
			throw new CompilerException("cannot get hole's type");
		}
	}
}