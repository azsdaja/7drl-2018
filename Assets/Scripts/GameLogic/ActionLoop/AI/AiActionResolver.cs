using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.GridRelated;
using Assets.Scripts.RNG;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop.AI
{
	public class AiActionResolver : IAiActionResolver
	{
		private readonly IGameContext _gameContext;
		private readonly IRandomNumberGenerator _rng;
		private readonly IActionFactory _actionFactory;
		private readonly INavigator _navigator;
		private readonly IEntityDetector _entityDetector;
		private readonly ITextEffectPresenter _textEffectPresenter;
		private readonly IActiveNeedResolver _activeNeedResolver;
	
		public AiActionResolver(IGameContext gameContext, IRandomNumberGenerator rng, IActionFactory actionFactory,
			INavigator navigator, IEntityDetector entityDetector, ITextEffectPresenter textEffectPresenter, IActiveNeedResolver activeNeedResolver)
		{
			_gameContext = gameContext;
			_rng = rng;
			_actionFactory = actionFactory;
			_navigator = navigator;
			_entityDetector = entityDetector;
			_textEffectPresenter = textEffectPresenter;
			_activeNeedResolver = activeNeedResolver;
		}

		public IGameAction GetAction(ActorData actorData)
		{
			if (actorData.Entity.EntityAnimator.IsAnimating)
			{
				return null;
			}

			if (actorData.StoredAction != null)
			{
				IGameAction actionToReturn = actorData.StoredAction;
				actorData.StoredAction = null;
				return actionToReturn;
			}

			return ResolveActionForAggresion(actorData);
		}

		private IGameAction ResolveActionForRest(ActorData actorData)
		{
			if (actorData.AiData.TurnsLeftToStayWhileResting > 0)
			{
				--actorData.AiData.TurnsLeftToStayWhileResting;
				_textEffectPresenter.ShowTextEffect(actorData.LogicalPosition, "---");
				return _actionFactory.CreatePassAction(actorData);
			}
			bool isAtDestination
				= !actorData.NavigationData.Destination.HasValue ||
				  actorData.LogicalPosition == actorData.NavigationData.Destination;
			Vector2Int nextStep;
		
			if (isAtDestination)
			{
				actorData.AiData.TurnsLeftToStayWhileResting = _rng.Next(1, 10);
				NavigationData navigationData = GetReachableNavigationDataForWander(actorData, actorData.LogicalPosition, 3);
				actorData.NavigationData = navigationData;
				nextStep = _navigator.ResolveNextStep(actorData).Value;
			}
			else
			{
				nextStep = _navigator.ResolveNextStep(actorData).Value;
			}
			IGameAction moveGameAction = CreateMoveAction(actorData, nextStep);
			return moveGameAction;
		}

		private IGameAction ResolveActionForSafety(ActorData actorData)
		{
			List<ActorData> enemyActorsNearby = _entityDetector.DetectActors(actorData.LogicalPosition, 5)
				.Where(a => a.Team != actorData.Team).ToList();
			enemyActorsNearby.Sort((first, second) =>
				Vector2IntUtilities.WalkDistance(first.LogicalPosition, actorData.LogicalPosition)
					.CompareTo(Vector2IntUtilities.WalkDistance(second.LogicalPosition, actorData.LogicalPosition)));
			var closestEnemy = enemyActorsNearby.FirstOrDefault();
			if (closestEnemy == null)
			{
				Vector2Int target;
				if (actorData.ActorType == ActorType.HerdAnimalImmature && actorData.AiData.HerdMemberData.Protector != null)
				{
					target = actorData.AiData.HerdMemberData.Protector.ActorData.LogicalPosition;
				}
				else
				{
					IEnumerable<ActorData> friendlyActorsNearby = _entityDetector.DetectActors(actorData.LogicalPosition, 10)
						.Where(a => a.Team == actorData.Team);
					Vector2Int safetyCenter = Vector2IntUtilities.Average(friendlyActorsNearby.Select(a => a.LogicalPosition).ToList());
					target = safetyCenter;
				}
			
				NavigationData newNavigationData = GetReachableNavigationDataForWander(actorData, target, 7);
				actorData.NavigationData = newNavigationData;

				Vector2Int? potentialNextStep = _navigator.ResolveNextStep(actorData);
				var nextStep = potentialNextStep.Value;
				return CreateMoveAction(actorData, nextStep);
			}
			else
			{
				Vector2Int direction = Vector2IntUtilities.Normalized(actorData.LogicalPosition - closestEnemy.LogicalPosition);
				_textEffectPresenter.ShowTextEffect(actorData.LogicalPosition, "AAAAA!");
				return _actionFactory.CreateMoveAction(actorData, direction);
			}
		}

		private IGameAction ResolveActionForAggresion(ActorData actorData)
		{
			List<ActorData> enemiesClose = _entityDetector.DetectActors(actorData.LogicalPosition, actorData.VisionRayLength)
				.Where(a => a.Team != actorData.Team)
				.ToList();
			enemiesClose.Sort((first, second) => 
				Vector2IntUtilities.WalkDistance(first.LogicalPosition, actorData.LogicalPosition) 
				.CompareTo(Vector2IntUtilities.WalkDistance(second.LogicalPosition, actorData.LogicalPosition)) 
			);
			ActorData closestEnemy = enemiesClose.FirstOrDefault();

			if (closestEnemy != null)
			{
				Vector2Int toEnemy = closestEnemy.LogicalPosition - actorData.LogicalPosition;
				if (Vector2IntUtilities.IsOneStep(toEnemy) || actorData.Weapon.AllowsFarCombat && Vector2IntUtilities.IsTwoSteps(toEnemy))
				{
					IGameAction attackAction = _actionFactory.CreateAttackAction(actorData, closestEnemy);
					if (DateTime.UtcNow < closestEnemy.BlockedUntil)
					{
						actorData.StoredAction = attackAction;
						actorData.BlockedUntil = closestEnemy.BlockedUntil;
						return null;
					}
					return attackAction;
				}
				int moveX = toEnemy.x.CompareTo(0);
				int moveY = toEnemy.y.CompareTo(0);
				Vector2Int moveVector = new Vector2Int(moveX, moveY);

				var actorsBlockingMove = _entityDetector.DetectActors(actorData.LogicalPosition + moveVector);
				if (actorsBlockingMove.Any())
				{
					Vector2Int? finalMoveVector = null;
					Vector2Int alternativeMoveVector1;
					Vector2Int alternativeMoveVector2;

					// trying to find best alternative vectors to move
					if (moveVector.x == 0)
					{
						alternativeMoveVector1 = new Vector2Int(+1, moveVector.y);
						alternativeMoveVector2 = new Vector2Int(-1, moveVector.y);
					}
					else if (moveVector.y == 0)
					{
						alternativeMoveVector1 = new Vector2Int(moveVector.x, -1);
						alternativeMoveVector2 = new Vector2Int(moveVector.x, +1);
					}
					else
					{
						alternativeMoveVector1 = new Vector2Int(moveVector.x, 0);
						alternativeMoveVector2 = new Vector2Int(0, moveVector.y);
					}
					

					var actorsBlockingAlternativeMove1 = _entityDetector.DetectActors(actorData.LogicalPosition + alternativeMoveVector1);
					if (!actorsBlockingAlternativeMove1.Any())
					{
						finalMoveVector = alternativeMoveVector1;
					}
					else
					{
						var actorsBlockingAlternativeMove2 = _entityDetector.DetectActors(actorData.LogicalPosition + alternativeMoveVector2);
						if (!actorsBlockingAlternativeMove2.Any())
							finalMoveVector = alternativeMoveVector2;
					}
					if (finalMoveVector.HasValue)
					{
						return _actionFactory.CreateMoveAction(actorData, finalMoveVector.Value);
					}
					return _actionFactory.CreatePassAction(actorData);
				}
				return _actionFactory.CreateMoveAction(actorData, moveVector);
			}
			else
			{
				_textEffectPresenter.ShowTextEffect(actorData.LogicalPosition, "Grrr!");
				return _actionFactory.CreatePassAction(actorData);
			}
		}

		private NavigationData GetReachableNavigationDataForWander(ActorData actorData, Vector2Int targetAreaCenter, int wanderRange)
		{
			NavigationData newNavigationData;
			while (true)
			{
				Vector2Int wanderVector = new Vector2Int(_rng.Next(-wanderRange, wanderRange), _rng.Next(-wanderRange, wanderRange));
				Vector2Int newTargetPosition = targetAreaCenter + wanderVector;
				if (newTargetPosition == actorData.LogicalPosition)
					continue;
				List<Vector2Int> path = _navigator.GetPath(actorData.LogicalPosition, newTargetPosition);
				if (path == null)
				{
					continue;
				}
				newNavigationData = new NavigationData
				{
					Destination = newTargetPosition,
					PathToFollow = path
				};
				break;
			}
			return newNavigationData;
		}

		private IGameAction ResolveActionForHunger(ActorData actorData)
		{
			bool shouldChooseNewDestination = !actorData.NavigationData.Destination.HasValue
			                                  || !_gameContext.LeavesPositions.Contains(actorData.NavigationData.Destination
				                                  .Value);

			if (shouldChooseNewDestination)
			{
				Vector2Int newTargetPosition = ChooseRandomLeavesPosition(actorData);
				var newNavigationData = new NavigationData {Destination = newTargetPosition};
				actorData.NavigationData = newNavigationData;
			}
			if (actorData.LogicalPosition == actorData.NavigationData.Destination)
			{
				return _actionFactory.CreateEatEnvironmentAction(actorData);
			}
			else
			{
				Vector2Int nextStep = _navigator.ResolveNextStep(actorData).Value;

				IGameAction moveGameAction = CreateMoveAction(actorData, nextStep);
				return moveGameAction;
			}
		}

		private IGameAction ResolveActionForCare(ActorData actorData)
		{
			return _actionFactory.CreateCallAction(actorData);
		}

		private IGameAction CreateMoveAction(ActorData actorData, Vector2Int nextStep)
		{
			Vector2Int direction = nextStep - actorData.LogicalPosition;
			IGameAction moveGameAction = _actionFactory.CreateMoveAction(actorData, direction);
			return moveGameAction;
		}

		private Vector2Int ChooseRandomLeavesPosition(ActorData actorData)
		{
			Vector2Int newTargetPosition;
			while (true)
			{
				int candidatesCount = 5;
				var candidates = new Vector2Int[candidatesCount];
				for (int i = 0; i < candidatesCount; i++)
				{
					candidates[i] = _rng.Choice(_gameContext.LeavesPositions);
				}
				List<Vector2Int> candidatesList = candidates.ToList();
				candidatesList.Sort((first, second) =>
				{
					int distanceToFirst = (actorData.LogicalPosition - first).sqrMagnitude;
					int distanceToSecond = (actorData.LogicalPosition - second).sqrMagnitude;
					return distanceToFirst.CompareTo(distanceToSecond);
				});
				newTargetPosition = candidatesList.First();
				if (newTargetPosition != actorData.LogicalPosition)
					break;
			}
			return newTargetPosition;
		}
	}
}