using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.Pathfinding;
using UnityEngine;
using Zenject;

public class WeaponAnimator : MonoBehaviour
{
	private Animator _animator;
	private WeaponAnimationData _weaponAnimationData;
	private SpriteRenderer _weaponSprite;
	private IGridInfoProvider _gridInfoProvider;
	private IWeaponColorizer _weaponColorizer;
	private Vector3 _initialPosition;
	private Quaternion _initialRotation;
	private float _timeSinceBeginning;
	private Vector3 _mySwingTarget;
	private Quaternion _mySwingTargetRotation;
	private bool _isAggressiveAttack;

	public SpriteRenderer WeaponSprite
	{
		get { return _weaponSprite; }
	}

	[Inject]
	public void Init(IGridInfoProvider gridInfoProvider, IWeaponColorizer weaponColorizer)
	{
		_gridInfoProvider = gridInfoProvider;
		_weaponColorizer = weaponColorizer;
	}

	void Awake()
	{
		_animator = GetComponent<Animator>();
		Weapon usedWeapon = transform.parent.GetComponent<ActorBehaviour>().ActorData.Weapon;
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
		
		transform.position = Vector3.LerpUnclamped(_initialPosition, _mySwingTarget, movementCurve.Evaluate(progress));
		transform.rotation = Quaternion.Lerp(_initialRotation, _mySwingTargetRotation, _weaponAnimationData.NormalMovementCurve.Evaluate(progress));
	}
	
	public void SwingTo(Vector2Int targetPosition, bool isAggressiveAttack)
	{
		_initialPosition = transform.position;
		_isAggressiveAttack = isAggressiveAttack;
		_mySwingTarget = _gridInfoProvider.GetCellCenterWorld(targetPosition);
		_animator.enabled = false;
		Vector3 directionToTarget = _mySwingTarget - _initialPosition;
		_mySwingTargetRotation = Quaternion.LookRotation(directionToTarget);
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
