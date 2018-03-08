﻿using UnityEngine;

namespace Assets.Scripts.GameLogic.GameCore
{
	public class WeaponColorizer : IWeaponColorizer
	{
		private readonly IGameContext _gameContext;
		private SpriteRenderer _playerWeaponSpriteRenderer;

		public WeaponColorizer(IGameContext gameContext)
		{
			_gameContext = gameContext;
		}

		// todo: iteresting question — how should we initialize _playerWeaponSpriteRenderer if player is created at Start phase?

		public void Colorize(Color color)
		{
			_playerWeaponSpriteRenderer = _gameContext.PlayerActor.transform.Find("Weapon").GetComponent<SpriteRenderer>();
			_playerWeaponSpriteRenderer.color = color;
		}

		public void Decolorize()
		{
			_playerWeaponSpriteRenderer = _gameContext.PlayerActor.transform.Find("Weapon").GetComponent<SpriteRenderer>();
			_playerWeaponSpriteRenderer.color = Color.white;
		}
	}
}