using UnityEngine;

namespace Assets.Scripts.GameLogic.Animation
{
	public interface IEntityAnimator
	{
		bool IsAnimating { get; }
		void MoveTo(Vector2Int sourceLogicalPosition, Vector2Int targetLogicalPosition);
		void Bump(Vector2Int sourceLogicalPosition, Vector2Int affectedPosition);
		void FallIn(Vector2Int sourceLogicalPosition, GameEntity targetEntity);
		void FallOut(Vector2Int sourceLogicalPosition, Vector2Int targetLogicalPosition);
		void KnockOut();
		void StandUp();
	}
}