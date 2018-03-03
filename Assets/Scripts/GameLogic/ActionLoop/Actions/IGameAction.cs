using System.Collections.Generic;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;

namespace Assets.Scripts.GameLogic.ActionLoop.Actions
{
	public interface IGameAction
	{
		ActorData ActorData { get; }
		float EnergyCost { get; }
		IEnumerable<IActionEffect> Execute();
	}
}