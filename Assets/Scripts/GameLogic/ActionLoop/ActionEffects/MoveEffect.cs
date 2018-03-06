using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.GameLogic.Animation;
using Assets.Scripts.GridRelated;
using Assets.Scripts.Pathfinding;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop.ActionEffects
{
	public class MoveEffect : IActionEffect
	{
		private readonly IGridInfoProvider _gridInfoProvider;
		private readonly IEntityDetector _entityDetector;
		private readonly ActorAligner _actorAligner;
		public Vector2Int PreviousPosition { get; private set; }
		public ActorData ActorData { get; private set; }

		public MoveEffect(ActorData actorData, Vector2Int previousPosition, IGridInfoProvider gridInfoProvider, IEntityDetector entityDetector)
		{
			ActorData = actorData;
			PreviousPosition = previousPosition;
			_gridInfoProvider = gridInfoProvider;
			_entityDetector = entityDetector;
			_actorAligner = new ActorAligner();
		}

		public virtual void Process()
		{
			IGameEntity entity = ActorData.Entity;
			IEntityAnimator entityAnimator = entity.EntityAnimator;

			IEnumerable<ActorData> enemiesNearby = _entityDetector.DetectActors(ActorData.LogicalPosition, 3)
				.Where(e => ActorData.Team != e.Team)
				.Where(e => Vector2IntUtilities.IsTwoSteps(e.LogicalPosition, ActorData.LogicalPosition));

			List<Vector2Int> directionsToEnemiesNearby = enemiesNearby.Select(e => e.LogicalPosition - ActorData.LogicalPosition).ToList();
			bool thereAreSomeEnemiesOnOneSide = directionsToEnemiesNearby.Any()
				&& 
				(directionsToEnemiesNearby.All(direction => direction.x < 0)
				|| directionsToEnemiesNearby.All(direction => direction.x > 0));
			if (thereAreSomeEnemiesOnOneSide)
			{
				_actorAligner.AlignActorToDirection(ActorData.Entity, directionsToEnemiesNearby.First().x);
			}
			else
			{
				_actorAligner.AlignActorToDirection(ActorData.Entity, ActorData.LogicalPosition.x - PreviousPosition.x);
			}

			if (entity.IsVisible)
				entityAnimator.MoveTo(PreviousPosition, ActorData.LogicalPosition);
			else
			{
				Vector3 animationTargetPosition = _gridInfoProvider.GetCellCenterWorld(ActorData.LogicalPosition);
			
				// The next line makes this class kind of untestable, because ActorData.Entity is a MonoBehaviour. I believe it must be so,
				// so that we can see and assign the reference to it in the inspector window. If this happens more often in other effects, 
				// we could maybe extract some kind of proxy class to keep the Entity, so that we can test with fake proxy.
				entity.Position = animationTargetPosition;
			}
		}
	}
}