using System.Linq;
using bCC;
using JetBrains.Annotations;

namespace LLVM
{
	public class TypeConverter
	{
		public static string ConvertType([CanBeNull] Type type)
		{
			if (type is PrimaryType primaryType)
				switch (primaryType.ToString())
				{
					case "nulltype":
					case "bool": return "i8";
					case "string": return "i8*";
					default:
						return primaryType.ToString();
				}
			if (type is LambdaType lambdaType)
			{
				// TODO
			}
			if (type is SecondaryType secondaryType)
			{
				if (secondaryType.Struct != null)
					return $"{{{string.Join(",", secondaryType.Struct.FieldList.Select(i => ConvertType(i.Type)))}}}";
				throw new CompilerException($"cannot resolve {type}");
			}
			throw new CompilerException($"unknown type {type}");
		}

	}
}