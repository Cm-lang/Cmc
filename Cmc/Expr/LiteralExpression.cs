using Cmc.Core;
using JetBrains.Annotations;

namespace Cmc.Expr
{
	public class LiteralExpression : AtomicExpression
	{
		[NotNull] public readonly Type Type;

		protected LiteralExpression(
			MetaData metaData,
			[NotNull] Type type) :
			base(metaData) => Type = type;

		public override Type GetExpressionType() => Type;
	}

	public class IntLiteralExpression : LiteralExpression
	{
		public static readonly int[] AcceptableLength = {8, 16, 32, 64};
		public readonly int Length;

		[NotNull] public readonly string Value;

		public IntLiteralExpression(
			MetaData metaData,
			[NotNull] string value,
			// FEATURE #27
			bool isSigned,
			int length = 32)
			: base(metaData, new PrimaryType(metaData, $"{(isSigned ? "i" : "u")}{length}", length / 8))
		{
			Value = value;
			Length = length;
			// FEATURE #26
			if (!AcceptableLength.Contains(length))
				Errors.Add($"{MetaData.GetErrorHeader()}integer length of {length} is not supported");
		}

		public override IEnumerable<string> Dump() =>
			new[] {$"literal expression [{Value}]:\n"}
				.Concat(Type.Dump().Select(MapFunc));
	}

	public class BoolLiteralExpression : LiteralExpression
	{
		public readonly bool Value;

		public BoolLiteralExpression(
			MetaData metaData,
			bool value) :
			base(metaData, new PrimaryType(
				metaData,
				PrimaryType.BoolType,
				1))
			=> Value = value;

		public int ValueToInt() => Value ? 1 : 0;

		public override IEnumerable<string> Dump() =>
			new[] {$"bool literal expression [{Value}]:\n"};
	}
}