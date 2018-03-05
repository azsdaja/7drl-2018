using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop.ActionEffects
{
	public class ActorAligner
	{
		public void AlignActorToDirection(GameEntity entity, int nextPosition, int originalPosition)
		{
			int xDelta = nextPosition - originalPosition;
			if (xDelta == 0)
			{
			}
			else if (xDelta > 0)
			{
				entity.transform.localScale = new Vector3(1, 1, 1);
			}
			else
			{
				entity.transform.localScale = new Vector3(-1, 1, 1);
			}
		}
	}
}