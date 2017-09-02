using System.Collections.Generic;
using System.Linq;
using Cmc.Core;
using Cmc.Stmt;
using JetBrains.Annotations;

namespace Cmc.Expr
{
	/// <summary>
	///     A function is a variable with the type of lambda
	///     This is the class for anonymous lambda
	/// </summary>
	public class LambdaExpression : AtomicExpression
	{
		[CanBeNull] private readonly Type _declaredType;
		[NotNull] public readonly StatementList Body;
		[NotNull] public readonly IList<VariableDeclaration> ParameterList;
		private Type _type;

		public LambdaExpression(
			MetaData metaData,
			[NotNull] StatementList body,
			// FEATURE #22
			[CanBeNull] IList<VariableDeclaration> parameterList = null,
			[CanBeNull] Type returnType = null) : base(metaData)
		{
			Body = body;
			_declaredType = returnType;
			ParameterList = parameterList ?? new List<VariableDeclaration>();
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			foreach (var variableDeclaration in ParameterList)
				variableDeclaration.SurroundWith(Env);
			var bodyEnv = new Environment(Env);
			foreach (var variableDeclaration in ParameterList)
				bodyEnv.Declarations.Add(variableDeclaration);
			Body.SurroundWith(bodyEnv);
			var retTypes = Body.FindReturnStatements().Select(i =>
			{
				i.WhereToJump = this;
				return i.Expression.GetExpressionType();
			}).ToList();
			// FEATURE #24
			if (retTypes.Any(i => !Equals(i, _declaredType ?? retTypes.First())))
				Errors.Add(
					$"{MetaData.GetErrorHeader()}ambiguous return types:\n" +
					(_declaredType != null ? $"<{_declaredType}>" : "") +
					$"[{string.Join(",", retTypes.Select(i => i.ToString()))}]");
			// FEATURE #12
			var retType = _declaredType ?? (retTypes.Count != 0
				              ? retTypes.First()
				              // FEATURE #19
				              : new PrimaryType(MetaData, PrimaryType.NullType));
			_type = new LambdaType(MetaData, ParameterList.Select(i => i.Type).ToList(), retType);
		}

		public override Type GetExpressionType()
		{
			return _type;
		}

		public override IEnumerable<string> Dump()
		{
			return new[]
				{
					"lambda:\n",
					"  type:\n"
				}
				.Concat(GetExpressionType().Dump().Select(MapFunc2))
				.Concat(new[] {"  parameters:\n"})
				.Concat(ParameterList.SelectMany(i => i.Dump().Select(MapFunc2)))
				.Concat(new[] {"  body:\n"})
				.Concat(Body.Dump().Select(MapFunc2));
		}
	}
}