using Assets.Scripts.GameLogic.ActionLoop.Actions;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop
{
	public interface IActionFactory
	{
		IGameAction CreateDisplaceAction(ActorData actorData, ActorData actorAtTargetPosition);
		IGameAction CreateAttackAction(ActorData actorData, ActorData attackedActor);
		IGameAction CreateMoveAction(ActorData actorData, Vector2Int actionVector);
		IGameAction CreateDropAction(ActorData actorData, ItemData firstItem);
		IGameAction CreatePickUpAction(ActorData actorData, ItemData itemToPickUp);
		IGameAction CreateCatchAction(ActorData actorData, ActorData caughtActor);
		IGameAction CreateReleaseAction(ActorData actorData);
		IGameAction CreateEatAction(ActorData actorData, ItemData foodItem);
		IGameAction CreateEatEnvironmentAction(ActorData actorData);
		IGameAction CreatePassAction(ActorData actorData);
		IGameAction CreateCallAction(ActorData actorData);
		IGameAction CreateStandUpAction(ActorData actorData);
	}
}