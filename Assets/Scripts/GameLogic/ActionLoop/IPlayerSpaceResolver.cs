namespace Assets.Scripts.GameLogic.ActionLoop
{
	public interface IPlayerSpaceResolver
	{
		bool ResolveIfPlayerHasLittleSpace(ActorData actorData);
	}
}