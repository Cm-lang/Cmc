﻿using System;
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
		[NotNull] public readonly IList<Statement> Statements;
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

		public override IEnumerable<string> Dump() =>
			new[] {"statement list:\n"}
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

	public class IfStatement : Statement
	{
		[NotNull] public readonly Expression Condition;
		[NotNull] public readonly StatementList IfStatementList;
		[CanBeNull] public readonly StatementList ElseStatementList;
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
				IfStatementList.Env = new Environment(Env);
				if (ElseStatementList != null) ElseStatementList.Env = new Environment(Env);
			}
		}

		public IfStatement(
			MetaData metaData,
			[NotNull] Expression condition,
			[NotNull] StatementList ifStatementList,
			[CanBeNull] StatementList elseStatementList = null) : base(metaData)
		{
			Condition = condition;
			IfStatementList = ifStatementList;
			// FEATURE #2
			ElseStatementList = elseStatementList;
		}
	}
}