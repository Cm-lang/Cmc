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
	public abstract class ConditionalExpression : Expression
	{
		[NotNull] public readonly Expression Condition;

		protected ConditionalExpression(
			MetaData metaData,
			[NotNull] Expression condition) : base(metaData)
		{
			Condition = condition;
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			Condition.SurroundWith(Env);
		}
	}

	public class WhileExpression : ConditionalExpression
	{
		[NotNull] public StatementList OkStatementList;
		public int Optimized;
		[NotNull] public readonly JumpLabelDeclaration EndLabel;

		public WhileExpression(
			MetaData metaData,
			[NotNull] Expression condition,
			[NotNull] StatementList okStatementList,
			[CanBeNull] JumpLabelDeclaration endLabel = null) :
			base(metaData, condition)
		{
			OkStatementList = okStatementList;
			EndLabel = endLabel ?? new JumpLabelDeclaration(MetaData, "");
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			var jmp = new JumpLabelDeclaration(MetaData, "");
			jmp.SurroundWith(Env);
			EndLabel.SurroundWith(Env);
			// FEATURE #16
			var conditionType = Condition.GetExpressionType().ToString();
			if (!string.Equals(conditionType, PrimaryType.BoolType, StringComparison.Ordinal))
				Errors.Add(
					$"{MetaData.GetErrorHeader()}expected a bool as the \"while\" statement\'s condition, " +
					$"found {conditionType}");
			OkStatementList.Statements.Add(jmp);
			var bodyEnv = new Environment(Env);
			bodyEnv.Declarations.Add(EndLabel);
			OkStatementList.SurroundWith(bodyEnv);
			// FEATURE #17
			if (Pragma.KeepAll || !(Condition is BoolLiteralExpression boolean) || boolean.Value) return;
			OptimizedStatementList = new StatementList(MetaData);
			Optimized = 1;
		}

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

		public override IEnumerable<string> DumpCode() =>
			new[] {$"while ({string.Join("", Condition.DumpCode())}) {{\n"}
				.Concat(OkStatementList.DumpCode().Select(MapFunc))
				.Append("}");

		public override Type GetExpressionType() => new PrimaryType(MetaData, "void");
	}
}