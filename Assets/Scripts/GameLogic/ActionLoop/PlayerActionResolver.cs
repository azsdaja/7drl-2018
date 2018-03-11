using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using Assets.Scripts.GameLogic.ActionLoop.AI;
using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.GridRelated;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.GameLogic.ActionLoop
{
	public class PlayerActionResolver : IPlayerActionResolver
	{
		private readonly IEntityDetector _entityDetector;
		private readonly IGameContext _gameContext;
		private readonly IUiConfig _uiConfig;
		private readonly IInputHolder _inputHolder;
		private readonly IActionFactory _actionFactory;
		private readonly IArrowsVisibilityManager _arrowsVisibilityManager;
		private readonly IWeaponColorizer _weaponColorizer;
		private readonly IClearWayBetweenTwoPointsDetector _clearWayBetweenTwoPointsDetector;
		private Tile _heavyDoorsHClosedTile;
		private Tile _heavyDoorsVClosedTile;

		public PlayerActionResolver(IEntityDetector entityDetector, IInputHolder inputHolder, 
			IActionFactory actionFactory, IArrowsVisibilityManager arrowsVisibilityManager, IWeaponColorizer weaponColorizer, 
			IClearWayBetweenTwoPointsDetector clearWayBetweenTwoPointsDetector, IGameContext gameContext, IUiConfig uiConfig)
		{
			_entityDetector = entityDetector;
			_inputHolder = inputHolder;
			_actionFactory = actionFactory;
			_arrowsVisibilityManager = arrowsVisibilityManager;
			_weaponColorizer = weaponColorizer;
			_clearWayBetweenTwoPointsDetector = clearWayBetweenTwoPointsDetector;
			_gameContext = gameContext;
			_uiConfig = uiConfig;
		}

		public IGameAction GetAction(ActorData actorData)
		{
			if (_inputHolder.PlayerInput == PlayerInput.None)
			{
				return null;
			}

			if (_inputHolder.PlayerInput != PlayerInput.UseCurrentItem && _inputHolder.PlayerInput != PlayerInput.DropCurrentItem)
			{
				_uiConfig.ItemHolder.DeselectItem();
				_uiConfig.TooltipPresenter.Panel.gameObject.SetActive(false);
				var quitButton = GameObject.FindGameObjectWithTag("button_quit");
				if(quitButton!=null)
					quitButton.SetActive(false);
			}

			IGameAction gameActionToReturn;

			if (_inputHolder.PlayerInput == PlayerInput.PickUp)
			{
				IList<ItemData> items = _entityDetector.DetectItems(actorData.LogicalPosition).ToList();
				bool allSlotsOccupied = _uiConfig.ItemHolder.Items.All(i => i != null);
				if (!items.Any() || allSlotsOccupied)
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
				ActorData caughtActor = _entityDetector.DetectActors(actorData.LogicalPosition, 1).FirstOrDefault();
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
			if (_inputHolder.PlayerInput == PlayerInput.UseCurrentItem)
			{
				_inputHolder.PlayerInput = PlayerInput.None;
				ItemDefinition item = _uiConfig.ItemHolder.CurrentItem();
				if (item == null)
					return null;

				return _actionFactory.CreateUseItemAction(actorData, item);
			}
			if (_inputHolder.PlayerInput == PlayerInput.DropCurrentItem)
			{
				_inputHolder.PlayerInput = PlayerInput.None;
				ItemDefinition item = _uiConfig.ItemHolder.CurrentItem();
				if (item == null)
					return null;

				return _actionFactory.CreateDropItemAction(actorData, item);
			}
			if (_inputHolder.PlayerInput == PlayerInput.Ascend)
			{
				_inputHolder.PlayerInput = PlayerInput.None;
				Vector2Int playerPosition = _gameContext.PlayerActor.ActorData.LogicalPosition;
				TileBase envTileBelowPlayer = _gameContext.EnvironmentTilemap.GetTile(playerPosition.ToVector3Int());
				if (envTileBelowPlayer != null)
				{
					return _actionFactory.CreateAscendAction(actorData);
				}
				return null;
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
			if (_inputHolder.PlayerInputModifier == PlayerInputModifier.DaringBlow)
			{
				isAggressiveAttack = true;
				_inputHolder.PlayerInputModifier = PlayerInputModifier.None;
				_inputHolder.PlayerInput = PlayerInput.None;
				_weaponColorizer.Decolorize((actorData.Entity as ActorBehaviour).WeaponAnimator);
			}

			IList<Vector2Int> targetPositionsCone = Vector2IntUtilities.GetCone(actionVector)
				.Select(zeroBasedPosition => actorData.LogicalPosition + zeroBasedPosition)
				.ToList();
			IList<ActorData> actorsCloseToCone = _entityDetector.DetectActors(targetPosition, 2).ToList();

			ActorData targetActor;
			ActorData actorAtTargetPosition = actorsCloseToCone.FirstOrDefault(a => a.LogicalPosition == targetPosition);
			if (actorAtTargetPosition != null && actorAtTargetPosition.Team == actorData.Team)
			{
				_inputHolder.PlayerInput = PlayerInput.None;
				gameActionToReturn = _actionFactory.CreateDisplaceAction(actorData, actorAtTargetPosition);
				return gameActionToReturn;
			}

			bool isPushing = _inputHolder.PlayerInputModifier == PlayerInputModifier.Push;
			_arrowsVisibilityManager.Hide();
			_inputHolder.PlayerInputModifier = PlayerInputModifier.None;
			if ((actorAtTargetPosition != null || isPushing) || actorData.WeaponWeld.WeaponDefinition.AllowsFarCombat == false)
			{
				targetActor = actorAtTargetPosition;
			}
			else
			{
				targetActor = actorsCloseToCone
					.FirstOrDefault(potentialTarget => potentialTarget.Team != actorData.Team && 
					targetPositionsCone.Contains(potentialTarget.LogicalPosition)
					&& _clearWayBetweenTwoPointsDetector.ClearWayExists(actorData.LogicalPosition, potentialTarget.LogicalPosition));
			}
			
			if(targetActor != null) // hit!
			{
				gameActionToReturn = isPushing
					? _actionFactory.CreatePushAction(actorData, targetActor)
					: _actionFactory.CreateAttackAction(actorData, targetActor, isAggressiveAttack);
			}
			else
			{
				TileBase wallTileAtTarget = _gameContext.WallsTilemap.GetTile(targetPosition.ToVector3Int());
				if (wallTileAtTarget != null)
				{

					_heavyDoorsHClosedTile = Resources.Load<Tile>("Tiles/Environment/doors_HEAVY_0");
					_heavyDoorsVClosedTile = Resources.Load<Tile>("Tiles/Environment/doors_HEAVY_2");
					if ((wallTileAtTarget == _heavyDoorsHClosedTile || wallTileAtTarget == _heavyDoorsVClosedTile)
						&& _uiConfig.ItemHolder.Items.Where(i => i!= null).All(i => i.Name != "Key")
						&& _gameContext.PlayerActor.ActorData.WeaponWeld.Name != "Key")
					{
						// bump	
						gameActionToReturn = _actionFactory.CreateMoveAction(actorData, actionVector);
					}
					else
					{
						var doorsHClosedTile = Resources.Load<Tile>("Tiles/Environment/doors_H_closed");
						var doorsVClosedTile = Resources.Load<Tile>("Tiles/Environment/doors_V_closed");

						if (wallTileAtTarget == doorsHClosedTile || wallTileAtTarget == doorsVClosedTile)
						{
							bool isHorizontal = wallTileAtTarget == doorsHClosedTile;
							gameActionToReturn = _actionFactory.CreateOpenDoorAction(actorData, targetPosition, isHorizontal);
						}
						else if (wallTileAtTarget == _heavyDoorsHClosedTile || wallTileAtTarget == _heavyDoorsVClosedTile)
						{
							bool isHorizontal = wallTileAtTarget == _heavyDoorsHClosedTile;
							gameActionToReturn = _actionFactory.CreateOpenDoorAction(actorData, targetPosition, isHorizontal, true);
						}
						else
						{
							gameActionToReturn = _actionFactory.CreateMoveAction(actorData, actionVector);
						}
					}
				}
				else
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