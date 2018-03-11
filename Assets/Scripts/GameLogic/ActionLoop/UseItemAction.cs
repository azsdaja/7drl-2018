using System;
using System.Collections.Generic;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using Assets.Scripts.GameLogic.GameCore;

namespace Assets.Scripts.GameLogic.ActionLoop
{
	public class UseItemAction : GameAction
	{
		private readonly ItemDefinition _item;
		private readonly IEntitySpawner _entitySpawner;
		private readonly IUiConfig _uiConfig;

		public UseItemAction(ActorData actorData, ItemDefinition item, float energyCost, IEntitySpawner entitySpawner, 
			IUiConfig uiConfig, IActionEffectFactory actionEffectFactory)
			: base(actorData, energyCost, actionEffectFactory)
		{
			_item = item;
			_entitySpawner = entitySpawner;
			_uiConfig = uiConfig;
		}

		public override IEnumerable<IActionEffect> Execute()
		{
			if (_item.ItemType == ItemType.Weapon)
			{
				ItemDefinition previousWeapon = ActorData.WeaponWeld;
				_uiConfig.ItemHolder.AddItem(previousWeapon);

				ActorData.WeaponWeld = _item;
				Action effectAction = () => ((ActorBehaviour) ActorData.Entity).WeaponAnimator.Awake();
				var effect = new LambdaEffect(effectAction);
				yield return effect;
			}
			else if (_item.ItemType == ItemType.Food)
			{
				ActorData.Health = (int) (ActorData.MaxHealth * 1.3f);
				if (ActorData.Health > ActorData.MaxHealth)
					ActorData.Health = ActorData.MaxHealth;
			}
			_uiConfig.ItemHolder.RemoveItem(_uiConfig.ItemHolder.SelectedItemIndex);
		}
	}
}