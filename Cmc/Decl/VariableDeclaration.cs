using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cmc.Core;
using Cmc.Expr;
using JetBrains.Annotations;
using Environment = Cmc.Core.Environment;

namespace Cmc.Decl
{
	public sealed class VariableDeclaration : Declaration
	{
		[NotNull] public readonly Expression Expression;
		public readonly bool Mutability;
		public ulong Address;
		public int Align = 8;
		public bool IsGlobal = false;
		[CanBeNull] public Type Type;

		public VariableDeclaration(
			MetaData metaData,
			[NotNull] string name,
			[CanBeNull] Expression expression = null,
			bool isMutable = false,
			[CanBeNull] Type type = null,
			Modifier modifier = Modifier.Private) :
			base(metaData, name, modifier)
		{
			Expression = expression ?? new NullExpression(MetaData);
			Type = type;
			Mutability = isMutable;
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			// https://github.com/Cm-lang/Cm-Document/issues/12
			if (string.Equals(Name, "recur", StringComparison.Ordinal))
			{
				Debug.Assert(null != Expression);
				Debug.Assert(null != Expression.Env);
				Type = Expression.GetExpressionType();
				return;
			}
			Expression.SurroundWith(Env);
			var exprType = Expression.GetExpressionType();
			// FEATURE #8
			Type = Type ?? exprType;
			// FEATURE #30
			Type.SurroundWith(Env);
			if (Type is UnknownType unknownType) Type = unknownType.Resolve();
			if (Type is PrimaryType primaryType) Align = primaryType.Align;
			// FEATURE #11
			if (!string.Equals(exprType.ToString(), PrimaryType.NullType, StringComparison.Ordinal) &&
			    !Equals(Type, exprType))
				// FEATURE #9
				Errors.Add($"{MetaData.GetErrorHeader()}type mismatch, expected: {Type}, actual: {exprType}");
		}

		public override bool Equals(object obj) =>
			obj is Declaration declaration && declaration.Name == Name;

		public override IEnumerable<string> Dump() => new[]
			{
				$"variable declaration [{Name}]:\n",
				"  type:\n"
			}
			.Concat(Type?.Dump().Select(MapFunc2) ?? new[] {"    cannot infer!\n"})
			.Concat(new[] {"  initialize expression:\n"})
			.Concat(Expression.Dump().Select(MapFunc2));

		public string LlvmNameGen() => IsGlobal ? $"@glob{Address}" : $"%var{Address}";
	}
}