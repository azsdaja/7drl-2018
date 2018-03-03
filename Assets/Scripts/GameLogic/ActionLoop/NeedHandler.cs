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

			--actorData.Satiation;

			if (actorData.ControlledByPlayer)
			{
				if (actorData.Satiation < 1)
				{
					_deathHandler.HandleDeath(actorData);
				}
				return;
			}
	
			actorData.NeedData.ModifySatisfaction(NeedType.Hunger, -0.005f);
			actorData.NeedData.ModifySatisfaction(NeedType.Rest, -0.01f);

			float maxDistance = 10f;
			List<ActorData> actorsNearby = _entityDetector.DetectActors(actorData.LogicalPosition, maxDistance).ToList();
			List<ActorData> enemyActorsNearby = actorsNearby.Where(a => a.Team != actorData.Team).ToList();
			List<ActorData> friendlyActorsNearby = actorsNearby.Where(a => a != actorData && a.Team == actorData.Team).ToList();

			float danger = enemyActorsNearby.Sum(a => GetProximityScore(actorData, a, maxDistance));
			float safety = friendlyActorsNearby.Sum(a => GetProximityScore(actorData, a, maxDistance));
			actorData.NeedData.ModifySatisfaction(NeedType.Safety, safety);

			if (actorData.ActorType == ActorType.HerdAnimalMother || actorData.ActorType == ActorType.HerdAnimalImmature)
			{
				actorData.NeedData.ModifySatisfaction(NeedType.Safety, -danger);
			}
			if (actorData.ActorType == ActorType.HerdAnimalMother)
			{
				float deerlingCareScore = GetDeerlingCareScore(actorData);
				actorData.NeedData.ModifySatisfaction(NeedType.Care, deerlingCareScore);
			}
			if (actorData.ActorType == ActorType.HerdAnimalMother || actorData.ActorType == ActorType.HerdAnimalFather)
			{
				actorData.NeedData.ModifySatisfaction(NeedType.Aggresion, 0.03f);
				if(enemyActorsNearby.Any() && friendlyActorsNearby.Any(a => a.ActorType == ActorType.HerdAnimalImmature))
				{
					actorData.NeedData.ModifySatisfaction(NeedType.Aggresion, -0.1f);
				}
			}
			if (actorData.ActorType == ActorType.HerdAnimalFather)
			{
				actorData.NeedData.ModifySatisfaction(NeedType.Aggresion, 0.05f);
			}
			if (actorData.ActorType == ActorType.HerdAnimalImmature)
			{
				if (actorData.AiData.HerdMemberData.Protector != null)
				{
					float safetyScoreFromProtector =
						GetProximityScore(actorData, actorData.AiData.HerdMemberData.Protector.ActorData, maxDistance);
					actorData.NeedData.ModifySatisfaction(NeedType.Safety, safetyScoreFromProtector);
				}
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