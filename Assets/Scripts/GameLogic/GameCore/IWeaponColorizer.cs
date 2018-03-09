using UnityEngine;

namespace Assets.Scripts.GameLogic.GameCore
{
	public interface IWeaponColorizer
	{
		void Colorize(WeaponAnimator weapon, Color color);
		void Decolorize(WeaponAnimator weaponAnimator);
	}
}