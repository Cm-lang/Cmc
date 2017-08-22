using System;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable 659

namespace bCC_AST
{
	public class Declaration : IAst
	{
		public virtual IList<Declaration> FindDependencies() => new List<Declaration> {this};

		public readonly string Name;

		public Declaration(MetaData metaData, string name) : base(metaData)
		{
			Name = name;
		}
	}

	public class FunctionDeclaration : Declaration
	{
		public readonly StatementList Body;

		public override IList<Declaration> FindDependencies() =>
			Body.Statements.SelectMany(i => i.GetDependencies()).ToList();

		public FunctionDeclaration(MetaData metaData, string name, StatementList body) : base(metaData, name)
		{
			Body = body;
		}

		// TODO: add type check
		public override bool Equals(object obj) => obj is Declaration declaration && declaration.Name == Name;
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

		public override bool Equals(object obj) => obj is Declaration declaration && declaration.Name == Name;
	}

	public class Macro : Declaration
	{
		public override IList<Declaration> FindDependencies() => new List<Declaration>();

		public Macro(MetaData metaData, string name) : base(metaData, name)
		{
		}
	}
}