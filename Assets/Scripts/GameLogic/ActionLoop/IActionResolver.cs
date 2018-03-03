using Assets.Scripts.GameLogic.ActionLoop.Actions;

namespace Assets.Scripts.GameLogic.ActionLoop
{
	public interface IActionResolver
	{
		IGameAction GetAction(ActorData actorData);
	}
}