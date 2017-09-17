using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Cmc.Core;
using Cmc.Expr;
using JetBrains.Annotations;
using static System.StringComparison;
using Environment = Cmc.Core.Environment;

#pragma warning disable 659

namespace Cmc.Decl
{
	public sealed class VariableDeclaration : Declaration
	{
		[NotNull] public readonly Expression Expression;
		public readonly bool Mutability;
		public bool IsGlobal = false;
		[CanBeNull] public Type Type;

		public VariableDeclaration(
			MetaData metaData,
			[NotNull] string name,
			[CanBeNull] Expression expression = null,
			bool isMutable = false,
			[CanBeNull] Type type = null,
			Modifier[] modifiers = null) :
			base(metaData, name, modifiers ?? new[] {Modifier.Private})
		{
			Expression = expression ?? new NullExpression(MetaData);
			Type = type;
			Mutability = isMutable;
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			// https://github.com/Cm-lang/Cm-Document/issues/12
			if (string.Equals(Name, ReservedWords.Recur, Ordinal))
			{
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
			// FEATURE #11
			if (!string.Equals(exprType.ToString(), PrimaryType.NullType, Ordinal) &&
			    !Equals(Type, exprType))
				// FEATURE #9
				Errors.Add($"{MetaData.GetErrorHeader()}type mismatch, expected: {Type}, actual: {exprType}");
		}

		/// <summary>
		///  Conservatism inline
		/// </summary>
		/// <returns>if it should be inlined</returns>
		public bool ShouldBeInlinedImmediately() =>
			Mutability && (UsageCount <= 1 || Expression is AtomicExpression);

		public override bool Equals(object obj) =>
			obj is Declaration declaration && declaration.Name == Name;

		public override IEnumerable<string> Dump() => new[]
			{
				$"{(Mutability ? "mutable" : "immutable")} variable declaration [{Name}]:\n",
				"  type:\n"
			}
			.Concat(Type?.Dump().Select(MapFunc2) ?? new[] {"    cannot infer!\n"})
			.Concat(new[] {"  initialize expression:\n"})
			.Concat(Expression.Dump().Select(MapFunc2));
	}
}