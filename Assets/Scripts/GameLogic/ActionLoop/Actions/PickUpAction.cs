using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GameLogic.GameCore;

namespace Assets.Scripts.GameLogic.ActionLoop.Actions
{
	public class PickUpAction : GameAction
	{
		private readonly IEntityRemover _entityRemover;
		private readonly IUiConfig _uiConfig;
		private readonly IGameContext _gameContext;

		public ItemData ItemToPickUp { private set; get; }

		public PickUpAction(ActorData actorData, float energyCost, ItemData itemToPickUp, IActionEffectFactory actionEffectFactory, 
			IEntityRemover entityRemover, IUiConfig uiConfig, IGameContext gameContext) 
			: base(actorData, energyCost, actionEffectFactory)
		{
			ItemToPickUp = itemToPickUp;
			_entityRemover = entityRemover;
			_uiConfig = uiConfig;
			_gameContext = gameContext;
		}

		public override IEnumerable<IActionEffect> Execute()
		{
			if (ItemToPickUp.ItemDefinition.Name == "Key") // uuch...
			{
				_gameContext.PlayerPickedUpKey = true;

				ActorData.WeaponWeld = ItemToPickUp.ItemDefinition;
				Action effectAction = () => ((ActorBehaviour)ActorData.Entity).WeaponAnimator.Awake();
				var effect = new LambdaEffect(effectAction);
				yield return effect;
			}
			_uiConfig.ItemHolder.AddItem(ItemToPickUp.ItemDefinition);
			_entityRemover.RemoveItem(ItemToPickUp);
			yield break;
		}
	}
}
