using UnityEngine;

namespace Assets.Scripts.CSharpUtilities
{
	public class ColorUtilities
	{
		public static Color Lerp3(Color color1, Color color2, Color color3, float value)
		{
			if (value < .5f)
			{
				float firstHalfValue = value * 2;
				return Color.Lerp(color1, color2, firstHalfValue);
			
			}
			else
			{
				float secondHalfValue = (value - .5f) * 2;
				return Color.Lerp(color2, color3, secondHalfValue);
			}
		}
	}
}
