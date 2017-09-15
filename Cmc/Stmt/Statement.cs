using System;
using System.Collections.Generic;
using System.Linq;
using Cmc.Core;
using Cmc.Decl;
using Cmc.Expr;
using JetBrains.Annotations;
using Environment = Cmc.Core.Environment;

namespace Cmc.Stmt
{
	public class Statement : Ast
	{
		public Statement(MetaData metaData) : base(metaData)
		{
		}

		/// <summary>
		///  sometimes you need to convert those complex expressions
		///  or statements into a statement list.
		///
		///  in order to express them as a list of simple expressions
		/// </summary>
		[CanBeNull] public StatementList ConvertedStatementList;

		[NotNull]
		public virtual IEnumerable<ReturnStatement> FindReturnStatements() => new List<ReturnStatement>(0);

		[NotNull]
		public virtual IEnumerable<JumpStatement> FindJumpStatements() => new List<JumpStatement>(0);

		public override IEnumerable<string> Dump() => new[] {"empty statement"};
	}

	/// <summary>
	///  represent nothing
	/// </summary>
	public class EmptyStatement : Statement
	{
		public EmptyStatement(MetaData metaData) : base(metaData)
		{
		}
	}

	public class ExpressionStatement : Statement
	{
		[NotNull] public readonly Expression Expression;

		public ExpressionStatement(
			MetaData metaData,
			[NotNull] Expression expression) :
			base(metaData) =>
			Expression = expression;

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			Expression.SurroundWith(Env);
			var res = Expression.ConvertedResult;
			if (null != res)
				ConvertedStatementList = new StatementList(MetaData,
					res.ConvertedStatements.Concat(new[]
					{
						new ExpressionStatement(MetaData, res.ConvertedExpression)
					}).ToArray());
		}

		public override IEnumerable<string> Dump() => new[] {"expression statement:\n"}
			.Concat(Expression.Dump().Select(MapFunc));
	}

	public class ReturnStatement : ExpressionStatement
	{
		public LambdaExpression WhereToJump;

		public ReturnStatement(
			MetaData metaData,
			[CanBeNull] Expression expression = null) :
			base(metaData, expression ?? new NullExpression(metaData))
		{
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			if (Expression is AtomicExpression) return;
			var variableName = $"{MetaData.TrimedFileName}{MetaData.LineNumber}{GetHashCode()}";
			ConvertedStatementList = new StatementList(MetaData,
				new VariableDeclaration(MetaData, variableName, Expression, true),
				new ReturnStatement(MetaData, new VariableExpression(MetaData, variableName)));
		}

		public override IEnumerable<string> Dump() => new[] {"return statement:\n"}
			.Concat(Expression.Dump().Select(MapFunc));

		public override IEnumerable<ReturnStatement> FindReturnStatements() => new[] {this};
	}

	/// <summary>
	///   FEATURE #25
	/// </summary>
	public class JumpStatement : Statement
	{
		public enum Jump
		{
			Break,
			Continue
		}

		public readonly Jump JumpKind;

		public JumpStatement(
			MetaData metaData,
			Jump jumpKind) :
			base(metaData) => JumpKind = jumpKind;

		public override IEnumerable<JumpStatement> FindJumpStatements() => new[] {this};
	}

	public class AssignmentStatement : Statement
	{
		[NotNull] public readonly Expression LhsExpression;
		[NotNull] public readonly Expression RhsExpression;

		public AssignmentStatement(
			MetaData metaData,
			[NotNull] Expression lhsExpression,
			[NotNull] Expression rhsExpression) :
			base(metaData)
		{
			LhsExpression = lhsExpression;
			RhsExpression = rhsExpression;
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			LhsExpression.SurroundWith(Env);
			RhsExpression.SurroundWith(Env);
			var lhs = LhsExpression.GetExpressionType();
			var rhs = RhsExpression.GetExpressionType();
			// FEATURE #14
			// FEATURE #11
			if (!string.Equals(rhs.ToString(), PrimaryType.NullType, StringComparison.Ordinal) &&
			    !string.Equals(lhs.ToString(), rhs.ToString(), StringComparison.Ordinal))
				Errors.Add($"{MetaData.GetErrorHeader()}assigning a {rhs} to a {lhs} is invalid.");
			// FEATURE #20
			var validLhs = LhsExpression.GetLhsExpression();
			if (null == validLhs)
				Errors.Add($"{MetaData.GetErrorHeader()}a {lhs} cannot be assigned.");
			else if (null == validLhs.Declaration)
				Errors.Add($"{MetaData.GetErrorHeader()}can't find declaration of {validLhs.Name}");
			// FEATURE #21
			else if (!validLhs.Declaration.Mutability)
				Errors.Add($"{MetaData.GetErrorHeader()}cannot assign to an immutable variable.");
			else validLhs.Declaration.Used = true;
			if (!(RhsExpression is AtomicExpression))
			{
				ConvertedStatementList = new StatementList(MetaData,
					new VariableDeclaration(MetaData, ""));
			}
		}

		public override IEnumerable<string> Dump() => new[]
			{
				"assignment statemnt:\n",
				"  lhs:\n"
			}
			.Concat(LhsExpression.Dump().Select(MapFunc2))
			.Concat(new[] {"  rhs:\n"})
			.Concat(RhsExpression.Dump().Select(MapFunc2));
	}
}