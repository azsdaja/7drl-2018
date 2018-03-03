using System;

namespace Assets.Scripts.CSharpUtilities
{
	public static class EnumExtensions
	{
		public static bool HasFlag(this Enum checker, Enum @checked)
		{
			// check if from the same type.
			if (checker.GetType() != @checked.GetType())
			{
				throw new ArgumentException("The checked flag is not from the same type as the checked variable.");
			}

			ulong checkerLong = Convert.ToUInt64(@checked);
			ulong checkedLong = Convert.ToUInt64(checker);

			return (checkedLong & checkerLong) == checkerLong;
		}
	}
}