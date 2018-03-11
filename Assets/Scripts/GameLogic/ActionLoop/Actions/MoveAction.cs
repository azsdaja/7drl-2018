using System;
using System.Collections.Generic;
using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.GridRelated;
using Assets.Scripts.Pathfinding;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.GameLogic.ActionLoop.Actions
{
	public class MoveAction : DirectedAction
	{
		private readonly IGridInfoProvider _gridInfoProvider;
		private readonly IEntityDetector _entityDetector;
		private readonly IUiConfig _uiConfig;
		private readonly IGameContext _gameContext;
		private readonly ITextEffectPresenter _textEffectPresenter;


		public MoveAction(ActorData actorData, float energyCost, IActionEffectFactory actionEffectFactory, 
			Vector2Int direction, IGridInfoProvider gridInfoProvider, IEntityDetector entityDetector, 
			IGameContext gameContext, ITextEffectPresenter textEffectPresenter, IUiConfig uiConfig) 
			: base(actorData, energyCost, actionEffectFactory, direction)
		{
			GuardDirection(direction);
			_gridInfoProvider = gridInfoProvider;
			_entityDetector = entityDetector;
			_gameContext = gameContext;
			_textEffectPresenter = textEffectPresenter;
			_uiConfig = uiConfig;
		}

		public override IEnumerable<IActionEffect> Execute()
		{
			Vector2Int previousPosition = ActorData.LogicalPosition;
			Vector2Int newPosition = previousPosition + Direction;
			if (_gridInfoProvider.IsWalkable(newPosition))
			{

				if (ActorData.ActorType == ActorType.Player && _gameContext.EnvironmentTilemap.HasTile(newPosition.ToVector3Int()))
				{
					TileBase stairsDownTile = Resources.Load<TileBase>("Tiles/Environment/Stairs_down");
					if (_gameContext.EnvironmentTilemap.GetTile(newPosition.ToVector3Int()) == stairsDownTile)
					{
						_textEffectPresenter.ShowTextEffect(ActorData.LogicalPosition, "Back down? Never!", Color.yellow);
					}
				}
				if (_gameContext.BasherSteps == 0 && ActorData.ActorType == ActorType.Basher && Vector2IntUtilities.WalkDistance(ActorData.LogicalPosition,
					    _gameContext.PlayerActor.ActorData.LogicalPosition) <= 3)
				{
					_gameContext.BasherSteps = 1;
					_uiConfig.BasherMessage.gameObject.SetActive(true);
					_uiConfig.BasherMessage.SetMessage("Ha! So you didn't disappoint me! Finally I see you again, my rattish friend! As you might " +
					"have guessed, that was me that sent you the key. I'll be happy to take you away from this scary place. But first things first. " +
					"You know that you saved my life by attributing to yourself my deeds against the revolutionists. But you also know " +
					"that I'm a man of honor and this honor has been terribly undermined by you covering me. I challenge you to a duel!");


				}

				IActionEffect effect = ActionEffectFactory.CreateMoveEffect(ActorData, previousPosition);
				ActorData.LogicalPosition = newPosition;
				if (ActorData.CaughtActor != null)
				{
					ActorData.CaughtActor.LogicalPosition = newPosition;
				}

				yield return effect;
			}
			else
			{
				IActionEffect effect = ActionEffectFactory.CreateBumpEffect(ActorData, newPosition);
				yield return effect;
			}
		}

		private void GuardDirection(Vector2Int direction)
		{
			if(direction.x > 1 || direction.x < -1 || direction.y > 1 || direction.y < -1)
				throw new ArgumentException("Direction to move is exceeding one step: " + direction, "direction");
		}
	}
}
