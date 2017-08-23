using System.Collections.Generic;

#pragma warning disable 659

namespace bCC
{
	public class Declaration : Statement
	{
		public virtual IList<Declaration> FindDependencies() => new List<Declaration> {this};

		public readonly string Name;

		public Declaration(MetaData metaData, string name) : base(metaData)
		{
			Name = name;
		}
	}

	public sealed class VariableDeclaration : Declaration
	{
		public readonly bool Mutability;
		public readonly Expression Expression;
		public readonly Type Type;

		public override IList<Declaration> FindDependencies() => Expression.GetDependencies();

		public VariableDeclaration(
			MetaData metaData,
			string name,
			Expression expression,
			bool isMutable = false,
			Type type = null) :
			base(metaData, name)
		{
			Expression = expression;
			Expression.Env = Env;
			var exprType = expression.GetExpressionType();
			// FEATURE #8
			Type = type ?? exprType;
			if (Type != exprType)
				// FEATURE #9
				Errors.Add(metaData.GetErrorHeader() + "type mismatch, expected: " + Type.Name + ", actual: " + exprType);
			Mutability = isMutable;
		}

		public override bool Equals(object obj) => obj is Declaration declaration && declaration.Name == Name;
	}

	public class Macro : Declaration
	{
		public override IList<Declaration> FindDependencies() => new List<Declaration>();

		public string Content;

		public Macro(MetaData metaData, string name, string content) : base(metaData, name)
		{
			Content = content;
		}
	}
}