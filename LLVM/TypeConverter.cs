using System.Linq;
using Cmc;
using JetBrains.Annotations;

namespace LLVM
{
	public static class TypeConverter
	{
		public static string ConvertType([CanBeNull] Type type)
		{
			switch (type)
			{
				case PrimaryType primaryType:
					switch (primaryType.ToString())
					{
						case "nulltype":
						case "bool": return "i8";
						case "string": return "i8*";
						default:
							return primaryType.ToString();
					}
				case LambdaType lambdaType:
					// TODO select a class
					break;
				case SecondaryType secondaryType:
					if (secondaryType.Struct != null)
						return "{" + string.Join(",",
							       from i in secondaryType.Struct.FieldList
							       select ConvertType(i.Type)) + "}";
					throw new CompilerException($"cannot resolve {type}");
			}
		}
	}
}