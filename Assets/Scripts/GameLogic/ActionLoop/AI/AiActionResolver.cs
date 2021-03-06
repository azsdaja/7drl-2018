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

		private IGameAction ResolveActionForAggresion(ActorData actorData)
		{
			if (_rng.Check(0.04f))
			{
				if (_gameContext.PlayerActor.ActorData.Health <= 0)
				{
					if (_rng.Check(0.07f))
					{
						string text = _rng.Choice(new[] { "Ha, ha!", "I got him!", "I know my strength!", "Got what he deserved!",
							"Guess what we'll cook for dinner..." });
						_textEffectPresenter.ShowTextEffect(actorData.LogicalPosition, text);
					}
				}
				else if (actorData.ActorType == ActorType.Dog && actorData.Team != Team.Beasts)
				{
					string text = _rng.Choice(new[] {"Woof", "Wrrrr!"});
					_textEffectPresenter.ShowTextEffect(actorData.LogicalPosition, text);
				}
				else if (actorData.ActorType == ActorType.LastMonster)
				{
					var potential = new[] {"Whshsh!", "Rrrruv!"}.ToList();
					if(!actorData.Entity.IsVisible) potential.AddRange(new[]{"[THUD!]", "[THUD!]", "[THUD!]"});
					string text = _rng.Choice(potential);
					_textEffectPresenter.ShowTextEffect(actorData.LogicalPosition, text, text == "[THUD!]" ? Color.white 
						: Color.magenta, true);
				}
				else if (actorData.ActorType == ActorType.Friend || actorData.ActorType == ActorType.Buddy)
				{
					string text = _rng.Choice(new[] {"Ma-uluh, ruv!", "Suku bgeve lir...", "Alir tak rettenekopast!"});
					_textEffectPresenter.ShowTextEffect(actorData.LogicalPosition, text, new Color(0.7f, 0.8f, 1f));
				}
				else if (actorData.ActorType != ActorType.Basher && actorData.ActorType != ActorType.LastMonster)
				{
					string text = _rng.Choice(new[] { "Back to your ward!", "Squeak!", "You're mine!", "Comrades, help me!", "Aah!" });
					_textEffectPresenter.ShowTextEffect(actorData.LogicalPosition, text);
				}
			}

			List<ActorData> enemiesClose = _entityDetector.DetectActors(actorData.LogicalPosition, actorData.VisionRayLength)
				.Where(a => a.Team != actorData.Team && a.Entity.IsVisible)
				.ToList();
			enemiesClose.Sort((first, second) => 
				Vector2IntUtilities.WalkDistance(first.LogicalPosition, actorData.LogicalPosition) 
				.CompareTo(Vector2IntUtilities.WalkDistance(second.LogicalPosition, actorData.LogicalPosition)) 
			);
			ActorData closestEnemy = enemiesClose.FirstOrDefault();

			if (closestEnemy != null)
			{
				Vector2Int toEnemy = closestEnemy.LogicalPosition - actorData.LogicalPosition;

				// mam nadzieje, ze zadziala
				if(Vector2IntUtilities.WalkDistance(closestEnemy.LogicalPosition, actorData.LogicalPosition) == 2)
				{ // move towards player if possible and desired
					bool possible = false;

					Vector2Int? legalMove = null;
					Vector2Int directionToEnemy = Vector2IntUtilities.Normalized(toEnemy);
					IEnumerable<Vector2Int> candidateMovesToEnemy = Vector2IntUtilities.GetCone(directionToEnemy)
						.Select(coneVector => coneVector - directionToEnemy)
						.Where(fixedConeVector => fixedConeVector != actorData.LogicalPosition);

					IList<Vector2Int> candidateMovesShuffled = _rng.Shuffle(candidateMovesToEnemy);
					foreach (var cand in candidateMovesShuffled)
					{
						if (!_entityDetector.DetectActors(actorData.LogicalPosition + cand).Any()
						    && _gridInfoProvider.IsWalkable(actorData.LogicalPosition + cand))
						{
							legalMove = cand;
							break;
						}
					}
					if(legalMove.HasValue)
					{
						int closeCombatAdvantage = actorData.WeaponWeld.WeaponDefinition.CloseCombatModifier -
						                               closestEnemy.WeaponWeld.WeaponDefinition.CloseCombatModifier;
						if (closeCombatAdvantage < 0) closeCombatAdvantage = 0;
						float chanceToMove = .1f + 0.2f * closeCombatAdvantage;
						if (_rng.Check(chanceToMove))
						{
							return _actionFactory.CreateMoveAction(actorData, legalMove.Value);
						}
					}
				}

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
						float pushingChanceScore = 0.08f;
						if (actorData.WeaponWeld.WeaponDefinition.CloseCombatModifier < closestEnemy.WeaponWeld.WeaponDefinition.CloseCombatModifier)
							pushingChanceScore += .25f;
						if (!_gridInfoProvider.IsWalkable(closestEnemy.LogicalPosition + toEnemy + toEnemy))
							pushingChanceScore += .2f;

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
							float healthFactor = (1 - actorData.HealthProgress) * .15f;
							float swordsFactor = (closestEnemy.Swords - actorData.Swords) * .15f;
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
							float daringBlowChance = actorData.AiTraits.Contains(AiTrait.Aggressive) ? .5f : .2f;
							if (actorData.ActorType == ActorType.Basher) daringBlowChance += .2f;
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
				ActorData playerClose = _entityDetector.DetectActors(actorData.LogicalPosition, actorData.VisionRayLength).FirstOrDefault(
					f => f != actorData && f.ActorType == ActorType.Player);
				if (playerClose != null)
				{
					Vector2Int toFriend = playerClose.LogicalPosition - actorData.LogicalPosition;
					Vector2Int directionToFriend = Vector2IntUtilities.Normalized(toFriend);
					Vector2Int legalMove = new Vector2Int();

					IEnumerable<Vector2Int> candidateMovesToFriend = Vector2IntUtilities.GetCone(directionToFriend)
						.Select(coneVector => coneVector - directionToFriend)
						.Where(fixedConeVector => fixedConeVector != actorData.LogicalPosition);

					IList<Vector2Int> candidateMovesShuffled = _rng.Shuffle(candidateMovesToFriend);
					foreach (var cand in candidateMovesShuffled)
					{
						if (!_entityDetector.DetectActors(actorData.LogicalPosition + cand).Any()
						    && _gridInfoProvider.IsWalkable(actorData.LogicalPosition + cand))
						{
							legalMove = cand;
							break;
						}
					}
					
					Vector2Int moveVector = legalMove;
					if (!_entityDetector.DetectActors(actorData.LogicalPosition + moveVector).Any())
					{
						return _actionFactory.CreateMoveAction(actorData, moveVector);
					}
				}
				return _actionFactory.CreatePassAction(actorData);
			}
			if (Vector2IntUtilities.WalkDistance(actorData.LogicalPosition, _gameContext.PlayerActor.ActorData.LogicalPosition) < 15
				&& _gameContext.PlayerActor.ActorData.Health > 0 && actorData.ActorType != ActorType.Basher)
			{
				Vector2Int? farReachablePoint = GetFarReachablePoint(actorData);
				if (farReachablePoint.HasValue)
				{
					Vector2Int moveVector = Vector2IntUtilities.Normalized(farReachablePoint.Value - actorData.LogicalPosition);
					return _actionFactory.CreateMoveAction(actorData, moveVector);
				}
			}
			return _actionFactory.CreatePassAction(actorData);
		}

		private Vector2Int? GetFarReachablePoint(ActorData actorData)
		{
			var reachableRandomPoints = new List<Vector2Int>();
			for (int i = 0; i < 5; i++)
			{
				Vector2Int wanderVector = new Vector2Int(_rng.Next(-7, 7), _rng.Next(-7, 7));
				Vector2Int candidate = actorData.LogicalPosition + wanderVector;
				if (_clearWayBetweenTwoPointsDetector.ClearWayExists(actorData.LogicalPosition, candidate))
				{
					reachableRandomPoints.Add(candidate);
				}
			}
			if (reachableRandomPoints.Any())
			{
				reachableRandomPoints.Sort((first, second) =>
				Vector2IntUtilities.WalkDistance(actorData.LogicalPosition, first).CompareTo(Vector2IntUtilities.WalkDistance(actorData.LogicalPosition, second))
				);

				return reachableRandomPoints.First();
			}

			return null;
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
	}
}