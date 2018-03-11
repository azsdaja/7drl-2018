﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using Assets.Scripts.GameLogic.Configuration;
using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.GridRelated;
using Assets.Scripts.Pathfinding;
using Assets.Scripts.RNG;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop.AI
{
	public class AiActionResolver : IAiActionResolver
	{
		private readonly IGameContext _gameContext;
		private readonly IGridInfoProvider _gridInfoProvider;
		private readonly IRandomNumberGenerator _rng;
		private readonly IActionFactory _actionFactory;
		private readonly INavigator _navigator;
		private readonly IEntityDetector _entityDetector;
		private readonly ITextEffectPresenter _textEffectPresenter;
		private readonly IActiveNeedResolver _activeNeedResolver;
		private readonly IClearWayBetweenTwoPointsDetector _clearWayBetweenTwoPointsDetector;

		public AiActionResolver(IGameContext gameContext, IGridInfoProvider gridInfoProvider, IRandomNumberGenerator rng,
			IActionFactory actionFactory, INavigator navigator, IEntityDetector entityDetector, ITextEffectPresenter textEffectPresenter, IActiveNeedResolver activeNeedResolver, IClearWayBetweenTwoPointsDetector clearWayBetweenTwoPointsDetector)
		{
			_gameContext = gameContext;
			_gridInfoProvider = gridInfoProvider;
			_rng = rng;
			_actionFactory = actionFactory;
			_navigator = navigator;
			_entityDetector = entityDetector;
			_textEffectPresenter = textEffectPresenter;
			_activeNeedResolver = activeNeedResolver;
			_clearWayBetweenTwoPointsDetector = clearWayBetweenTwoPointsDetector;
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
				if (Vector2IntUtilities.IsOneStep(toEnemy) 
					|| (actorData.WeaponWeld.WeaponDefinition.AllowsFarCombat && Vector2IntUtilities.IsOneOrTwoSteps(toEnemy) 
								&& _clearWayBetweenTwoPointsDetector.ClearWayExists(actorData.LogicalPosition, closestEnemy.LogicalPosition)))
				{
					IGameAction actionToPerform;
					
					bool pushingIsPossible = actorData.AiTraits.Contains(AiTrait.Pusher) && Vector2IntUtilities.IsOneStep(toEnemy)
					                         && _gridInfoProvider.IsWalkable(closestEnemy.LogicalPosition + toEnemy);
					bool pushingIsDesired = false;
					if (pushingIsPossible)
					{
						float pushingChanceScore = 0.15f;
						if (actorData.WeaponWeld.WeaponDefinition.CloseCombatModifier < closestEnemy.WeaponWeld.WeaponDefinition.CloseCombatModifier)
							pushingChanceScore += .3f;
						if (!_gridInfoProvider.IsWalkable(closestEnemy.LogicalPosition + toEnemy + toEnemy))
							pushingChanceScore += .25f;

						if (_rng.Check(pushingChanceScore))
						{
							pushingIsDesired = true;
						}
					}
					
					if (pushingIsPossible && pushingIsDesired)
					{
						actionToPerform = _actionFactory.CreatePushAction(actorData, closestEnemy);
					}
					else
					{
						bool isInGoodPosition = Vector2IntUtilities.IsOneStep(toEnemy) && actorData.WeaponWeld.WeaponDefinition.CloseCombatModifier
						                                               > closestEnemy.WeaponWeld.WeaponDefinition.CloseCombatModifier;
						if (Vector2IntUtilities.IsOneOrTwoSteps(toEnemy) && !isInGoodPosition)// && actorData.AiTraits.Contains(AiTrait.Careful)))
						{


							float chanceToStepBack = 0f;
							float healthFactor = (1 - actorData.HealthProgress) * .2f;
							float swordsFactor = (closestEnemy.Swords - actorData.Swords) * .2f;
							chanceToStepBack = healthFactor + swordsFactor;
							if (_rng.Check(chanceToStepBack))
							{
								Vector2Int directionFromEnemy = Vector2IntUtilities.Normalized(toEnemy) * -1;
								IEnumerable<Vector2Int> positionsToStepBack = Vector2IntUtilities.GetCone(directionFromEnemy)
									.Select(coneVector => actorData.LogicalPosition + coneVector - directionFromEnemy)
									.Where( position => position!= actorData.LogicalPosition);
								foreach (var conePosition in positionsToStepBack)
								{
									if (!_gridInfoProvider.IsWalkable(conePosition) ||
									    _entityDetector.DetectEntities(conePosition).Any()) continue;
									Vector2Int stepBackMoveVector = conePosition - actorData.LogicalPosition;
									return _actionFactory.CreateMoveAction(actorData, stepBackMoveVector);
								}
							}
						}

						bool isDaringBlow = false;
						if (actorData.Traits.Contains(Trait.DaringBlow) && actorData.Swords >= 2)
						{
							float daringBlowChance = actorData.AiTraits.Contains(AiTrait.Aggressive) ? .7f : .3f;
							if (_rng.Check(daringBlowChance))
							{
								isDaringBlow = true;
							}
						}
						actionToPerform = _actionFactory.CreateAttackAction(actorData, closestEnemy, isDaringBlow);
					}
					if (DateTime.UtcNow < closestEnemy.BlockedUntil)
					{
						actorData.StoredAction = actionToPerform;
						actorData.BlockedUntil = closestEnemy.BlockedUntil;
						return null;
					}
					return actionToPerform;
				}
				int moveX = toEnemy.x.CompareTo(0);
				int moveY = toEnemy.y.CompareTo(0);
				Vector2Int moveVector = new Vector2Int(moveX, moveY);

				Func<Vector2Int, bool> isWalkableAndFree = position =>
					_gridInfoProvider.IsWalkable(position)
					&& !_entityDetector.DetectActors(position).Any();

				if (!isWalkableAndFree(actorData.LogicalPosition + moveVector))
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

					if (isWalkableAndFree(actorData.LogicalPosition + alternativeMoveVector1))
					{
						finalMoveVector = alternativeMoveVector1;
					}
					else if (isWalkableAndFree(actorData.LogicalPosition + alternativeMoveVector2))
					{
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
			else if (actorData.Team == Team.Beasts)
			{
				ActorData friendClose = _entityDetector.DetectActors(actorData.LogicalPosition, actorData.VisionRayLength).FirstOrDefault(
					f => f != actorData);
				if (friendClose != null)
				{
					Vector2Int toFriend = friendClose.LogicalPosition - actorData.LogicalPosition;
					int moveX = toFriend.x.CompareTo(0);
					int moveY = toFriend.y.CompareTo(0);
					Vector2Int moveVector = new Vector2Int(moveX, moveY);
					if (!_entityDetector.DetectActors(actorData.LogicalPosition + moveVector).Any())
					{
						return _actionFactory.CreateMoveAction(actorData, moveVector);
					}
				}
				_textEffectPresenter.ShowTextEffect(actorData.LogicalPosition, "Grrr!");
				return _actionFactory.CreatePassAction(actorData);
			}
			return _actionFactory.CreatePassAction(actorData);
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