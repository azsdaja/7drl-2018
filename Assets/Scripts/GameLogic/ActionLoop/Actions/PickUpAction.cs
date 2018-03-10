using System.Collections.Generic;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GameLogic.GameCore;

namespace Assets.Scripts.GameLogic.ActionLoop.Actions
{
	public class PickUpAction : GameAction
	{
		private readonly IEntitySpawner _entitySpawner;
		private readonly IEntityRemover _entityRemover;
		private readonly IUiConfig _uiConfig;

		public ItemData ItemToPickUp { private set; get; }

		public PickUpAction(ActorData actorData, float energyCost, ItemData itemToPickUp, IActionEffectFactory actionEffectFactory, 
			IEntitySpawner entitySpawner, IEntityRemover entityRemover, IUiConfig uiConfig) 
			: base(actorData, energyCost, actionEffectFactory)
		{
			ItemToPickUp = itemToPickUp;
			_entitySpawner = entitySpawner;
			_entityRemover = entityRemover;
			_uiConfig = uiConfig;
		}

		public override IEnumerable<IActionEffect> Execute()
		{
			//if (ItemToPickUp.ItemType == ItemType.Weapon)
			//{
			//	WeaponDefinition previousWeapon = ActorData.WeaponDefinition;
			//	ActorData.WeaponDefinition = ItemToPickUp.WeaponDefinition;
			//
			//	IActionEffect effect = ActionEffectFactory.CreateLambdaEffect(() =>
			//	{
			//		_entitySpawner.SpawnWeapon(previousWeapon, ActorData.LogicalPosition);
			//		ActorData.Entity.GetComponentInChildren<WeaponAnimator>().Awake();
			//
			//		_entityRemover.RemoveItem(ItemToPickUp);
			//	});
			//
			//	yield return effect;
			//}
			//else
			{
				_uiConfig.ItemHolder.AddItem(ItemToPickUp.ItemDefinition);
				_entityRemover.RemoveItem(ItemToPickUp);
			}
			yield break;
		}
	}
}
