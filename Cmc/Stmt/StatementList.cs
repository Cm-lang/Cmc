using System.Collections.Generic;
using System.Linq;
using Cmc.Core;
using Cmc.Decl;
using JetBrains.Annotations;

namespace Cmc.Stmt
{
	public class StatementList : Statement
	{
		[NotNull] public List<Statement> Statements;
		[NotNull] public List<Statement> JumpOutStatements;

		public StatementList(
			MetaData metaData,
			params Statement[] statements) :
			base(metaData)
		{
			JumpOutStatements = new List<Statement>();
			Statements = statements.ToList();
		}

		public StatementList(
			MetaData metaData,
			IEnumerable<Statement> statements) :
			base(metaData)
		{
			JumpOutStatements = new List<Statement>();
			Statements = statements.ToList();
		}

		public void Flatten() => Statements =
			new List<Statement>(
				from i in Statements
				from j in i.ConvertedStatementList?.Statements ?? new[] {i}.ToList()
				select j);

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
					if (convertedResult != null && 0 != convertedResult.ConvertedStatements.Count)
					{
						converted.AddRange(convertedResult.ConvertedStatements);
						expression.Expression = convertedResult.ConvertedExpression;
						converted.Add(expression);
						expression.Expression.ConvertedResult = null;
						// expression might be a return statement
						// converted.Add(new ExpressionStatement(MetaData, convertedResult.ConvertedExpression));
					}
					else
						converted.Add(expression);
				}
				else
					converted.Add(statement);
			}
			ConvertedStatementList = new StatementList(MetaData, converted);
		}

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