using System.Collections.Generic;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using Assets.Scripts.UI;

namespace Assets.Scripts.GameLogic.ActionLoop
{
	public class EatAction : GameAction
	{
		private readonly INeedHandler _needHandler;
		private readonly ITextEffectPresenter _textEffectPresenter;
		private readonly IEntityRemover _entityRemover;
		private readonly ItemData _foodItemToEat;

		public EatAction(ActorData actorData, float energyCost, IActionEffectFactory actionEffectFactory, INeedHandler needHandler, 
			ITextEffectPresenter textEffectPresenter, IEntityRemover entityRemover, ItemData foodItemToEat)
			: base(actorData, energyCost, actionEffectFactory)
		{
			_needHandler = needHandler;
			_textEffectPresenter = textEffectPresenter;
			_entityRemover = entityRemover;
			_foodItemToEat = foodItemToEat;
		}

		public override IEnumerable<IActionEffect> Execute()
		{
			_entityRemover.RemoveItem(_foodItemToEat);

			return new[]{new LambdaEffect(() =>
			{
				_textEffectPresenter.ShowTextEffect(ActorData.LogicalPosition, "Munch!");
			})};
		}
	}
}
