using System.Collections.Generic;

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

	public class VariableDeclaration : Declaration
	{
		public readonly bool Mutability;
		public readonly Expression Expression;
		public readonly Type Type;

		public override IList<Declaration> FindDependencies() => Expression.GetDependencies();

		public VariableDeclaration(MetaData metaData, string name, Expression expression, bool isMutable = false,
			Type type = null) :
			base(metaData, name)
		{
			Expression = expression;
			var exprType = expression.GetExpressionType();
			Type = type ?? exprType;
			if (Type != exprType)
				Errors.Add(metaData.GetErrorHeader() + "type mismatch, expected: " + Type.Name + ", actual: " + exprType);
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