using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Pathfinding;
using UnityEngine;
using Zenject;

public class WeaponAnimator : MonoBehaviour
{
	private Animator _animator;
	private IGridInfoProvider _gridInfoProvider;
	private Vector3 _initialPosition;
	private Quaternion _initialRotation;
	private float _timeSinceBeginning;
	private Vector3 _mySwingTarget;
	private Quaternion _mySwingTargetRotation;

	public float AnimationDuration = 1f;
	public AnimationCurve MovementCurve;

	[Inject]
	public void Init(IGridInfoProvider gridInfoProvider)
	{
		_gridInfoProvider = gridInfoProvider;
	}

	void Start()
	{
		_animator = GetComponent<Animator>();
	}

	void Update()
	{
		if (_animator.enabled) return;

		_timeSinceBeginning += Time.deltaTime;
		float progress = _timeSinceBeginning / AnimationDuration;
		
		if (progress > 1f)
		{
			_timeSinceBeginning = 0f;
			_animator.enabled = true;
		}

		transform.position = Vector3.Lerp(_initialPosition, _mySwingTarget, MovementCurve.Evaluate(progress));
		transform.rotation = Quaternion.Lerp(_initialRotation, _mySwingTargetRotation, MovementCurve.Evaluate(progress));
	}
	
	public void SwingTo(Vector2Int targetPosition)
	{
		_initialPosition = transform.position;
		_animator.enabled = false;
		Vector3 directionToTarget = _mySwingTarget - _initialPosition;
		_mySwingTargetRotation = Quaternion.LookRotation(directionToTarget);
		_mySwingTarget = _gridInfoProvider.GetCellCenterWorld(targetPosition);
	}
	
	public void DefendSwing(Transform enemyWeaponTransform, Vector2Int targetPosition)
	{
		_initialPosition = transform.position;
		_initialRotation = transform.rotation;

		Vector3 enemySwingTarget = _gridInfoProvider.GetCellCenterWorld(targetPosition);
		_mySwingTarget = (enemyWeaponTransform.position + enemySwingTarget) * .5f;
		Quaternion enemyWeaponRotation = enemyWeaponTransform.rotation;
		int rotationToUse = 90;

		// should give enemy weapon z rotation plus rotationToUse degrees
		float zRotation = (((enemyWeaponRotation.eulerAngles.z + rotationToUse) + 180) % 360) - 180; 

		_mySwingTargetRotation = Quaternion.Euler(_initialRotation.eulerAngles.x, _initialRotation.eulerAngles.y, zRotation);
		_animator.enabled = false;
	}
}
