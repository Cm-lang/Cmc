using JetBrains.Annotations;

namespace bCC.Core
{
	public struct MetaData
	{
		public int LineNumber;
		public string FileName;

		public MetaData(int lineNumber, string fileName)
		{
			LineNumber = lineNumber;
			FileName = fileName;
		}

		// FEATURE #10
		[NotNull]
		public string GetErrorHeader() => $"Error in file {FileName} at line {LineNumber}: ";

		private static int _count;
		public static readonly MetaData Empty = new MetaData(_count++, "Unknown");
		public static readonly MetaData BuiltIn = new MetaData(-1, "[built-in]");
	}
}