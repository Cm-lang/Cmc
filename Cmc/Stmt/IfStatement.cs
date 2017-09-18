using System;
using System.Collections.Generic;
using System.Linq;
using Cmc.Core;
using Cmc.Expr;
using JetBrains.Annotations;
using Environment = Cmc.Core.Environment;

namespace Cmc.Stmt
{
	public class IfStatement : WhileStatement
	{
		[NotNull] public StatementList ElseStatementList;

		public IfStatement(
			MetaData metaData,
			[NotNull] Expression condition,
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
			if (Pragma.KeepAll || !(Condition is BoolLiteralExpression boolean)) return;
			if (boolean.Value)
			{
				ElseStatementList.Statements = new List<Statement>(0);
				Optimized = 1;
			}
			else
			{
				OkStatementList.Statements = new List<Statement>(0);
				Optimized = 2;
			}
		}

		public override IEnumerable<JumpStatement> FindJumpStatements() =>
			OkStatementList.FindJumpStatements()
				.Concat(ElseStatementList.FindJumpStatements());

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