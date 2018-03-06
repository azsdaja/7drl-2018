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
	public class NeedHandler : INeedHandler
	{
		private readonly IEntityDetector _entityDetector;
		private readonly IGameConfig _gameConfig;
		private readonly IDeathHandler _deathHandler;
		private readonly MaxSwordCalculator _maxSwordCalculator;

		public NeedHandler(IEntityDetector entityDetector, IGameConfig gameConfig, IDeathHandler deathHandler)
		{
			_entityDetector = entityDetector;
			_gameConfig = gameConfig;
			_deathHandler = deathHandler;
			_maxSwordCalculator = new MaxSwordCalculator();
		}

		public void Heartbeat(ActorData actorData)
		{
			++actorData.RoundsCount;

			if (actorData.Health < actorData.MaxHealth)
				++actorData.Health;

			IEnumerable<ActorData> actors = _entityDetector.DetectActors(actorData.LogicalPosition, 2);
			if (actors.Any(a => Vector2IntUtilities.IsOneStep(a.LogicalPosition, actorData.LogicalPosition)))
			{
				actorData.IsInCloseCombat = true;
			}
			else
			{
				actorData.IsInCloseCombat = false;
			}

			int maxSwords = _maxSwordCalculator.Calculate(actorData);
			actorData.MaxSwords = maxSwords;
			if (actorData.Swords > maxSwords)
				actorData.Swords = maxSwords;
			if (actorData.Swords < maxSwords && actorData.RoundsCount % actorData.Weapon.RecoveryTime == 0)
			{
				++actorData.Swords;
			}
		}

		public void ModifyNeed(NeedData needData, NeedType needType, float delta)
		{
			needData.ModifySatisfaction(needType, delta);
		}

		public void ModifyNeedWithFactor(NeedData needData, NeedType needType, NeedFactor needFactor, float delta)
		{
			needData.ModifyNeedWithFactor(needType, needFactor, delta);
		}

		private float GetDeerlingCareScore(ActorData activeActor)
		{
			if (activeActor.AiData.HerdMemberData.Child == null) return 0f;
			return -0.01f * Vector2IntUtilities.WalkDistance(activeActor.LogicalPosition, 
				       activeActor.AiData.HerdMemberData.Child.ActorData.LogicalPosition);
		}

		private float GetProximityScore(ActorData actorData, ActorData otherActor, float maxDistance)
		{
			float distanceNormalized = Vector2IntUtilities.WalkDistance(actorData.LogicalPosition, otherActor.LogicalPosition) / maxDistance;
			float proximity = _gameConfig.NeedConfig.ProximityToDanger.Evaluate(distanceNormalized);
			return proximity;
		}
	}
}