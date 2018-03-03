using System.Collections.Generic;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;

namespace Assets.Scripts.GameLogic.ActionLoop.Actions
{
	public class DisplaceAction : GameAction
	{
		public ActorData DisplacedActor { get; private set; }

		public DisplaceAction(ActorData actorData, ActorData displacedActor, float energyCost, IActionEffectFactory actionEffectFactory) 
			: base(actorData, energyCost, actionEffectFactory)
		{
			DisplacedActor = displacedActor;
		}

		public override IEnumerable<IActionEffect> Execute()
		{
			ActorData activeActor = ActorData;

			var activeActorPositionBefore = activeActor.LogicalPosition;
			var displacedActorPositionBefore = DisplacedActor.LogicalPosition;

			activeActor.LogicalPosition = displacedActorPositionBefore;
			DisplacedActor.LogicalPosition = activeActorPositionBefore;

			yield return ActionEffectFactory.CreateMoveEffect(activeActor, activeActorPositionBefore);
			yield return ActionEffectFactory.CreateMoveEffect(DisplacedActor, displacedActorPositionBefore);
		}
	}
}
