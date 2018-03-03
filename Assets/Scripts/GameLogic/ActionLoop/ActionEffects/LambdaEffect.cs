using System;

namespace Assets.Scripts.GameLogic.ActionLoop.ActionEffects
{
	public class LambdaEffect : IActionEffect
	{
		public Action EffectAction { get; private set; }

		public LambdaEffect(Action effectAction)
		{
			EffectAction = effectAction;
		}

		public virtual void Process()
		{
			EffectAction();
		}
	}
}