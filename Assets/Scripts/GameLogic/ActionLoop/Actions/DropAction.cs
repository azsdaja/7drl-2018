using System.Collections.Generic;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;

namespace Assets.Scripts.GameLogic.ActionLoop.Actions
{
	public class DropAction : GameAction
	{
		public ItemData ItemToDrop { private set; get; }

		public DropAction(ActorData actorData, float energyCost, ItemData itemToDrop, IActionEffectFactory actionEffectFactory) 
			: base(actorData, energyCost, actionEffectFactory)
		{
			ItemToDrop = itemToDrop;
		}

		public override IEnumerable<IActionEffect> Execute()
		{
			ActorData.Items.Remove(ItemToDrop);
			ItemToDrop.LogicalPosition = ActorData.LogicalPosition;

			yield return ActionEffectFactory.CreateLambdaEffect(() =>
			{
				ItemToDrop.Entity.RefreshWorldPosition();
				ItemToDrop.Entity.Show();
			});
		}

	
	}
}
