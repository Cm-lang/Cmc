using System.Linq;
using JetBrains.Annotations;

namespace Cmc.Core
{
	public struct MetaData
	{
		public readonly int LineNumber;
		public readonly string FileName;
		public readonly string TrimedFileName;

		public MetaData(int lineNumber, string fileName)
		{
			LineNumber = lineNumber;
			FileName = fileName;
			TrimedFileName = string.Concat(FileName.Where(i => char.IsLetterOrDigit(i) || i == '_'));
		}

		// FEATURE #10
		[NotNull]
		public string GetErrorHeader() =>
			$"Error in file {FileName} at line {LineNumber}: ";

		private static int _count;
		public static readonly MetaData Empty = new MetaData(_count++, "Unknown");
	}
}