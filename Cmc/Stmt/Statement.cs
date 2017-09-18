using System.Collections.Generic;
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

		[NotNull]
		public virtual IEnumerable<ReturnStatement> FindReturnStatements() => new List<ReturnStatement>(0);

		[NotNull]
		public virtual IEnumerable<JumpStatement> FindJumpStatements() => new List<JumpStatement>(0);

		public override IEnumerable<string> Dump() => new[] {"empty statement"};
	}

	public class ExpressionStatement : Statement
	{
		[NotNull] public readonly Expression Expression;

		public ExpressionStatement(
			MetaData metaData,
			[NotNull] Expression expression) :
			base(metaData) =>
			Expression = expression;

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			Expression.SurroundWith(Env);
			var res = Expression.ConvertedResult;
			if (null != res)
				ConvertedStatementList = new StatementList(MetaData);
		}

		public override IEnumerable<string> Dump() => new[] {"expression statement:\n"}
			.Concat(Expression.Dump().Select(MapFunc));
	}

	public class ReturnStatement : ExpressionStatement
	{
		public LabelDeclaration Label;

		public ReturnStatement(
			MetaData metaData,
			[CanBeNull] Expression expression = null) :
			base(metaData, expression ?? new NullExpression(metaData))
		{
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			if (Expression is AtomicExpression) return;
			var variableName = $"{MetaData.TrimedFileName}{MetaData.LineNumber}{GetHashCode()}";
			ConvertedStatementList = new StatementList(MetaData,
				new ReturnStatement(MetaData, new VariableExpression(MetaData, variableName)));
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
		public LabelDeclaration Label;

		public JumpStatement(
			MetaData metaData,
			Jump jumpKind) :
			base(metaData) => JumpKind = jumpKind;

		public override IEnumerable<JumpStatement> FindJumpStatements() => new[] {this};
	}
}