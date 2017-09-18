using System;
using System.Collections.Generic;
using System.Linq;
using Cmc.Core;
using Cmc.Decl;
using Cmc.Expr;
using JetBrains.Annotations;
using Environment = Cmc.Core.Environment;

namespace Cmc.Stmt
{
	public class AssignmentStatement : Statement
	{
		[NotNull] public readonly Expression LhsExpression;
		[NotNull] public readonly Expression RhsExpression;

		public AssignmentStatement(
			MetaData metaData,
			[NotNull] Expression lhsExpression,
			[NotNull] Expression rhsExpression) :
			base(metaData)
		{
			LhsExpression = lhsExpression;
			RhsExpression = rhsExpression;
		}

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			LhsExpression.SurroundWith(Env);
			RhsExpression.SurroundWith(Env);
			var lhs = LhsExpression.GetExpressionType();
			var rhs = RhsExpression.GetExpressionType();
			// FEATURE #14
			// FEATURE #11
			if (!string.Equals(rhs.ToString(), PrimaryType.NullType, StringComparison.Ordinal) &&
			    !string.Equals(lhs.ToString(), rhs.ToString(), StringComparison.Ordinal))
				Errors.Add($"{MetaData.GetErrorHeader()}assigning a {rhs} to a {lhs} is invalid.");
			// FEATURE #20
			var validLhs = LhsExpression.GetLhsExpression();
			if (null == validLhs)
				Errors.Add($"{MetaData.GetErrorHeader()}a {lhs} cannot be assigned.");
			else if (null == validLhs.Declaration)
				Errors.Add($"{MetaData.GetErrorHeader()}can't find declaration of {validLhs.Name}");
			// FEATURE #21
			else if (!validLhs.DeclarationMutability)
				Errors.Add($"{MetaData.GetErrorHeader()}cannot assign to an immutable variable.");
			else validLhs.Declaration.UsageCount++;
			if (!(RhsExpression is AtomicExpression))
				ConvertedStatementList = new StatementList(MetaData,
					new VariableDeclaration(MetaData, ""));
		}

		public override IEnumerable<string> Dump() => new[]
			{
				"assignment statemnt:\n",
				"  lhs:\n"
			}
			.Concat(LhsExpression.Dump().Select(MapFunc2))
			.Concat(new[] {"  rhs:\n"})
			.Concat(RhsExpression.Dump().Select(MapFunc2));
	}
}