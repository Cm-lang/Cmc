using System;
using System.Collections.Generic;

namespace bCC_AST
{
	public abstract class IAst
	{
		public MetaData MetaData;

		protected IAst(MetaData metaData)
		{
			MetaData = metaData;
		}
	}

	public class Statement : IAst
	{
		public Statement(MetaData metaData) : base(metaData)
		{
		}
	}

	public class StatementList : Statement
	{
		public readonly IList<Statement> Statements;

		public StatementList(MetaData metaData, params Statement[] statements) : base(metaData)
		{
			Statements = statements;
		}
	}

	public abstract class Declaration : IAst
	{
		public abstract IList<Declaration> FindDependencies();

		protected Declaration(MetaData metaData) : base(metaData)
		{
		}
	}

	public class FunctionDeclaration : Declaration
	{
		public override IList<Declaration> FindDependencies()
		{
			throw new NotImplementedException();
		}

		public FunctionDeclaration(MetaData metaData) : base(metaData)
		{
		}
	}

	public class VariableDeclaration : Declaration
	{
		public override IList<Declaration> FindDependencies()
		{
			throw new NotImplementedException();
		}

		public VariableDeclaration(MetaData metaData) : base(metaData)
		{
		}
	}

	public class Macro : Declaration
	{
		public override IList<Declaration> FindDependencies() => new List<Declaration>();

		public Macro(MetaData metaData) : base(metaData)
		{
		}
	}

	public class BcFile
	{
		public readonly IList<Declaration> Declarations;

		public BcFile(params Declaration[] declarations)
		{
			Declarations = declarations;
			foreach (var declaration in Declarations)
			{
				foreach (var dependency in declaration.FindDependencies())
				{
					if (!Declarations.Contains(dependency))
					{
						Console.WriteLine("Error: ");
					}
				}
			}
		}
	}
}