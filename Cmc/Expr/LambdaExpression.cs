using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
		[NotNull] public StatementList Body;
		[NotNull] public readonly IList<VariableDeclaration> ParameterList;
		[NotNull] public readonly ReturnLabelDeclaration EndLabel;
		protected Type Type;

		public LambdaExpression(
			MetaData metaData,
			[NotNull] StatementList body,
			// FEATURE #22
			[CanBeNull] IList<VariableDeclaration> parameterList = null,
			[CanBeNull] Type returnType = null,
			[CanBeNull] ReturnLabelDeclaration endLabel = null) : base(metaData)
		{
			Body = body;
			DeclaredType = returnType;
			ParameterList = parameterList ?? new List<VariableDeclaration>(0);
			EndLabel = endLabel ?? new ReturnLabelDeclaration(MetaData, "");
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
			EndLabel.SurroundWith(Env);
			var bodyEnv = new Environment(Env);
			Env.Declarations.Add(EndLabel);
			foreach (var variableDeclaration in ParameterList)
				bodyEnv.Declarations.Add(variableDeclaration);
			// FEATURE #37
			var recur = new VariableDeclaration(MetaData, ReservedWords.Recur, this);
			// https://github.com/Cm-lang/Cm-Document/issues/12
			if (null != DeclaredType)
				Type = new LambdaType(MetaData, (
					from i in ParameterList
					select i.Type).ToList(), DeclaredType);
			// FEATURE #39
			recur.SurroundWith(Env);
			bodyEnv.Declarations.Add(recur);
			Body.SurroundWith(bodyEnv);
//			while (null != Body.OptimizedStatementList)
//				Body = Body.OptimizedStatementList;
			while (null != Body.ConvertedStatementList)
				Body = Body.ConvertedStatementList;
			Body.Statements.Add(EndLabel);
			var retTypes = EndLabel.StatementsUsingThis.Select(i =>
			{
				i.ReturnLabel = EndLabel;
				return i.Expression.GetExpressionType();
			}).ToList();
//			Body.Flatten();
			if (retTypes.Any(i => !Equals(i, DeclaredType ?? retTypes.First())))
				Errors.Add(
					$"{MetaData.GetErrorHeader()}ambiguous return types:\n" +
					(DeclaredType != null ? $"<{DeclaredType}>" : "") +
					$"[{string.Join(",", from i in retTypes select i.ToString())}]");
			// FEATURE #12
			var retType = DeclaredType ?? (retTypes.Count != 0
				              ? retTypes.First()
				              // FEATURE #19
				              : new PrimaryType(MetaData, PrimaryType.NullType));
			if (retTypes.Count > 1)
			{
				var varName = $"returnCollector{(ulong) GetHashCode()}";
				Body.Statements.Insert(0, new VariableDeclaration(MetaData, varName, type: retType));
				var returnValueCollector = new VariableExpression(MetaData, varName);
				foreach (var endLabelStatement in EndLabel.StatementsUsingThis)
					endLabelStatement.Unify(returnValueCollector);
				Body.Statements.Add(new ReturnStatement(MetaData, returnValueCollector)
				{
					ReturnLabel = EndLabel
				});
			}
			Type = new LambdaType(MetaData, (
				from i in ParameterList
				select i.Type).ToList(), retType);
		}

		public override Type GetExpressionType() => Type;

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