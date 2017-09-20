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

		[CanBeNull]
		public Declaration Declaration
		{
			get => _declaration;
			set
			{
				switch (value)
				{
					case VariableDeclaration variable:
						ChangeDeclaration(variable);
						break;
					case ExternDeclaration @extern:
						ChangeDeclaration(@extern);
						break;
					default:
						Errors.AddAndThrow($"{MetaData.GetErrorHeader()}{value} isn't a variable");
						throw new CompilerException("gg");
				}
			}
		}

		public bool DeclarationMutability;
		[CanBeNull] private Type _declarationType;
		[CanBeNull] private Declaration _declaration;

		public VariableExpression(
			MetaData metaData,
			[NotNull] string name) :
			base(metaData) => Name = name;

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			var declaration = Env.FindDeclarationByName(Name);
			Declaration = declaration;
		}

		private void ChangeDeclaration(VariableDeclaration variableDeclaration)
		{
			_declarationType = variableDeclaration.Type;
			DeclarationMutability = variableDeclaration.Mutability;
			_declaration = variableDeclaration;
			_declaration.UsageCount++;
		}

		private void ChangeDeclaration(ExternDeclaration externDeclaration)
		{
			_declarationType = externDeclaration.Type;
			DeclarationMutability = externDeclaration.Mutability;
			_declaration = externDeclaration;
			_declaration.UsageCount++;
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