using UnityEngine;

namespace Assets.Scripts.GameLogic.GameCore
{
	public interface IWeaponColorizer
	{
		void Colorize(Color color);
		void Decolorize();
	}
}