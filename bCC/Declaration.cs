using System.Collections.Generic;
using JetBrains.Annotations;
using static System.StringComparison;

#pragma warning disable 659

namespace bCC
{
	public class Declaration : Statement
	{
		[NotNull]
		public virtual IList<Declaration> FindDependencies() => new List<Declaration> {this};

		[NotNull] public readonly string Name;

		public Declaration(MetaData metaData, [NotNull] string name) : base(metaData)
		{
			Name = name;
		}
	}

	public sealed class VariableDeclaration : Declaration
	{
		public readonly bool Mutability;
		[NotNull] public readonly Expression Expression;
		[NotNull] public readonly Type Type;

		public override IList<Declaration> FindDependencies() => Expression.GetDependencies();

		public VariableDeclaration(
			MetaData metaData,
			[NotNull] string name,
			[CanBeNull] Expression expression = null,
			bool isMutable = false,
			[CanBeNull] Type type = null) :
			base(metaData, name)
		{
			Expression = expression ?? new NullExpression(MetaData);
			Expression.Env = Env;
			var exprType = Expression.GetExpressionType();
			// FEATURE #8
			Type = type ?? exprType;
			// FEATURE #11
			if (!string.Equals(Type.Name, NullExpression.NullType, Ordinal) && Type != exprType)
				// FEATURE #9
				Errors.Add(metaData.GetErrorHeader() + "type mismatch, expected: " + Type.Name + ", actual: " + exprType);
			Mutability = isMutable;
		}

		public override bool Equals(object obj) => obj is Declaration declaration && declaration.Name == Name;
	}

	public class Macro : Declaration
	{
		public override IList<Declaration> FindDependencies() => new List<Declaration>();

		[NotNull] public string Content;

		public Macro(MetaData metaData, [NotNull] string name, [NotNull] string content) : base(metaData, name)
		{
			Content = content;
		}
	}
}