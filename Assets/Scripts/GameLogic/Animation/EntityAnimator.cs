using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.Pathfinding;
using UnityEngine;
using Zenject;

// todo: refactor to share code for similar animations
namespace Assets.Scripts.GameLogic.Animation
{
	/// <summary>
	/// Performs animations related to operations on the transform, e.g. moving, bumping.
	/// </summary>
	public class EntityAnimator : MonoBehaviour, IEntityAnimator
	{
		private IGridInfoProvider _gridInfoProvider;

		private const float DefaultAnimationDuration = .12f;
		private const float FallInAnimationDuration = .3f;
		private float _timeSinceStartedAnimation;
		[SerializeField, HideInInspector] private Vector2Int _previousLogicalPosition;
		[SerializeField, HideInInspector] private Vector2Int _animationTargetPosition;
		[SerializeField, HideInInspector] private Vector2Int _animationAffectedPosition;
		[SerializeField, HideInInspector] private Quaternion _animationStartRotation;

		private EntityAnimationState _animationState;
		private GameEntity _entityToBecomeParent;
		public bool IsAnimating { get { return _animationState != EntityAnimationState.Inactive; } }
	
		[Inject]
		public void Init(IGridInfoProvider gridInfoProvider)
		{
			_gridInfoProvider = gridInfoProvider;
		}

		public void MoveTo(Vector2Int sourceLogicalPosition, Vector2Int targetLogicalPosition)
		{
			SnapToLastTargetIfStillAnimating();
			_previousLogicalPosition = sourceLogicalPosition;
			_animationTargetPosition = targetLogicalPosition;
			_animationState = EntityAnimationState.Moving;
		}

		public void Bump(Vector2Int sourceLogicalPosition, Vector2Int affectedPosition)
		{
			SnapToLastTargetIfStillAnimating();
			_previousLogicalPosition = sourceLogicalPosition;
			_animationAffectedPosition = affectedPosition;
			_animationState = EntityAnimationState.Bumping;
		}

		public void FallIn(Vector2Int sourceLogicalPosition, GameEntity targetEntity)
		{
			SnapToLastTargetIfStillAnimating();
			_previousLogicalPosition = sourceLogicalPosition;
			_animationStartRotation = transform.rotation;
			_animationTargetPosition = targetEntity.EntityData.LogicalPosition;
			_entityToBecomeParent = targetEntity;
			GetComponent<SpriteRenderer>().sortingOrder += 1;
			_animationState = EntityAnimationState.FallingIn;
		}

		public void FallOut(Vector2Int sourceLogicalPosition, Vector2Int targetLogicalPosition)
		{
			SnapToLastTargetIfStillAnimating();
			_previousLogicalPosition = sourceLogicalPosition;
			_animationStartRotation = transform.rotation;
			_animationTargetPosition = targetLogicalPosition;
			_animationState = EntityAnimationState.FallingOut;
		}

		public void KnockOut()
		{
			SnapToLastTargetIfStillAnimating();
			_animationStartRotation = transform.rotation;
			_animationState = EntityAnimationState.BeingKnockedOut;
		}

		public void StandUp()
		{
			SnapToLastTargetIfStillAnimating();
			_animationStartRotation = transform.rotation;
			_animationState = EntityAnimationState.StandingUp;
		}

		void Update()
		{
			if (_animationState == EntityAnimationState.Inactive)
			{
				return;
			}
			if (_animationState == EntityAnimationState.Moving)
			{
				_timeSinceStartedAnimation += Time.smoothDeltaTime; // deltaTime could be unnaturally big 
				// because of calculations done when player performs an action

				Vector3 previousWorldPosition = _gridInfoProvider.GetCellCenterWorld(_previousLogicalPosition);
				Vector3 animationTargetPosition = _gridInfoProvider.GetCellCenterWorld(_animationTargetPosition);
				Vector3 interpolatedPosition 
					= Vector3.Lerp(previousWorldPosition, animationTargetPosition, _timeSinceStartedAnimation / DefaultAnimationDuration);
				transform.position = interpolatedPosition;
				bool finished = _timeSinceStartedAnimation > DefaultAnimationDuration;
				if (finished)
				{
					_animationState = EntityAnimationState.Inactive;
					_timeSinceStartedAnimation = 0f;
				}
			}
			else if (_animationState == EntityAnimationState.FallingIn)
			{
				_timeSinceStartedAnimation += Time.smoothDeltaTime; // deltaTime could be unnaturally big 
				// because of calculations done when player performs an action

				Vector3 previousWorldPosition = _gridInfoProvider.GetCellCenterWorld(_previousLogicalPosition);
				Vector3 animationTargetPosition = _gridInfoProvider.GetCellCenterWorld(_animationTargetPosition);
				float progress = _timeSinceStartedAnimation / FallInAnimationDuration;
				Vector3 interpolatedPosition
					= Vector3.Lerp(previousWorldPosition, animationTargetPosition, progress);
				Vector3 interpolatedScale  = Vector3.Lerp(new Vector3(1,1,1), new Vector3(0.5f, 0.5f, 0.5f), progress);
				Quaternion interpolatedRotation  = Quaternion.Lerp(_animationStartRotation, Quaternion.Euler(0, 0, 180), progress);
				transform.position = interpolatedPosition;
				transform.localScale = interpolatedScale;
				transform.rotation = interpolatedRotation;
				bool finished = progress >= 1;
				if (finished)
				{
					transform.parent = _entityToBecomeParent.transform;
					_animationState = EntityAnimationState.Inactive;
					_timeSinceStartedAnimation = 0f;
					_entityToBecomeParent = null;
				}
			}
			else if (_animationState == EntityAnimationState.FallingOut)
			{
				_timeSinceStartedAnimation += Time.smoothDeltaTime; // deltaTime could be unnaturally big 
				// because of calculations done when player performs an action

				Vector3 previousWorldPosition = _gridInfoProvider.GetCellCenterWorld(_previousLogicalPosition);
				Vector3 animationTargetPosition = _gridInfoProvider.GetCellCenterWorld(_animationTargetPosition);
				float progress = _timeSinceStartedAnimation / FallInAnimationDuration;
				Vector3 interpolatedPosition
					= Vector3.Lerp(previousWorldPosition, animationTargetPosition, progress);
				Vector3 interpolatedScale = Vector3.Lerp(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1, 1, 1), progress);
				Quaternion interpolatedRotation = Quaternion.Lerp(_animationStartRotation, Quaternion.identity, progress);
				transform.position = interpolatedPosition;
				transform.localScale = interpolatedScale;
				transform.rotation = interpolatedRotation;
				bool finished = progress >= 1;
				if (finished)
				{
					_animationState = EntityAnimationState.Inactive;
					GetComponent<SpriteRenderer>().sortingOrder -= 1;
					_timeSinceStartedAnimation = 0f;
					_entityToBecomeParent = null;
				}
			}
			else if (_animationState == EntityAnimationState.Bumping)
			{
				_timeSinceStartedAnimation += Time.smoothDeltaTime;
				Vector3 previousWorldPosition = _gridInfoProvider.GetCellCenterWorld(_previousLogicalPosition);
				Vector3 middleWayToTarget = (previousWorldPosition + _gridInfoProvider.GetCellCenterWorld(_animationAffectedPosition)) / 2;
				Vector3 interpolatedPosition
					= Vector3Utilities.LerpThereAndBack(previousWorldPosition, middleWayToTarget, _timeSinceStartedAnimation / DefaultAnimationDuration);
				transform.position = interpolatedPosition;
				bool finished = _timeSinceStartedAnimation > DefaultAnimationDuration;
				if (finished)
				{
					_animationState = EntityAnimationState.Inactive;
					_timeSinceStartedAnimation = 0f;
				}
			}
			else if (_animationState == EntityAnimationState.BeingKnockedOut)
			{
				_timeSinceStartedAnimation += Time.smoothDeltaTime; // deltaTime could be unnaturally big 
				// because of calculations done when player performs an action
				float progress = _timeSinceStartedAnimation / DefaultAnimationDuration;
				Quaternion interpolatedRotation = Quaternion.Lerp(_animationStartRotation, Quaternion.Euler(0, 0, 180), progress);
				transform.rotation = interpolatedRotation;
				bool finished = progress >= 1;
				if (finished)
				{
					_animationState = EntityAnimationState.Inactive;
					_timeSinceStartedAnimation = 0f;
				}
			}
			else if (_animationState == EntityAnimationState.StandingUp)
			{
				_timeSinceStartedAnimation += Time.smoothDeltaTime; // deltaTime could be unnaturally big 
				// because of calculations done when player performs an action
				float progress = _timeSinceStartedAnimation / DefaultAnimationDuration;
				Quaternion interpolatedRotation = Quaternion.Lerp(_animationStartRotation, Quaternion.identity, progress);
				transform.rotation = interpolatedRotation;
				bool finished = progress >= 1;
				if (finished)
				{
					_animationState = EntityAnimationState.Inactive;
					_timeSinceStartedAnimation = 0f;
				}
			}
		}

		private void SnapToLastTargetIfStillAnimating()
		{
			if (_animationState != EntityAnimationState.Inactive)
			{
				transform.position = _gridInfoProvider.GetCellCenterWorld(_animationTargetPosition);
				_previousLogicalPosition = _animationTargetPosition;
			}
		}
	}
}