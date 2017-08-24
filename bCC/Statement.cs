using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using static System.StringComparison;
using static bCC.PrimaryType;

namespace bCC
{
	public class Statement : Ast
	{
		public Statement(MetaData metaData) : base(metaData)
		{
		}

		public override IEnumerable<string> Dump() => new[] {"empty statement"};
	}

	public class ExpressionStatement : Statement
	{
		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			Expression.SurroundWith(Env);
		}

		[NotNull] public readonly Expression Expression;
		public ExpressionStatement(MetaData metaData, Expression expression) : base(metaData) => Expression = expression;

		public override IEnumerable<string> Dump() =>
			new[] {"expression statement:\n"}
				.Concat(Expression.Dump().Select(MapFunc));
	}

	public class ReturnStatement : ExpressionStatement
	{
		public ReturnStatement(MetaData metaData, Expression expression) : base(metaData, expression)
		{
		}

		public override IEnumerable<string> Dump() =>
			new[] {"return statement:\n"}
				.Concat(Expression.Dump().Select(MapFunc));
	}

	public class StatementList : Statement
	{
		[NotNull] public IList<Statement> Statements;

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

		public StatementList(MetaData metaData, params Statement[] statements) : base(metaData) => Statements = statements;

		public override IEnumerable<string> Dump() => Statements.Count == 0
			? new[] {"empty statement list\n"}
			: new[] {"statement list:\n"}
				.Concat(Statements.SelectMany(i => i.Dump().Select(MapFunc)));
	}

	public class AssignmentStatement : Statement
	{
		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			LhsExpression.SurroundWith(Env);
			RhsExpression.SurroundWith(Env);
			var lhs = LhsExpression.GetExpressionType();
			var rhs = RhsExpression.GetExpressionType();
			// FEATURE #14
			// FEATURE #11
			if (!string.Equals(rhs.Name, NullType, Ordinal) &&
			    !string.Equals(lhs.Name, rhs.Name, Ordinal))
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

		[NotNull] public readonly Expression LhsExpression;
		[NotNull] public readonly Expression RhsExpression;

		public AssignmentStatement(
			MetaData metaData,
			[NotNull] Expression lhsExpression,
			[NotNull] Expression rhsExpression)
			: base(metaData)
		{
			LhsExpression = lhsExpression;
			RhsExpression = rhsExpression;
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

	public class WhileStatement : Statement
	{
		[NotNull] public readonly Expression Condition;
		[NotNull] public StatementList OkStatementList;

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			Condition.SurroundWith(Env);
			// FEATURE #16
			var conditionType = Condition.GetExpressionType().Name;
			if (!string.Equals(conditionType, "bool", Ordinal))
				Errors.Add(
					$"{MetaData.GetErrorHeader()}expected a bool as the \"while\" statement\'s condition, found {conditionType}");
			OkStatementList.SurroundWith(new Environment(Env));
		}

		public WhileStatement(
			MetaData metaData,
			[NotNull] Expression condition,
			[NotNull] StatementList okStatementList) : base(metaData)
		{
			Condition = condition;
			OkStatementList = okStatementList;
		}

		public override IEnumerable<string> Dump() => new[]
			{
				"while statement:\n",
				"  condition:\n"
			}
			.Concat(Condition.Dump().Select(MapFunc2))
			.Concat(new[] {"  body:\n"})
			.Concat(OkStatementList.Dump().Select(MapFunc2));
	}

	public class IfStatement : WhileStatement
	{
		[NotNull] public StatementList ElseStatementList;
		public int Optimized;

		public override void SurroundWith(Environment environment)
		{
			// base.SurroundWith(environment);
			// don't call base, because it will raise error
			// as a while expression
			Env = environment;
			Condition.SurroundWith(Env);
			// FEATURE #1
			var conditionType = Condition.GetExpressionType().Name;
			if (!string.Equals(conditionType, "bool", Ordinal))
				Errors.Add(
					$"{MetaData.GetErrorHeader()}expected a bool as the \"if\" statement\'s condition, found {conditionType}");
			OkStatementList.SurroundWith(new Environment(Env));
			ElseStatementList.SurroundWith(new Environment(Env));
			// FEATURE #17
			if (!(Condition is BoolLiteralExpression boolean)) return;
			if (boolean.Value)
			{
				ElseStatementList.Statements = new List<Statement>();
				Optimized = 1;
			}
			else
			{
				OkStatementList.Statements = new List<Statement>();
				Optimized = 2;
			}
		}

		public IfStatement(
			MetaData metaData,
			[NotNull] Expression condition,
			[NotNull] StatementList ifStatementList,
			[CanBeNull] StatementList elseStatementList = null) : base(metaData, condition, ifStatementList)
		{
			// FEATURE #2
			ElseStatementList = elseStatementList ?? new StatementList(MetaData);
		}

		public override IEnumerable<string> Dump() => new[]
			{
				"if statement:\n",
				"  condition:\n"
			}
			.Concat(Condition.Dump().Select(MapFunc2))
			.Concat(new[] {$"  true branch{(Optimized == 2 ? " [optimized]" : "")}:\n"})
			.Concat(OkStatementList.Dump().Select(MapFunc2))
			.Concat(new[] {$"  false branch{(Optimized == 1 ? " [optimized]" : "")}:\n"})
			.Concat(ElseStatementList.Dump().Select(MapFunc2));
	}
}