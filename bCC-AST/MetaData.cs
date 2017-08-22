namespace bCC_AST
{
	public struct MetaData
	{
		public int LineNumber;
		public string FileName;

		public string GetErrorHeader() => "Error in file " + FileName + " at line " + LineNumber + ": ";
	}
}