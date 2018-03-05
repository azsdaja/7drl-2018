using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.GridRelated;

namespace Assets.Scripts.GameLogic.ActionLoop
{
	public class NeedHandler : INeedHandler
	{
		private readonly IEntityDetector _entityDetector;
		private readonly IGameConfig _gameConfig;
		private readonly IDeathHandler _deathHandler;

		public NeedHandler(IEntityDetector entityDetector, IGameConfig gameConfig, IDeathHandler deathHandler)
		{
			_entityDetector = entityDetector;
			_gameConfig = gameConfig;
			_deathHandler = deathHandler;
		}

		public void Heartbeat(ActorData actorData)
		{
			++actorData.RoundsCount;

			if (actorData.Health < actorData.MaxHealth)
				++actorData.Health;

			if (actorData.Swords < actorData.MaxSwords && actorData.RoundsCount % actorData.Weapon.RecoveryTime == 0)
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