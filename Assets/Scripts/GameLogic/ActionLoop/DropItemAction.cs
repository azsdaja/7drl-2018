using System;
using System.Collections.Generic;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using Assets.Scripts.GameLogic.GameCore;

namespace Assets.Scripts.GameLogic.ActionLoop
{
	public class DropItemAction : GameAction
	{
		private readonly ItemDefinition _item;
		private readonly IEntitySpawner _entitySpawner;
		private readonly IUiConfig _uiConfig;

		public DropItemAction(ActorData actorData, ItemDefinition item, float energyCost, IEntitySpawner entitySpawner, 
			IActionEffectFactory actionEffectFactory, IUiConfig uiConfig) 
			: base(actorData, energyCost, actionEffectFactory)
		{
			_item = item;
			_entitySpawner = entitySpawner;
			_uiConfig = uiConfig;
		}

		public override IEnumerable<IActionEffect> Execute()
		{
			_uiConfig.ItemHolder.RemoveItem(_uiConfig.ItemHolder.SelectedItemIndex);
			_entitySpawner.SpawnItem(_item, ActorData.LogicalPosition);
			yield break;
		}
	}
}