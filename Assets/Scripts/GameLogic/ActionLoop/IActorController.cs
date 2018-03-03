namespace Assets.Scripts.GameLogic.ActionLoop
{
	public interface IActorController
	{
		/// <returns>True, when control should be switched no next actor (when the actor is regaining energy or he performed an action).
		/// False, when control should remain at the current actor (when action hasn't been resolved yet or when action was invalid).</returns>
		bool GiveControl(ActorData actorData);
	}
}