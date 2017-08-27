using System;
using System.Collections.Generic;
using System.Linq;
using bCC.Core;
using bCC.Expression;
using JetBrains.Annotations;

namespace bCC.Statement
{
	public class IfStatement : WhileStatement
	{
		[NotNull] public StatementList ElseStatementList;
		public int Optimized;

		public IfStatement(
			MetaData metaData,
			[NotNull] Expression.Expression condition,
			[NotNull] StatementList ifStatementList,
			[CanBeNull] StatementList elseStatementList = null) : base(metaData, condition, ifStatementList) =>
			ElseStatementList = elseStatementList ?? new StatementList(MetaData);

		public override void SurroundWith(Environment environment)
		{
			// base.SurroundWith(environment);
			// don't call base, because it will raise error
			// as a while expression
			Env = environment;
			Condition.SurroundWith(Env);
			// FEATURE #1
			var conditionType = Condition.GetExpressionType().ToString();
			if (!string.Equals(conditionType, PrimaryType.BoolType, StringComparison.Ordinal))
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

		public override IEnumerable<ReturnStatement> FindReturnStatements() =>
			OkStatementList.FindReturnStatements().Concat(ElseStatementList.FindReturnStatements());

		public override IEnumerable<JumpStatement> FindJumpStatements() =>
			OkStatementList.FindJumpStatements().Concat(ElseStatementList.FindJumpStatements());

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