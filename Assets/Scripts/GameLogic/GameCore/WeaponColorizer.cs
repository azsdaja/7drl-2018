using UnityEngine;

namespace Assets.Scripts.GameLogic.GameCore
{
	public class WeaponColorizer : IWeaponColorizer
	{
		private IGameContext _gameContext;
		private SpriteRenderer _playerWeaponSpriteRenderer;

		public WeaponColorizer(IGameContext gameContext)
		{
			_gameContext = gameContext;
			_playerWeaponSpriteRenderer = _gameContext.PlayerActor.transform.Find("Weapon").GetComponent<SpriteRenderer>();
		}

		public void Colorize(Color color)
		{
			_playerWeaponSpriteRenderer.color = color;
		}

		public void Decolorize()
		{
			_playerWeaponSpriteRenderer.color = Color.white;
		}
	}
}