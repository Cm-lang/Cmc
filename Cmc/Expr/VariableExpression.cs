using System.Collections.Generic;
using System.Linq;
using Cmc.Core;
using Cmc.Decl;
using JetBrains.Annotations;

namespace Cmc.Expr
{
	public class VariableExpression : AtomicExpression
	{
		[NotNull] public readonly string Name;
		[CanBeNull] public Declaration Declaration;
		public bool DeclarationMutability;
		[CanBeNull] private Type _declarationType;

		public VariableExpression(
			MetaData metaData,
			[NotNull] string name) :
			base(metaData) => Name = name;

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			var declaration = Env.FindDeclarationByName(Name);
			switch (declaration)
			{
				case VariableDeclaration variableDeclaration:
					ChangeDeclaration(variableDeclaration);
					break;
				case ExternDeclaration externDeclaration:
					ChangeDeclaration(externDeclaration);
					break;
				default:
					Errors.AddAndThrow($"{MetaData.GetErrorHeader()}{declaration} isn't a variable");
					break;
			}
		}

		public void ChangeDeclaration(VariableDeclaration variableDeclaration)
		{
			_declarationType = variableDeclaration.Type;
			DeclarationMutability = variableDeclaration.Mutability;
			Declaration = variableDeclaration;
			Declaration.UsageCount++;
		}

		public void ChangeDeclaration(ExternDeclaration externDeclaration)
		{
			_declarationType = externDeclaration.Type;
			DeclarationMutability = externDeclaration.Mutability;
			Declaration = externDeclaration;
			Declaration.UsageCount++;
		}

		public override Type GetExpressionType() =>
			_declarationType ?? throw new CompilerException("unknown type");

		public override IEnumerable<string> Dump() => new[]
			{
				$"variable expression [{Name}]:\n",
				"  type:\n"
			}
			.Concat(_declarationType?.Dump().Select(MapFunc2) ?? new[] {"    cannot infer!\n"});

		public override VariableExpression GetLhsExpression() => this;
	}
}