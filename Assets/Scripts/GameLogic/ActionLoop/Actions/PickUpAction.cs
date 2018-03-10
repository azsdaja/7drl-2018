using System.Collections.Generic;
using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop.Actions
{
	public class PickUpAction : GameAction
	{
		private readonly IEntitySpawner _entitySpawner;
		private readonly IEntityRemover _entityRemover;

		public ItemData ItemToPickUp { private set; get; }

		public PickUpAction(ActorData actorData, float energyCost, ItemData itemToPickUp, IActionEffectFactory actionEffectFactory, 
			IEntitySpawner entitySpawner, IEntityRemover entityRemover) 
			: base(actorData, energyCost, actionEffectFactory)
		{
			ItemToPickUp = itemToPickUp;
			_entitySpawner = entitySpawner;
			_entityRemover = entityRemover;
		}

		public override IEnumerable<IActionEffect> Execute()
		{
			if (ItemToPickUp.Weapon != null)
			{
				Weapon previousWeapon = ActorData.Weapon;
				ActorData.Weapon = ItemToPickUp.Weapon;

				IActionEffect effect = ActionEffectFactory.CreateLambdaEffect(() =>
				{
					_entitySpawner.SpawnWeapon(previousWeapon, ActorData.LogicalPosition);
					ActorData.Entity.GetComponentInChildren<WeaponAnimator>().Awake();

					_entityRemover.RemoveItem(ItemToPickUp);
				});

				yield return effect;
			}


		}
	}
}
