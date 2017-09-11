using System.Collections.Generic;
using System.Linq;
using Cmc.Core;
using Cmc.Decl;
using Cmc.Stmt;
using JetBrains.Annotations;
using static System.StringComparison;

namespace Cmc.Expr
{
	public class FunctionCallExpression : Expression
	{
		[NotNull] public readonly IList<Expression> ParameterList;

		[NotNull] public readonly Expression Receiver;
		private Type _type;

		public FunctionCallExpression(MetaData metaData, [NotNull] Expression receiver,
			[NotNull] IList<Expression> parameterList) :
			base(metaData)
		{
			ParameterList = parameterList;
			Receiver = receiver;
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			foreach (var expression in ParameterList) expression.SurroundWith(Env);
			Receiver.SurroundWith(Env);
			// FEATURE #33
			if (Receiver is VariableExpression receiver)
			{
				var receiverDeclaration = Env.FindDeclarationSatisfies(declaration =>
					declaration is VariableDeclaration variableDeclaration &&
					variableDeclaration.Type is LambdaType lambdaType &&
					lambdaType.ArgsList.Count == ParameterList.Count &&
					lambdaType.ArgsList.SequenceEqual(
						from i in ParameterList
						select i.GetExpressionType()));
				if (null != receiverDeclaration)
				{
					// receiverDeclaration is obviously a variable declaraion
					// because it's one of the filter condition
					receiver.Declaration = (VariableDeclaration) receiverDeclaration;
					receiverDeclaration.Used = true;
				}
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
				for (var i = 0; i < ParameterList.Count; i++)
					if (!Equals(ParameterList[i].GetExpressionType(), hisType.ArgsList[i]))
						Errors.Add(
							$"{MetaData.GetErrorHeader()}type mismatch: expected {hisType.ArgsList[i]}, " +
							$"found {ParameterList[i].GetExpressionType()}");
			}
			else
				Errors.Add(
					$"{MetaData.GetErrorHeader()}the function call receiver shoule be a function," +
					$" not {Receiver.GetExpressionType()}.");
			List<Statement> statements = null;
			for (var index = 0; index < ParameterList.Count; index++)
			{
				var expression = ParameterList[index];
				if (expression is AtomicExpression) continue;
				var name = $"tmp{expression.GetHashCode()}";
				if (null == statements) statements = new List<Statement>();
				if (null == expression.ConvertedResult)
					statements.Add(new VariableDeclaration(MetaData, name, expression));
				else
				{
					var convertedRes = expression.ConvertedResult;
					statements.AddRange(convertedRes.ConvertedStatements);
					statements.Add(new VariableDeclaration(MetaData, name, convertedRes.ConvertedExpression));
				}
				ParameterList[index] = new VariableExpression(MetaData, name);
			}
			if (null != statements)
				ConvertedResult = new ExpressionConvertedResult(statements, this);
		}

		public override Type GetExpressionType() =>
			_type ?? throw new CompilerException("type cannot be inferred");

		public override IEnumerable<string> Dump() => new[]
			{
				"function call expression:\n",
				"  receiver:\n"
			}
			.Concat(Receiver.Dump().Select(MapFunc2))
			.Concat(new[] {"  parameters:\n"})
			.Concat(
				from i in ParameterList
				from j in i.Dump().Select(MapFunc2)
				select j)
			.Concat(new[] {"  type:\n"})
			.Concat(_type.Dump().Select(MapFunc2));
	}

	public class RecurCallExpression : Expression
	{
		[NotNull] public readonly IList<Expression> ParameterList;
		[CanBeNull] public LambdaExpression Outside;

		public RecurCallExpression(
			MetaData metaData,
			[NotNull] IList<Expression> parameterList) :
			base(metaData)
		{
			ParameterList = parameterList;
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			foreach (var expression in ParameterList)
				expression.SurroundWith(environment);
			var declaration = Env.FindDeclarationSatisfies(decl =>
				decl is VariableDeclaration variable &&
				variable.Type is LambdaType lambdaType &&
				string.Equals(variable.Name, ReservedWords.Recur, Ordinal) &&
				lambdaType.ArgsList.Count == ParameterList.Count &&
				lambdaType.ArgsList.SequenceEqual(
					from i in ParameterList
					select i.GetExpressionType())) as VariableDeclaration;
			if (null == declaration)
				Errors.Add(
					$"{MetaData.GetErrorHeader()}call to recur" +
					$"({string.Join(",", from p in ParameterList select p.GetExpressionType().ToString())})" +
					"not found");
			else
				Outside = (LambdaExpression) declaration.Expression;
			List<Statement> statements = null;
			for (var index = 0; index < ParameterList.Count; index++)
			{
				var expression = ParameterList[index];
				if (expression is AtomicExpression) continue;
				var name = $"tmp{expression.GetHashCode()}";
				if (null == statements) statements = new List<Statement>();
				if (null == expression.ConvertedResult)
					statements.Add(new VariableDeclaration(MetaData, name, expression));
				else
				{
					var convertedRes = expression.ConvertedResult;
					statements.AddRange(convertedRes.ConvertedStatements);
					statements.Add(new VariableDeclaration(MetaData, name, convertedRes.ConvertedExpression));
				}
				ParameterList[index] = new VariableExpression(MetaData, name);
			}
			if (null != statements)
				ConvertedResult = new ExpressionConvertedResult(statements, this);
		}

		public override Type GetExpressionType() =>
			Outside?.GetExpressionType() ??
			throw new CompilerException("cannot find a lambda outside a recur");
	}
}