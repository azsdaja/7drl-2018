using System.Collections.Generic;
using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;

namespace Assets.Scripts.GameLogic.ActionLoop.Actions
{
	public class PickUpAction : GameAction
	{
		public ItemData ItemToPickUp { private set; get; }

		public PickUpAction(ActorData actorData, float energyCost, ItemData itemToPickUp, IActionEffectFactory actionEffectFactory) 
			: base(actorData, energyCost, actionEffectFactory)
		{
			ItemToPickUp = itemToPickUp;
		}

		public override IEnumerable<IActionEffect> Execute()
		{

			ActorData.Items.Add(ItemToPickUp);
			ItemToPickUp.LogicalPosition = Vector2IntUtilities.Min;

			IActionEffect effect = ActionEffectFactory.CreateLambdaEffect(() => ItemToPickUp.Entity.Hide());

			yield return effect;
		}
	}
}
