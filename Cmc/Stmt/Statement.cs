﻿using System.Collections.Generic;
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

		public override IEnumerable<string> Dump() => new[] {"empty statement"};
		public override IEnumerable<string> DumpCode() => new[] {";\n"};
	}

	public class ExpressionStatement : Statement
	{
		[NotNull] public Expression Expression;

		public ExpressionStatement(
			MetaData metaData,
			[NotNull] Expression expression) :
			base(metaData)
		{
			Expression = expression;
		}

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

		public override void ConvertGoto() => Expression.ConvertGoto();
		public override IEnumerable<string> DumpCode() => new[] {$"{string.Join("", Expression.DumpCode())};\n"};

		public override IEnumerable<string> Dump() => new[] {"expression statement:\n"}
			.Concat(Expression.Dump().Select(MapFunc));
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
		public JumpLabelDeclaration JumpLabel;
		[CanBeNull] private readonly string _labelName;

		public JumpStatement(
			MetaData metaData,
			Jump jumpKind,
			[CanBeNull] string labelName = null) :
			base(metaData)
		{
			JumpKind = jumpKind;
			_labelName = labelName;
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			var jumpLabel = Env.FindJumpLabelByName(_labelName ?? "");
			if (null == jumpLabel)
				Errors.AddAndThrow($"{MetaData.GetErrorHeader()}cannot return outside a lambda");
			else
			{
				JumpLabel = jumpLabel;
				JumpLabel.StatementsUsingThis.Add(this);
			}
		}

		public override string ToString() => JumpKind == Jump.Break ? "break" : "continue";
		public override IEnumerable<string> Dump() => new[] {$"jump statement [{this}] [{JumpLabel}]\n"};
		public override IEnumerable<string> DumpCode() => new[] {$"{this}:{JumpLabel};\n"};
	}

	public class GotoStatement : Statement
	{
		[NotNull] public readonly string Label;

		public GotoStatement(
			MetaData metaData,
			[NotNull] string label) : base(metaData) => Label = label;

		public override IEnumerable<string> Dump() => new[] {$"goto statement [{Label}]:\n"};
		public override IEnumerable<string> DumpCode() => new[] {$"goto {Label};\n"};
	}
}