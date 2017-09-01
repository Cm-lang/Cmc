using System.Collections.Generic;
using System.Linq;
using Cmc.Core;
using JetBrains.Annotations;

namespace Cmc.Expression
{
	public class FunctionCallExpression : AtomicExpression
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
					lambdaType.ArgsList.SequenceEqual(ParameterList.Select(i => i.GetExpressionType())));
				if (null != receiverDeclaration)
				{
					// receiverDeclaration is obviously a variable declaraion
					// because it's one of the filter condition
					receiver.Declaration = (VariableDeclaration) receiverDeclaration;
					receiverDeclaration.Used = true;
				}
			}
			var hisType = Receiver.GetExpressionType() as LambdaType;
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
		}

		public override Type GetExpressionType() => _type ?? throw new CompilerException("type cannot be inferred");

		public override IEnumerable<string> Dump() => new[]
			{
				"function call expression:\n",
				"  receiver:\n"
			}
			.Concat(Receiver.Dump().Select(MapFunc2))
			.Concat(new[] {"  parameters:\n"})
			.Concat(ParameterList.SelectMany(i => i.Dump().Select(MapFunc2)))
			.Concat(new[] {"  type:\n"})
			.Concat(_type.Dump().Select(MapFunc2));
	}
}