using System.Collections.Generic;
using System.Linq;
using Cmc.Core;
using Cmc.Decl;
using Cmc.Stmt;
using JetBrains.Annotations;

namespace Cmc.Expr
{
	/// <summary>
	///     A function is a variable with the type of lambda
	///     This is the class for anonymous lambda
	/// </summary>
	public class LambdaExpression : Expression
	{
		[CanBeNull] public Type DeclaredType;
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
			DeclaredType = returnType;
			ParameterList = parameterList ?? new List<VariableDeclaration>(0);
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			if (DeclaredType is UnknownType unknownType)
			{
				unknownType.SurroundWith(Env);
				DeclaredType = unknownType.Resolve();
			}
			foreach (var variableDeclaration in ParameterList)
				variableDeclaration.SurroundWith(Env);
			var bodyEnv = new Environment(Env);
			foreach (var variableDeclaration in ParameterList)
				bodyEnv.Declarations.Add(variableDeclaration);
			// FEATURE #37
			var recur = new VariableDeclaration(MetaData, ReservedWords.Recur, this);
			// https://github.com/Cm-lang/Cm-Document/issues/12
			if (null != DeclaredType)
				_type = new LambdaType(MetaData, (
					from i in ParameterList
					select i.Type).ToList(), DeclaredType);
			recur.SurroundWith(Env);
			bodyEnv.Declarations.Add(recur);
			Body.SurroundWith(bodyEnv);
			var retTypes = Body.FindReturnStatements().Select(i =>
			{
				i.WhereToJump = this;
				return i.Expression.GetExpressionType();
			}).ToList();
			// FEATURE #24
			if (retTypes.Any(i => !Equals(i, DeclaredType ?? retTypes.First())))
				Errors.Add(
					$"{MetaData.GetErrorHeader()}ambiguous return types:\n" +
					(DeclaredType != null ? $"<{DeclaredType}>" : "") +
					$"[{string.Join(",", retTypes.Select(i => i.ToString()))}]");
			// FEATURE #12
			var retType = DeclaredType ?? (retTypes.Count != 0
				              ? retTypes.First()
				              // FEATURE #19
				              : new PrimaryType(MetaData, PrimaryType.NullType));
			_type = new LambdaType(MetaData, (
				from i in ParameterList
				select i.Type).ToList(), retType);
		}

		public override Type GetExpressionType() => _type;

		public override IEnumerable<string> Dump() => new[]
			{
				"lambda:\n",
				"  type:\n"
			}
			.Concat(GetExpressionType().Dump().Select(MapFunc2))
			.Concat(new[] {"  parameters:\n"})
			.Concat(
				from i in ParameterList
				from j in i.Dump().Select(MapFunc2)
				select j)
			.Concat(new[] {"  body:\n"})
			.Concat(Body.Dump().Select(MapFunc2));
	}
}