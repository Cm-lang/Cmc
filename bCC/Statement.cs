using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using static System.StringComparison;

namespace bCC
{
	public class Statement : Ast
	{
		public Statement(MetaData metaData) : base(metaData)
		{
		}
	}

	public class ExpressionStatement : Statement
	{
		public override Environment Env
		{
			[CanBeNull] get => _env;
			[NotNull]
			set
			{
				_env = value;
				Expression.Env = Env;
			}
		}

		[NotNull] public readonly Expression Expression;
		private Environment _env;
		public ExpressionStatement(MetaData metaData, Expression expression) : base(metaData) => Expression = expression;

		public override IEnumerable<string> Dump() =>
			new[] {"expression statement:\n"}
				.Concat(Expression.Dump().Select(i => "  " + i));
	}

	public class ReturnStatement : ExpressionStatement
	{
		public ReturnStatement(MetaData metaData, Expression expression) : base(metaData, expression)
		{
		}

		public override IEnumerable<string> Dump() =>
			new[] {"return statement:\n"}
				.Concat(Expression.Dump().Select(i => "  " + i));
	}

	public class StatementList : Statement
	{
		[NotNull] public IList<Statement> Statements;
		private Environment _env;

		public sealed override Environment Env
		{
			get => _env;
			[NotNull]
			set
			{
				_env = value;
				var env = new Environment(Env);
				// FEATURE #4
				foreach (var statement in Statements)
					if (!(statement is Declaration declaration))
						statement.Env = env;
					else
					{
						statement.Env = env;
						env = new Environment(env);
						env.Declarations.Add(declaration);
					}
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
		public override Environment Env
		{
			get => _env;
			set
			{
				_env = value;
				LhsExpression.Env = Env;
				RhsExpression.Env = Env;
				var lhs = LhsExpression.GetExpressionType();
				var rhs = RhsExpression.GetExpressionType();
				// FEATURE #14
				if (!string.Equals(lhs.Name, rhs.Name, Ordinal))
					Errors.Add(MetaData.GetErrorHeader() + "assigning a " + rhs + " to a " + lhs + " is invalid.");
				// TODO check for mutability
			}
		}

		[NotNull] public readonly Expression LhsExpression;
		[NotNull] public readonly Expression RhsExpression;
		private Environment _env;

		public AssignmentStatement(MetaData metaData, [NotNull] Expression lhsExpression, [NotNull] Expression rhsExpression)
			: base(metaData)
		{
			LhsExpression = lhsExpression;
			RhsExpression = rhsExpression;
		}
	}

	public class WhileStatement : Statement
	{
		[NotNull] public readonly Expression Condition;
		[NotNull] public StatementList OkStatementList;
		private Environment _env;

		public override Environment Env
		{
			get => _env;
			[NotNull]
			set
			{
				_env = value;
				Condition.Env = Env;
				// FEATURE #16
				var conditionType = Condition.GetExpressionType().Name;
				if (!string.Equals(conditionType, "bool", Ordinal))
					Errors.Add(MetaData.GetErrorHeader() + "expected a bool as the \"while\" statement's condition, found " +
					           conditionType);
				OkStatementList.Env = new Environment(Env);
			}
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
			.Concat(Condition.Dump().Select(MapFunc).Select(MapFunc))
			.Concat(new[] {"  body:\n"})
			.Concat(OkStatementList.Dump().Select(MapFunc).Select(MapFunc));
	}

	public class IfStatement : WhileStatement
	{
		[NotNull] public StatementList ElseStatementList;
		private Environment _env;

		public override Environment Env
		{
			get => _env;
			[NotNull]
			set
			{
				_env = value;
				Condition.Env = Env;
				// FEATURE #1
				var conditionType = Condition.GetExpressionType().Name;
				if (!string.Equals(conditionType, "bool", Ordinal))
					Errors.Add(MetaData.GetErrorHeader() + "expected a bool as the \"if\" statement's condition, found " +
					           conditionType);
				OkStatementList.Env = new Environment(Env);
				ElseStatementList.Env = new Environment(Env);
				// FEATURE #17
				if (Condition is BoolLiteralExpression boolean)
					if (boolean.Value) ElseStatementList.Statements = new List<Statement>();
					else OkStatementList.Statements = new List<Statement>();
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
			.Concat(Condition.Dump().Select(MapFunc).Select(MapFunc))
			.Concat(new[] {"  true branch:\n"})
			.Concat(OkStatementList.Dump().Select(MapFunc).Select(MapFunc))
			.Concat(new[] {"  false branch:\n"})
			.Concat(ElseStatementList.Dump().Select(MapFunc).Select(MapFunc));
	}
}