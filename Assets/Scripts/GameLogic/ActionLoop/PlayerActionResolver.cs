using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.CSharpUtilities;
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
		private readonly IArrowsVisibilityManager _arrowsVisibilityManager;
		private readonly IWeaponColorizer _weaponColorizer;

		public PlayerActionResolver(IEntityDetector entityDetector, IInputHolder inputHolder, 
			IActionFactory actionFactory, IArrowsVisibilityManager arrowsVisibilityManager, IWeaponColorizer weaponColorizer)
		{
			_entityDetector = entityDetector;
			_inputHolder = inputHolder;
			_actionFactory = actionFactory;
			_arrowsVisibilityManager = arrowsVisibilityManager;
			_weaponColorizer = weaponColorizer;
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

			if (_inputHolder.PlayerInputModifier == PlayerInputModifier.Move)
			{
				_inputHolder.PlayerInputModifier = PlayerInputModifier.None;
				_inputHolder.PlayerInput = PlayerInput.None;
				_arrowsVisibilityManager.Hide();

				IEnumerable<ActorData> actorsAtTarget = _entityDetector.DetectActors(targetPosition);
				if (actorsAtTarget.Any())
				{
					return null;
				}
				gameActionToReturn = _actionFactory.CreateMoveAction(actorData, actionVector);
				return gameActionToReturn;
			}
			bool isAggressiveAttack = false;
			if (_inputHolder.PlayerInputModifier == PlayerInputModifier.Aggresive)
			{
				isAggressiveAttack = true;
				_inputHolder.PlayerInputModifier = PlayerInputModifier.None;
				_inputHolder.PlayerInput = PlayerInput.None;
				_weaponColorizer.Decolorize();
			}

			IList<Vector2Int> targetPositionsCone = Vector2IntUtilities.GetCone(actionVector)
				.Select(zeroBasedPosition => actorData.LogicalPosition + zeroBasedPosition)
				.ToList();
			IList<ActorData> enemiesCloseToCone = _entityDetector.DetectActors(targetPosition, 2)
				.Where(e => e.Team != actorData.Team).ToList();

			ActorData targetEnemy;
			ActorData enemyAtTargetPosition = enemiesCloseToCone.FirstOrDefault(a => a.LogicalPosition == targetPosition);
			if (enemyAtTargetPosition != null)
			{
				targetEnemy = enemyAtTargetPosition;
			}
			else
			{
				targetEnemy = enemiesCloseToCone.FirstOrDefault(a => targetPositionsCone.Contains(a.LogicalPosition));
			}
			
			if(targetEnemy != null) // hit!
			{
				if (targetEnemy.Team == actorData.Team)
				{
					gameActionToReturn = _actionFactory.CreateDisplaceAction(actorData, targetEnemy);
				}
				else
				{
					gameActionToReturn = _actionFactory.CreateAttackAction(actorData, targetEnemy, isAggressiveAttack);
				}
			}
			else
			{
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