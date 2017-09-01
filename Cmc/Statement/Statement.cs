using System;
using System.Collections.Generic;
using System.Linq;
using Cmc.Core;
using Cmc.Expression;
using JetBrains.Annotations;
using Environment = Cmc.Core.Environment;

namespace Cmc.Statement
{
	public class Statement : Ast
	{
		public Statement(MetaData metaData) : base(metaData)
		{
		}

		public virtual IEnumerable<ReturnStatement> FindReturnStatements() => new List<ReturnStatement>();
		public virtual IEnumerable<JumpStatement> FindJumpStatements() => new List<JumpStatement>();

		public override IEnumerable<string> Dump() => new[] {"empty statement"};
	}

	public class ExpressionStatement : Statement
	{
		[NotNull] public readonly Expression.Expression Expression;

		public ExpressionStatement(
			MetaData metaData,
			[NotNull] Expression.Expression expression) :
			base(metaData) =>
			Expression = expression;

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			Expression.SurroundWith(Env);
		}

		public override IEnumerable<string> Dump() => new[] {"expression statement:\n"}
			.Concat(Expression.Dump().Select(MapFunc));
	}

	public class ReturnStatement : ExpressionStatement
	{
		public LambdaExpression WhereToJump;

		public ReturnStatement(
			MetaData metaData,
			[CanBeNull] Expression.Expression expression = null) :
			base(metaData, expression ?? new NullExpression(metaData))
		{
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

	public class StatementList : Statement
	{
		[NotNull] public IList<Statement> Statements;

		public StatementList(
			MetaData metaData,
			params Statement[] statements) :
			base(metaData) => Statements = statements;

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			var env = new Environment(Env);
			// FEATURE #4
			foreach (var statement in Statements)
				if (!(statement is Declaration declaration))
					statement.SurroundWith(env);
				else
				{
					statement.SurroundWith(env);
					env = new Environment(env);
					env.Declarations.Add(declaration);
				}
		}

		public override IEnumerable<ReturnStatement> FindReturnStatements() =>
			Statements.SelectMany(i => i.FindReturnStatements());

		public override IEnumerable<JumpStatement> FindJumpStatements() =>
			Statements.SelectMany(i => i.FindJumpStatements());

		public override IEnumerable<string> Dump() => Statements.Count == 0
			? new[] {"empty statement list\n"}
			: new[] {"statement list:\n"}
				.Concat(Statements.SelectMany(i => i.Dump().Select(MapFunc)));
	}

	public class AssignmentStatement : Statement
	{
		[NotNull] public readonly Expression.Expression LhsExpression;
		[NotNull] public readonly Expression.Expression RhsExpression;

		public AssignmentStatement(
			MetaData metaData,
			[NotNull] Expression.Expression lhsExpression,
			[NotNull] Expression.Expression rhsExpression) :
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
			// DO something with validLhs
			// FEATURE #21
			else if (!validLhs.Declaration.Mutability)
				Errors.Add($"{MetaData.GetErrorHeader()}cannot assign to an immutable variable.");
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