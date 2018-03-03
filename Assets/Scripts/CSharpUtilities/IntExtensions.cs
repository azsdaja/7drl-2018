namespace Assets.Scripts.CSharpUtilities
{
	public static class IntExtensions
	{
		public static int Sign(this int number)
		{
			return number > 0 ? 1 : number == 0 ? 0 : -1;
		}
	}
}

