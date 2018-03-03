using System.Collections.Generic;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using Assets.Scripts.UI;

namespace Assets.Scripts.GameLogic.ActionLoop
{
	public class EatEnvironmentAction : GameAction
	{
		private readonly INeedHandler _needHandler;
		private readonly ITextEffectPresenter _textEffectPresenter;

		public EatEnvironmentAction(ActorData actorData, float energyCost, IActionEffectFactory actionEffectFactory, 
			INeedHandler needHandler, ITextEffectPresenter textEffectPresenter) 
			: base(actorData, energyCost, actionEffectFactory)
		{
			_needHandler = needHandler;
			_textEffectPresenter = textEffectPresenter;
		}

		public override IEnumerable<IActionEffect> Execute()
		{
			return new[]{new LambdaEffect(() =>
			{
				_textEffectPresenter.ShowTextEffect(ActorData.LogicalPosition, "Munch!");
			})};

		}
	}
}