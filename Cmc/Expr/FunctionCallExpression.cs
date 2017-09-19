using System;
using System.Collections.Generic;
using System.Linq;
using Cmc.Core;
using Cmc.Decl;
using Cmc.Stmt;
using JetBrains.Annotations;
using static System.StringComparison;
using Environment = Cmc.Core.Environment;

namespace Cmc.Expr
{
	public class FunctionCallExpression : Expression
	{
		[NotNull] public readonly IList<Expression> ArgsList;
		[NotNull] public readonly Expression Receiver;
		private Type _type;

		public FunctionCallExpression(
			MetaData metaData,
			[NotNull] Expression receiver,
			[NotNull] IList<Expression> parameterList) :
			base(metaData)
		{
			Receiver = receiver;
			ArgsList = parameterList;
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			foreach (var expression in ArgsList)
				expression.SurroundWith(environment);
			Receiver.SurroundWith(Env);
			// FEATURE #33
			if (Receiver is VariableExpression receiver)
			{
				var argsTypes = (from i in ArgsList
						select i.GetExpressionType())
					.ToList();
				var receiverDeclaration = Env.FindDeclarationSatisfies(declaration =>
					string.Equals(declaration.Name, receiver.Name, Ordinal) &&
					((declaration is VariableDeclaration variableDeclaration &&
					  variableDeclaration.Type is LambdaType lambdaType &&
					  lambdaType.ParamsList.Count == ArgsList.Count &&
					  lambdaType.ParamsList.SequenceEqual(argsTypes)) ||
					 (declaration is ExternDeclaration externDeclaration &&
					  externDeclaration.Type is LambdaType lambdaType2 &&
					  lambdaType2.ParamsList.Count == ArgsList.Count &&
					  lambdaType2.ParamsList.SequenceEqual(argsTypes))));
				if (null != receiverDeclaration)
				{
					// receiverDeclaration is obviously a variable declaraion / extern declaration
					// because it's one of the filter condition
					switch (receiverDeclaration)
					{
						case VariableDeclaration variableDeclaration:
							receiver.ChangeDeclaration(variableDeclaration);
							break;
						case ExternDeclaration externDeclaration:
							receiver.ChangeDeclaration(externDeclaration);
							break;
					}
				}
				else
					Errors.Add($"{MetaData.GetErrorHeader()}unresolved reference: \"{receiver.Name}\"");
			}
			LambdaType hisType;
			try
			{
				hisType = Receiver.GetExpressionType() as LambdaType;
			}
			catch (CompilerException)
			{
				if (Receiver is VariableExpression variable && string.Equals(variable.Name, "recur", Ordinal))
					throw new CompilerException(
						$"{MetaData.GetErrorHeader()}please specify lamdba return type when using `recur`");
				throw;
			}
			if (null != hisType)
			{
				_type = hisType.RetType;
				// FEATURE #32
				for (var i = 0; i < ArgsList.Count; i++)
					if (!Equals(ArgsList[i].GetExpressionType(), hisType.ParamsList[i]))
						Errors.Add(
							$"{MetaData.GetErrorHeader()}type mismatch: expected {hisType.ParamsList[i]}, " +
							$"found {ArgsList[i].GetExpressionType()}");
			}
			else
				Errors.Add(
					$"{MetaData.GetErrorHeader()}the function call receiver shoule be a function," +
					$" not {Receiver.GetExpressionType()}.");
			var tmp = Split();
			// if keepall, don't inline anything
			if (Pragma.KeepAll)
			{
				if (null != tmp)
					ConvertedResult = new ExpressionConvertedResult(tmp, this);
				return;
			}
			var statements = tmp ?? new List<Statement>();
			// FEATURE #44
			if (Receiver is LambdaExpression lambdaExpression)
			{
				var statementList = lambdaExpression.OptimizedStatementList;
				var s = statementList?.Statements.ToList() ?? lambdaExpression.Body.Statements.ToList();
				Expression ret = null;
				for (var i = s.Count - 1; i >= 0; i--)
					if (s[i] is ReturnStatement returnStatement)
					{
						ret = returnStatement.Expression;
						s.RemoveAt(i);
						break;
					}
				ret = ret ?? new NullExpression(MetaData);
				if (!(ret is AtomicExpression))
				{
					var varName = $"retTmp{(ulong) ret.GetHashCode()}";
					var expr = new VariableDeclaration(MetaData, varName, ret, type: ret.GetExpressionType());
					s.Add(expr);
					var variableExpression = new VariableExpression(MetaData, varName);
					variableExpression.ChangeDeclaration(expr);
					ret = variableExpression;
				}
				statements.AddRange(s);
				ConvertedResult = new ExpressionConvertedResult(statements, ret);
			}
			else
				ConvertedResult = new ExpressionConvertedResult(statements, this);
		}

		private IEnumerable<string> DumpParams() => new[]
			{
				"  parameters:\n"
			}
			.Concat(
				from i in ArgsList
				from j in i.Dump().Select(MapFunc2)
				select j);

		/// <summary>
		///  FEATURE #42
		///  if argument expressions are not atomic,
		///  convert them into seperate expressions
		/// </summary>
		private List<Statement> Split()
		{
			List<Statement> statements = null;
			for (var index = 0; index < ArgsList.Count; index++)
			{
				var expression = ArgsList[index];
				if (expression is AtomicExpression) continue;
				var name = $"tmp{(ulong) expression.GetHashCode()}";
				if (null == statements) statements = new List<Statement>();
				VariableDeclaration decl;
				if (null == expression.ConvertedResult)
					// FEATURE #43
					decl = new VariableDeclaration(MetaData, name, expression)
					{
						Type = expression.GetExpressionType()
					};
				else
				{
					var convertedRes = expression.ConvertedResult;
					statements.AddRange(convertedRes.ConvertedStatements);
					decl = new VariableDeclaration(MetaData, name, convertedRes.ConvertedExpression)
					{
						Type = convertedRes.ConvertedExpression.GetExpressionType()
					};
				}
				statements.Add(decl);
				ArgsList[index] = new VariableExpression(MetaData, name)
				{
					Declaration = decl
				};
			}
			return statements;
		}

		public override Type GetExpressionType() =>
			_type ?? throw new CompilerException("type cannot be inferred");

		public override IEnumerable<string> Dump() => new[]
			{
				"function call expression:\n",
				"  receiver:\n"
			}
			.Concat(Receiver.Dump().Select(MapFunc2))
			.Concat(DumpParams())
			.Concat(new[] {"  type:\n"})
			.Concat(_type.Dump().Select(MapFunc2));
	}
}