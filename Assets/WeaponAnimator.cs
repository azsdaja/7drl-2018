using System;
using System.Collections;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.Pathfinding;
using Assets.Scripts.RNG;
using UnityEngine;
using Zenject;

public class WeaponAnimator : MonoBehaviour
{
	private Animator _animator;
	private WeaponAnimationData _weaponAnimationData;
	private SpriteRenderer _weaponSprite;
	private IGridInfoProvider _gridInfoProvider;
	private IWeaponColorizer _weaponColorizer;
	private IRandomNumberGenerator _rng;
	private Vector3 _initialPosition;
	private Quaternion _initialRotation;
	private float _timeSinceBeginning;
	private Vector3 _mySwingTarget;
	private Quaternion _mySwingTargetRotation;
	private bool _isAggressiveAttack;
	private AnimationCurve _deviationCurveForX;
	private AnimationCurve _deviationCurveForY;
	private float _deviationImportanceForX;
	private float _deviationImportanceForY;
	private AnimationCurve _deviationCurveForRotation;
	private IEnumerator _createSparksCoroutine;

	public SpriteRenderer WeaponSprite
	{
		get { return _weaponSprite; }
	}

	[Inject]
	public void Init(IGridInfoProvider gridInfoProvider, IWeaponColorizer weaponColorizer, IRandomNumberGenerator rng)
	{
		_gridInfoProvider = gridInfoProvider;
		_weaponColorizer = weaponColorizer;
		_rng = rng;
	}

	public void Awake()
	{
		_animator = GetComponent<Animator>();
		WeaponDefinition usedWeapon = transform.parent.GetComponent<ActorBehaviour>().ActorData.WeaponWeld.WeaponDefinition;
		_weaponAnimationData = usedWeapon.WeaponAnimationData;
		_weaponSprite = GetComponent<SpriteRenderer>();
		WeaponSprite.sprite = usedWeapon.Sprite;
	}

	void Update()
	{
		if (_animator.enabled) return;

		_timeSinceBeginning += Time.deltaTime;
		float animationDuration = _isAggressiveAttack ? _weaponAnimationData.AggressiveAnimationDuration : _weaponAnimationData.NormalAnimationDuration;
		float progress = _timeSinceBeginning / animationDuration;
		
		if (progress > 1f)
		{
			_weaponColorizer.Decolorize(this);
			_timeSinceBeginning = 0f;
			_animator.enabled = true;
		}
		AnimationCurve movementCurve = _isAggressiveAttack ? _weaponAnimationData.AggressiveMovementCurve : _weaponAnimationData.NormalMovementCurve;

		Vector3 positionWithoutDeviation = Vector3.LerpUnclamped(_initialPosition, _mySwingTarget, movementCurve.Evaluate(progress));

		if (_deviationCurveForX != null)
		{
			var positionAfterDeviation = positionWithoutDeviation
			                             + new Vector3(
				                             _deviationCurveForX.Evaluate(progress) * _deviationImportanceForX,
				                             _deviationCurveForY.Evaluate(progress) * _deviationImportanceForY,
				                             0);

			transform.position = positionAfterDeviation;

			Quaternion rotationWithoutDeviation = Quaternion.Lerp(_initialRotation, _mySwingTargetRotation,
				_weaponAnimationData.NormalMovementCurve.Evaluate(progress));
			float eulerZDeviation = 360 * _deviationCurveForRotation.Evaluate(progress);
			var withDeviation = Quaternion.Euler(rotationWithoutDeviation.eulerAngles.x, rotationWithoutDeviation.eulerAngles.y,
				rotationWithoutDeviation.eulerAngles.z + eulerZDeviation);
			transform.localRotation = withDeviation;

		}
		else
		{
			transform.position = positionWithoutDeviation;
			transform.localRotation = Quaternion.Lerp(_initialRotation, _mySwingTargetRotation, _weaponAnimationData.NormalMovementCurve.Evaluate(progress));
		}

	}
	
	public void SwingTo(Vector2Int targetPosition, bool isAggressiveAttack)
	{
		_initialPosition = transform.position;
		_isAggressiveAttack = isAggressiveAttack;
		_mySwingTarget = _gridInfoProvider.GetCellCenterWorld(targetPosition);
		_animator.enabled = false;
		Vector3 directionToTarget = _mySwingTarget - _initialPosition;

		_deviationCurveForX = _rng.Choice(_weaponAnimationData.DeviationCurves);
		_deviationCurveForY = _rng.Choice(_weaponAnimationData.DeviationCurves);
		_deviationCurveForRotation = _rng.Choice(_weaponAnimationData.DeviationCurvesForRotation);
		_deviationImportanceForX = _rng.NextFloat();
		_deviationImportanceForY = _rng.NextFloat();

		var angle = ZRotationForLookAt2DConsidering45OffsetOfWeapon(Math.Abs(directionToTarget.x), directionToTarget.y);
		_mySwingTargetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
		
	}

	public static float ZRotationForLookAt2DConsidering45OffsetOfWeapon(float x, float y)
	{
		var angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
		return angle - 90 + 45;
	}

	public void DefendSwing(Transform enemyWeaponTransform, Vector2Int targetPosition)
	{
		_initialPosition = transform.position;
		_initialRotation = transform.localRotation;

		Vector3 enemySwingTarget = _gridInfoProvider.GetCellCenterWorld(targetPosition);
		_mySwingTarget = (enemyWeaponTransform.position + enemySwingTarget) * .5f;
		Quaternion enemyWeaponRotation = enemyWeaponTransform.localRotation;
		int rotationToUse = 90;

		// should give enemy weapon z rotation plus rotationToUse degrees
		float zRotation = (((enemyWeaponRotation.eulerAngles.z + rotationToUse) + 180) % 360) - 180; 

		_mySwingTargetRotation = Quaternion.Euler(_initialRotation.eulerAngles.x, _initialRotation.eulerAngles.y, zRotation);

		_createSparksCoroutine = CreateSparks(_mySwingTarget);
		StartCoroutine(_createSparksCoroutine);

		_animator.enabled = false;
	}

	private IEnumerator CreateSparks(Vector3 position)
	{
		yield return new WaitForSeconds(0.1f);

		Animator sparks = Resources.Load<Animator>("Prefabs/Sparks");
		Animator sparksObject = GameObject.Instantiate(sparks, position, Quaternion.identity);
		sparksObject.Play("Sparks");
		GameObject.Destroy(sparksObject.gameObject, .2f);
	}
}
