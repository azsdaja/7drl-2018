using System;
using System.Collections.Generic;
using System.Linq;
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
		private readonly IDeathHandler _deathHandler;

		public MoveAction(ActorData actorData, float energyCost, IActionEffectFactory actionEffectFactory, 
			Vector2Int direction, IGridInfoProvider gridInfoProvider, IEntityDetector entityDetector, 
			IGameContext gameContext, ITextEffectPresenter textEffectPresenter, IUiConfig uiConfig, IDeathHandler deathHandler) 
			: base(actorData, energyCost, actionEffectFactory, direction)
		{
			GuardDirection(direction);
			_gridInfoProvider = gridInfoProvider;
			_entityDetector = entityDetector;
			_gameContext = gameContext;
			_textEffectPresenter = textEffectPresenter;
			_uiConfig = uiConfig;
			_deathHandler = deathHandler;
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
					    _gameContext.PlayerActor.ActorData.LogicalPosition) <= 5)
				{
					_gameContext.BasherSteps = 1;
					_uiConfig.BasherMessage.gameObject.SetActive(true);
					_uiConfig.BasherMessage.SetMessage("Ha! So you didn't disappoint me! Finally I see you again, my rattish friend! As you might " +
					"have guessed, that was me that sent you the key. I'll be happy to take you away from this scary place. But first "+
					"we have to deal with one thing. You saved my life by attributing to yourself my deeds against the revolutionists. " +
					"But this also made me feel like a lousy coward. My honour has been terribly undermined and I need to clean it. " +
					"I challenge you to a duel! No magic, no eating, just me, you and steel. Prepare!");

					IEnumerable<ActorData> friendsAndBuddies = _entityDetector.DetectActors(ActorData.LogicalPosition, 15)
						.Where(a => a.ActorType == ActorType.Friend || a.ActorType == ActorType.Buddy);
					foreach (var friendOrBuddy in friendsAndBuddies)
					{
						_deathHandler.HandleDeath(friendOrBuddy);
					}
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
