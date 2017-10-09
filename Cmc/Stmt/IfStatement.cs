using System;
using System.Collections.Generic;
using System.Linq;
using Cmc.Core;
using Cmc.Expr;
using JetBrains.Annotations;
using Environment = Cmc.Core.Environment;

namespace Cmc.Stmt
{
	public class IfStatement : ConditionalStatement
	{
		[NotNull] public StatementList IfStatementList;
		[NotNull] public StatementList ElseStatementList;
		public int Optimized;

		public IfStatement(
			MetaData metaData,
			[NotNull] Expression condition,
			[NotNull] StatementList ifStatementList,
			[CanBeNull] StatementList elseStatementList = null) :
			base(metaData, condition)
		{
			IfStatementList = ifStatementList;
			ElseStatementList = elseStatementList ?? new StatementList(MetaData);
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			// FEATURE #1
			var conditionType = Condition.GetExpressionType().ToString();
			if (!string.Equals(conditionType, PrimaryType.BoolType, StringComparison.Ordinal))
				Errors.Add(
					$"{MetaData.GetErrorHeader()}expected a bool as the \"if\" statement\'s condition, found {conditionType}");
			IfStatementList.SurroundWith(new Environment(Env));
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
				IfStatementList.Statements = new List<Statement>(0);
				Optimized = 2;
			}
		}

		public override IEnumerable<string> Dump() => new[]
			{
				"if statement:\n",
				"  condition:\n"
			}
			.Concat(Condition.Dump().Select(MapFunc2))
			.Concat(new[] {$"  true branch{(Optimized == 2 ? " [optimized]" : "")}:\n"})
			.Concat(IfStatementList.Dump().Select(MapFunc2))
			.Concat(new[] {$"  false branch{(Optimized == 1 ? " [optimized]" : "")}:\n"})
			.Concat(ElseStatementList.Dump().Select(MapFunc2));

		public override IEnumerable<string> DumpCode() =>
			new[] {$"if ({string.Join("", Condition.DumpCode())}) {{\n"}
				.Concat(IfStatementList.DumpCode().Select(MapFunc))
				.Append("} else {\n")
				.Concat(ElseStatementList.DumpCode().Select(MapFunc))
				.Append("}\n");

		public override Type GetExpressionType() => new PrimaryType(MetaData, "void");
	}
}