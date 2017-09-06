using System.Collections.Generic;
using System.Linq;
using Cmc.Core;
using Cmc.Decl;
using JetBrains.Annotations;

namespace Cmc.Stmt
{
	public class StatementList : Statement
	{
		[NotNull] public IList<Statement> Statements;

		public StatementList(
			MetaData metaData,
			params Statement[] statements) :
			base(metaData) => Statements = statements;

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			var env = new Environment(Env);
			var converted = new List<Statement>(Statements.Count + 5);
			// FEATURE #4
			foreach (var statement in Statements)
			{
				if (!(statement is Declaration declaration))
					statement.SurroundWith(env);
				else
				{
					statement.SurroundWith(env);
					env = new Environment(env);
					env.Declarations.Add(declaration);
				}
				if (statement is ExpressionStatement expression)
				{
					var convertedResult = expression.Expression.ConvertedResult;
					// ReSharper disable once InvertIf
					if (convertedResult != null)
					{
						converted.AddRange(convertedResult.ConvertedStatements);
						converted.Add(new ExpressionStatement(MetaData, convertedResult.ConvertedExpression));
					}
				}
				else
					converted.Add(statement);
			}
			ConvertedStatementList = new StatementList(MetaData, converted.ToArray());
		}

		public override IEnumerable<ReturnStatement> FindReturnStatements() =>
			from i in Statements
			from j in i.FindReturnStatements()
			select j;

		public override IEnumerable<JumpStatement> FindJumpStatements() =>
			from i in Statements
			from j in i.FindJumpStatements()
			select j;

		public override IEnumerable<string> Dump() => Statements.Count == 0
			? new[] {"empty statement list\n"}
			: new[] {"statement list:\n"}
				.Concat(
					from i in Statements
					from j in i.Dump().Select(MapFunc)
					select j);
	}
}