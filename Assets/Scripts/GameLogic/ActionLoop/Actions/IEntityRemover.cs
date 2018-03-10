namespace Assets.Scripts.GameLogic.ActionLoop.Actions
{
	public interface IEntityRemover
	{
		void CleanSceneAndGameContextAfterDeath(ActorData actorData);
		void RemoveItem(ItemData item);
	}
}