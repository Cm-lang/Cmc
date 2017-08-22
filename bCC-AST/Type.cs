namespace bCC_AST
{
	public abstract class Type
	{
		public readonly string Name;

		protected Type(string name) => Name = name;
	}

	public class SecondaryType : Type
	{
		public SecondaryType(string name) : base(name)
		{
		}
	}
}