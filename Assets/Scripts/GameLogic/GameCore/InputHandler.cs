using System.Collections.Generic;
using Assets.Scripts.CSharpUtilities;
using C5;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

namespace Assets.Scripts.GameLogic.GameCore
{
	public class InputHandler : MonoBehaviour
	{
		private IInputHolder _inputHolder;
		private IGameContext _gameContext;
		private IArrowsVisibilityManager _arrowsVisibilityManager;
		private IWeaponColorizer _weaponColorizer;
		private const float InitialTimeLeftToRepeat = .35f;
		private const float RepeatInterval = .06f;
		private float _timeLeftToRepeat = InitialTimeLeftToRepeat;
		private Dictionary<PlayerInputModifier, Color> _modifiersToColors;

		[Inject]
		public void Init(IInputHolder inputHolder, IArrowsVisibilityManager arrowsVisibilityManager, 
			IWeaponColorizer weaponColorizer, IGameContext gameContext)
		{
			_inputHolder = inputHolder;
			_arrowsVisibilityManager = arrowsVisibilityManager;
			_weaponColorizer = weaponColorizer;
			_gameContext = gameContext;
		}

		void Start()
		{
			_modifiersToColors = new Dictionary<PlayerInputModifier, Color>
			{
				{PlayerInputModifier.Move, Color.green},
				{PlayerInputModifier.Push, Color.yellow},
			};
		}

		public void SetInputModifier(int modifierIntValue)
		{
			_inputHolder.PlayerInputModifier = (PlayerInputModifier)modifierIntValue;

			if (_inputHolder.PlayerInputModifier == PlayerInputModifier.Push ||
			    _inputHolder.PlayerInputModifier == PlayerInputModifier.Move)
			{
				var color = _modifiersToColors[_inputHolder.PlayerInputModifier];
				_arrowsVisibilityManager.Show(color);
			}
		}

		public void Update()
		{
			if (_gameContext.ControlBlocked)
				return;

			if (!Input.anyKey && !Input.anyKeyDown)
			{
				_timeLeftToRepeat = InitialTimeLeftToRepeat;
				return;
			}

			if (Input.GetKeyDown(KeyCode.Escape))
			{
				PlayerInputModifier cancelledInputModifier = _inputHolder.PlayerInputModifier;
				if(cancelledInputModifier == PlayerInputModifier.Move || cancelledInputModifier == PlayerInputModifier.Push)
					_arrowsVisibilityManager.Hide();
				else if(cancelledInputModifier == PlayerInputModifier.DaringBlow)
					_weaponColorizer.Decolorize(_gameContext.PlayerActor.WeaponAnimator);
				_inputHolder.PlayerInputModifier = PlayerInputModifier.None;
			}
			if (Input.GetKeyDown(KeyCode.W))
			{
				_inputHolder.PlayerInputModifier = PlayerInputModifier.Move;
				var naturalGreen = new Color(33, 160, 73);
				_arrowsVisibilityManager.Show(naturalGreen);
			}
			if (Input.GetKeyDown(KeyCode.D))
			{
				ActorData playerData = _gameContext.PlayerActor.ActorData;

				if (playerData.Traits.Contains(Trait.DaringBlow) && playerData.Swords >= 1)
				{
					_inputHolder.PlayerInputModifier = PlayerInputModifier.Push;
					_arrowsVisibilityManager.Show(Color.yellow);
				}
			}
			if (Input.GetKeyDown(KeyCode.A))
			{
				ActorData playerData = _gameContext.PlayerActor.ActorData;
				if (playerData.Traits.Contains(Trait.DaringBlow) && playerData.Swords >= 2)
				{
					_inputHolder.PlayerInputModifier = PlayerInputModifier.DaringBlow;
					_weaponColorizer.Colorize(_gameContext.PlayerActor.WeaponAnimator, Color.red);
				}
			}
			if ((Input.GetKey(KeyCode.LeftShift) | Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.Comma))
			{
				_inputHolder.PlayerInput = PlayerInput.Ascend;
			}
			if (Input.GetKeyDown(KeyCode.G))
			{
				_inputHolder.PlayerInput = PlayerInput.PickUp;
			}
			else if (Input.GetKeyDown(KeyCode.D))
			{
				_inputHolder.PlayerInput = PlayerInput.Drop;
			}
			else if (Input.GetKeyDown(KeyCode.C))
			{
				_inputHolder.PlayerInput = PlayerInput.Catch;
			}
			else if (Input.GetKeyDown(KeyCode.R))
			{
				_inputHolder.PlayerInput = PlayerInput.Release;
			}
			else if (KeyDownOrRepeating(KeyCode.Alpha5))
			{
				_inputHolder.PlayerInput = PlayerInput.Pass;
			}
			else if (KeyDownOrRepeating(KeyCode.E))
			{
				_inputHolder.PlayerInput = PlayerInput.Eat;
			}
			else if (KeyDownOrRepeating(KeyCode.UpArrow) || KeyDownOrRepeating(KeyCode.Keypad8) || KeyDownOrRepeating(KeyCode.U))
			{
				_inputHolder.PlayerInput = PlayerInput.MoveUp;
			}
			else if (KeyDownOrRepeating(KeyCode.LeftArrow) || KeyDownOrRepeating(KeyCode.Keypad4) || KeyDownOrRepeating(KeyCode.H))
			{
				_inputHolder.PlayerInput = PlayerInput.MoveLeft;
			}
			else if (KeyDownOrRepeating(KeyCode.DownArrow) || KeyDownOrRepeating(KeyCode.Keypad2) || KeyDownOrRepeating(KeyCode.M))
			{
				_inputHolder.PlayerInput = PlayerInput.MoveDown;
			}
			else if (KeyDownOrRepeating(KeyCode.RightArrow) || KeyDownOrRepeating(KeyCode.Keypad6) || KeyDownOrRepeating(KeyCode.K))
			{
				_inputHolder.PlayerInput = PlayerInput.MoveRight;
			}
			else if (KeyDownOrRepeating(KeyCode.Keypad7) || KeyDownOrRepeating(KeyCode.Y) || KeyDownOrRepeating(KeyCode.Z))
			{
				_inputHolder.PlayerInput = PlayerInput.MoveUpLeft;
			}
			else if (KeyDownOrRepeating(KeyCode.Keypad9) || KeyDownOrRepeating(KeyCode.I))
			{
				_inputHolder.PlayerInput = PlayerInput.MoveUpRight;
			}
			else if (KeyDownOrRepeating(KeyCode.Keypad1) || KeyDownOrRepeating(KeyCode.N))
			{
				_inputHolder.PlayerInput = PlayerInput.MoveDownLeft;
			}
			else if (KeyDownOrRepeating(KeyCode.Keypad3) || KeyDownOrRepeating(KeyCode.Comma))
			{
				_inputHolder.PlayerInput = PlayerInput.MoveDownRight;
			}
		}

		private bool KeyDownOrRepeating(KeyCode keyCode)
		{
			return Input.GetKeyDown(keyCode) || GetAndUpdateKey(keyCode);
		}

		private bool GetAndUpdateKey(KeyCode keyCode)
		{
			if (Input.GetKey(keyCode))
			{
				_timeLeftToRepeat -= Time.unscaledDeltaTime;
				if (_timeLeftToRepeat < 0)
				{
					_timeLeftToRepeat = RepeatInterval;
					return true;
				}
			}
			return false;
		}
	}
}
