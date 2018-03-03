using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.GridRelated;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop
{
	public class PlayerActionResolver : IPlayerActionResolver
	{
		private readonly IEntityDetector _entityDetector;
		private readonly IInputHolder _inputHolder;

		private readonly IActionFactory _actionFactory;

		public PlayerActionResolver(IEntityDetector entityDetector, IInputHolder inputHolder, IActionFactory actionFactory)
		{
			_entityDetector = entityDetector;
			_inputHolder = inputHolder;
			_actionFactory = actionFactory;
		}

		public IGameAction GetAction(ActorData actorData)
		{
			if (_inputHolder.PlayerInput == PlayerInput.None)
			{
				return null;
			}

			IGameAction gameActionToReturn;

			if (_inputHolder.PlayerInput == PlayerInput.PickUp)
			{
				IList<ItemData> items = _entityDetector.DetectItems(actorData.LogicalPosition).ToList();
				if (!items.Any())
				{
					return null;
				}
				ItemData itemToPickUp = items.First();
				_inputHolder.PlayerInput = PlayerInput.None;
				return _actionFactory.CreatePickUpAction(actorData, itemToPickUp);
			}

			if (_inputHolder.PlayerInput == PlayerInput.Drop)
			{
				ItemData firstItem = actorData.Items.FirstOrDefault();
				if (firstItem == null)
				{
					return null;
				}
				_inputHolder.PlayerInput = PlayerInput.None;
				return _actionFactory.CreateDropAction(actorData, firstItem);
			}
			if (_inputHolder.PlayerInput == PlayerInput.Catch)
			{
				ActorData caughtActor = _entityDetector.DetectActors(actorData.LogicalPosition, 1)
					.FirstOrDefault(a => a.ActorType == ActorType.HerdAnimalImmature);
				if (caughtActor == null)
				{
					return null;
				}
				_inputHolder.PlayerInput = PlayerInput.None;
				return _actionFactory.CreateCatchAction(actorData, caughtActor);
			}
			if (_inputHolder.PlayerInput == PlayerInput.Release)
			{
				if (actorData.CaughtActor == null)
				{
					return null;
				}
				_inputHolder.PlayerInput = PlayerInput.None;
				return _actionFactory.CreateReleaseAction(actorData);
			}
			if (_inputHolder.PlayerInput == PlayerInput.Pass)
			{
				_inputHolder.PlayerInput = PlayerInput.None;
				return _actionFactory.CreatePassAction(actorData);
			}
			if (_inputHolder.PlayerInput == PlayerInput.Eat)
			{
				_inputHolder.PlayerInput = PlayerInput.None;
				ItemData foodAtFeet = _entityDetector.DetectItems(actorData.LogicalPosition).FirstOrDefault(i => i.ItemType == ItemType.DeadBody);
				if (foodAtFeet == null)
				{
					return null;
				}
				return _actionFactory.CreateEatAction(actorData, foodAtFeet);
			}
			Vector2Int actionVector = GetActionVector(_inputHolder.PlayerInput);
			Vector2Int targetPosition = actionVector + actorData.LogicalPosition;
			ActorData actorAtTargetPosition = _entityDetector.DetectActors(targetPosition).FirstOrDefault();
			if(actorAtTargetPosition != null) // hit!
			{
				if (actorAtTargetPosition.Team == actorData.Team)
				{
					gameActionToReturn = _actionFactory.CreateDisplaceAction(actorData, actorAtTargetPosition);
				}
				else
				{
					gameActionToReturn = _actionFactory.CreateAttackAction(actorData, actorAtTargetPosition);
				}
			}
			else
			{
				float moveActionEnergyCost = 1.0f;
				gameActionToReturn = _actionFactory.CreateMoveAction(actorData, actionVector);
			}
			_inputHolder.PlayerInput = PlayerInput.None;
			return gameActionToReturn;
		}

		private Vector2Int GetActionVector(PlayerInput input)
		{
			Vector2Int actionVector = Vector2Int.zero;
			switch (input)
			{
				case PlayerInput.MoveUp:
					actionVector = new Vector2Int(0, 1);
					break;
				case PlayerInput.MoveLeft:
					actionVector = new Vector2Int(-1, 0);
					break;
				case PlayerInput.MoveDown:
					actionVector = new Vector2Int(0, -1);
					break;
				case PlayerInput.MoveRight:
					actionVector = new Vector2Int(1, 0);
					break;
				case PlayerInput.MoveUpLeft:
					actionVector = new Vector2Int(-1, 1);
					break;
				case PlayerInput.MoveUpRight:
					actionVector = new Vector2Int(1, 1);
					break;
				case PlayerInput.MoveDownLeft:
					actionVector = new Vector2Int(-1, -1);
					break;
				case PlayerInput.MoveDownRight:
					actionVector = new Vector2Int(1, -1);
					break;
			}
			return actionVector;
		}
	}
}