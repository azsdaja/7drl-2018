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
			ActorData visibleEnemy = _entityDetector.DetectActors(actorData.LogicalPosition, actorData.VisionRayLength)
				.FirstOrDefault(a => a.Team != actorData.Team);

			if (visibleEnemy != null)
			{
				Vector2Int toEnemy = visibleEnemy.LogicalPosition - actorData.LogicalPosition;
				if (Vector2IntUtilities.IsOneStep(toEnemy) || Vector2IntUtilities.IsTwoSteps(toEnemy))
				{
					return _actionFactory.CreateAttackAction(actorData, visibleEnemy);
				}
				var moveVector = Vector2IntUtilities.Normalized(toEnemy);
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