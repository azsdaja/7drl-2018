using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop.AI
{
	public interface INavigator
	{
		Vector2Int? ResolveNextStep(ActorData actorData);
		List<Vector2Int> GetPath(Vector2Int startPosition, Vector2Int targetPosition);
	}
}