using Assets.Scripts.GameLogic.Animation;

namespace Assets.Scripts.GameLogic.ActionLoop.ActionEffects
{
	public class KnockOutEffect : IActionEffect
	{
		public KnockOutEffect(ActorData actorData)
		{
			ActorData = actorData;
		}

		public ActorData ActorData { get; private set; }

		public virtual void Process()
		{
			IGameEntity entity = ActorData.Entity;
			IEntityAnimator entityAnimator = entity.EntityAnimator;
			bool actorIsVisible = entity.IsVisible;
			if (actorIsVisible)
				entityAnimator.KnockOut();
		}
	}
}
