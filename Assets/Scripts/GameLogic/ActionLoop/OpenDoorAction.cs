using System.Collections.Generic;
using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using Assets.Scripts.GameLogic.GameCore;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.GameLogic.ActionLoop
{
	public class OpenDoorAction : GameAction
	{
		private readonly Vector2Int _targetPosition;
		private readonly bool _isHorizontal;
		private readonly IGameContext _gameContext;

		public OpenDoorAction(ActorData actorData, Vector2Int targetPosition, bool isHorizontal, float energyCost, IActionEffectFactory actionEffectFactory, IGameContext gameContext) 
			: base(actorData, energyCost, actionEffectFactory)
		{
			_targetPosition = targetPosition;
			_isHorizontal = isHorizontal;
			_gameContext = gameContext;
		}

		public override IEnumerable<IActionEffect> Execute()
		{
			_gameContext.WallsTilemap.SetTile(_targetPosition.ToVector3Int(), null);

			TileBase openDoorTile = _isHorizontal ? Resources.Load<TileBase>("Tiles/Environment/doors_H_open") 
				: Resources.Load<TileBase>("Tiles/Environment/doors_V_open");
			_gameContext.EnvironmentTilemap.SetTile(_targetPosition.ToVector3Int(), openDoorTile);
			yield break;
		}
	}
}