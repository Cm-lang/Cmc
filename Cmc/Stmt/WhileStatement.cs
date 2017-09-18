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
	public class WhileStatement : Statement
	{
		[NotNull] public readonly Expression Condition;
		[NotNull] public StatementList OkStatementList;
		public int Optimized;

		public WhileStatement(
			MetaData metaData,
			[NotNull] Expression condition,
			[NotNull] StatementList okStatementList) : base(metaData)
		{
			Condition = condition;
			OkStatementList = okStatementList;
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			var jmp = new JumpLabelDeclaration(MetaData, "");
			jmp.SurroundWith(Env);
			Condition.SurroundWith(Env);
			// FEATURE #16
			var conditionType = Condition.GetExpressionType().ToString();
			if (!string.Equals(conditionType, PrimaryType.BoolType, StringComparison.Ordinal))
				Errors.Add(
					$"{MetaData.GetErrorHeader()}expected a bool as the \"while\" statement\'s condition, " +
					$"found {conditionType}");
			OkStatementList.Statements.Add(jmp);
			OkStatementList.SurroundWith(new Environment(Env));
			// FEATURE #17
			if (Pragma.KeepAll || !(Condition is BoolLiteralExpression boolean) || boolean.Value) return;
			OptimizedStatementList = new StatementList(MetaData);
			Optimized = 1;
		}

		public override IEnumerable<JumpStatement> FindJumpStatements() =>
			OkStatementList.FindJumpStatements();

		public override IEnumerable<string> Dump() => new[]
			{
				"while statement:\n",
				"  condition:\n"
			}
			.Concat(Condition.Dump().Select(MapFunc2))
			.Concat(new[] {"  body:\n"})
			.Concat(Optimized == 1
				? new[] {"   [optimized]"}
				: OkStatementList.Dump().Select(MapFunc2));
	}
}