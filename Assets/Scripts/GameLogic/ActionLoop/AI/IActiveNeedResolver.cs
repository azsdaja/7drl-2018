namespace Assets.Scripts.GameLogic.ActionLoop.AI
{
	public interface IActiveNeedResolver
	{
		NeedType ResolveActiveNeed(NeedData needData);
	}
}