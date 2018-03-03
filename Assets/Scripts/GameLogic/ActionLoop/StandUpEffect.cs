using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GameLogic.Animation;

namespace Assets.Scripts.GameLogic.ActionLoop
{
	public class StandUpEffect : IActionEffect
	{
		public ActorData ActorData { get; private set; }

		public StandUpEffect(ActorData actorData)
		{
			ActorData = actorData;
		}

		public void Process()
		{
			// todo: this piece of code is repeated among many effects. This should be definitely shared.
			IGameEntity entity = ActorData.Entity;
			IEntityAnimator entityAnimator = entity.EntityAnimator;
			bool actorIsVisible = entity.IsVisible;
			if (actorIsVisible)
				entityAnimator.StandUp();
		}
	}
}