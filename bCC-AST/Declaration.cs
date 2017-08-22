using System;
using System.Collections.Generic;

namespace bCC_AST
{
	public abstract class Declaration : IAst
	{
		public abstract IList<Declaration> FindDependencies();
		public readonly string Name;

		protected Declaration(MetaData metaData, string name) : base(metaData)
		{
			Name = name;
		}
	}

	public class FunctionDeclaration : Declaration
	{
		public readonly StatementList Body;

		public override IList<Declaration> FindDependencies()
		{
			throw new NotImplementedException();
		}

		public FunctionDeclaration(MetaData metaData, string name, StatementList body) : base(metaData, name)
		{
			Body = body;
		}
	}

	public class VariableDeclaration : Declaration
	{
		public readonly bool Mutability;
		public readonly Expression Expression;

		public override IList<Declaration> FindDependencies()
		{
			throw new NotImplementedException();
		}

		public VariableDeclaration(MetaData metaData, string name, Expression expression, bool isMutable = false) :
			base(metaData, name)
		{
			Expression = expression;
			Mutability = isMutable;
		}
	}

	public class Macro : Declaration
	{
		public override IList<Declaration> FindDependencies() => new List<Declaration>();

		public Macro(MetaData metaData, string name) : base(metaData, name)
		{
		}
	}
}