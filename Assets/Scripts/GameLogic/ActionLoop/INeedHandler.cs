namespace Assets.Scripts.GameLogic.ActionLoop
{
	public interface INeedHandler
	{
		void Heartbeat(ActorData actorData);
		void ModifyNeed(NeedData needData, NeedType needType, float delta);
		void ModifyNeedWithFactor(NeedData needData, NeedType needType, NeedFactor needFactor, float delta);
	}
}