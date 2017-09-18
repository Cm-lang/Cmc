using System;
using System.Collections.Generic;
using System.Linq;
using Cmc.Core;
using Cmc.Stmt;
using JetBrains.Annotations;
using Environment = Cmc.Core.Environment;

#pragma warning disable 659

namespace Cmc.Expr
{
	public class ExpressionConvertedResult
	{
		[NotNull] public IList<Statement> ConvertedStatements;
		[NotNull] public Expression ConvertedExpression;

		public ExpressionConvertedResult(
			[NotNull] IList<Statement> convertedStatements,
			[NotNull] Expression convertedExpression)
		{
			ConvertedStatements = convertedStatements;
			ConvertedExpression = convertedExpression;
		}
	}

	public abstract class Expression : Ast
	{
		protected Expression(MetaData metaData) : base(metaData)
		{
		}

		[CanBeNull] public ExpressionConvertedResult ConvertedResult = null;

		[NotNull]
		public abstract Type GetExpressionType();

		[CanBeNull]
		public virtual VariableExpression GetLhsExpression() => null;
	}

	public sealed class NullExpression : AtomicExpression
	{
		public NullExpression(MetaData metaData) : base(metaData)
		{
		}

		public override Type GetExpressionType() => new PrimaryType(MetaData, PrimaryType.NullType);

		public override IEnumerable<string> Dump() => new[] {"null expression\n"};
	}

	/// <summary>
	///  expressions that can be used directly
	///  without turning it into a variable
	/// </summary>
	public abstract class AtomicExpression : Expression
	{
		protected AtomicExpression(MetaData metaData) : base(metaData)
		{
		}
	}

	public class StringLiteralExpression : Expression
	{
		public readonly int ConstantPoolIndex;
		public readonly int Length;
		[NotNull] public readonly string Value;

		[NotNull] public static readonly Type Type =
			new PrimaryType(MetaData.Empty, PrimaryType.StringType);

		public StringLiteralExpression(
			MetaData metaData,
			[NotNull] string value) :
			base(metaData)
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
		public override IEnumerable<string> Dump() =>
			new[] {$"string literal expression <{ConstantPoolIndex}>[{Value}]:\n"}
				.Concat(Type.Dump().Select(MapFunc));

		public override Type GetExpressionType() => Type;
	}

	public class MemberAccessExpression : Expression
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
			if (null != Owner.ConvertedResult)
			{
			}
		}

		public override Type GetExpressionType() => Member.GetExpressionType();

		public override VariableExpression GetLhsExpression() => Member.GetLhsExpression();
	}

	/// <summary>
	///  Idris-style hole-oriented programming
	/// </summary>
	public sealed class HoleExpression : Expression
	{
		public HoleExpression(MetaData metaData) : base(metaData)
		{
		}

		public override Type GetExpressionType() =>
			throw new CompilerException("cannot get hole's type");
	}
}