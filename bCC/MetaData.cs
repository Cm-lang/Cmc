namespace bCC
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
		public string GetErrorHeader() => "Error in file " + FileName + " at line " + LineNumber + ": ";
		public static readonly MetaData DefaultMetaData = new MetaData(-1, "Unknown");
	}
}
