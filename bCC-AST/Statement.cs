using System.Collections.Generic;
using System.Linq;

namespace bCC
{
	public class Statement : IAst
	{
		public virtual Environment Env { get; set; }

		public virtual IList<Declaration> GetDependencies() => new List<Declaration>();

		public Statement(MetaData metaData) : base(metaData)
		{
		}
	}

	public class ExpressionStatement : Statement
	{
		public override Environment Env
		{
			get => _env;
			set
			{
				_env = value;
				Expression.Env = Env;
			}
		}

		public readonly Expression Expression;
		private Environment _env;
		public ExpressionStatement(MetaData metaData, Expression expression) : base(metaData) => Expression = expression;
	}

	public class StatementList : Statement
	{
		public readonly IList<Statement> Statements;
		private Environment _env;

		public sealed override Environment Env
		{
			get => _env;
			set
			{
				_env = value;
				var env = new Environment(Env);
				foreach (var statement in Statements)
				{
					if (!(statement is Declaration declaration)) continue;
					statement.Env = env;
					env = new Environment(env);
					env.Declarations.Add(declaration);
				}
			}
		}

		public override IList<Declaration> GetDependencies() =>
			Statements.SelectMany(statement => statement.GetDependencies()).ToList();

		public StatementList(MetaData metaData, params Statement[] statements) : base(metaData)
		{
			Statements = statements;
		}
	}

	public class IfStatement : Statement
	{
		public readonly Expression Condition;
		public readonly StatementList IfStatementList;
		public readonly StatementList ElseStatementList;
		private Environment _env;

		public override Environment Env
		{
			get => _env;
			set
			{
				_env = value;
				IfStatementList.Env = new Environment(Env);
				if (ElseStatementList != null) ElseStatementList.Env = new Environment(Env);
			}
		}

		public IfStatement(
			MetaData metaData,
			Expression condition,
			StatementList ifStatementList,
			StatementList elseStatementList = null) : base(metaData)
		{
			Condition = condition;
			IfStatementList = ifStatementList;
			ElseStatementList = elseStatementList;
		}
	}
}