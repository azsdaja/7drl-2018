using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GameLogic.GameCore;
using UnityEngine;
using UnityEngine.UI;

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
			if (ActorData.WeaponWeld.Name == "DogBite" && ItemToPickUp.ItemDefinition.Name == "Key") // uuch...
			{
				ActorData.WeaponWeld = ItemToPickUp.ItemDefinition;
				Action effectAction = () => ((ActorBehaviour)ActorData.Entity).WeaponAnimator.Awake();
				var effect = new LambdaEffect(effectAction);
				_entityRemover.RemoveItem(ItemToPickUp);
				var currentWeaponImage = _uiConfig.CurrentWeaponHolder.gameObject.transform.Find("Image").GetComponent<Image>();
				currentWeaponImage.sprite = ItemToPickUp.ItemDefinition.Sprite;
				currentWeaponImage.color = Color.white;
				yield return effect;
				yield break;
			}
			_uiConfig.ItemHolder.AddItem(ItemToPickUp.ItemDefinition);

			_entityRemover.RemoveItem(ItemToPickUp);
			yield break;
		}
	}
}
