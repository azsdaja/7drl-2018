using System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.GameLogic.GameCore
{
	public class InputHandler : MonoBehaviour
	{
		private IInputHolder _inputHolder;
		private IArrowsVisibilityManager _arrowsVisibilityManager;
		private const float InitialTimeLeftToRepeat = .35f;
		private const float RepeatInterval = .06f;
		private float _timeLeftToRepeat = InitialTimeLeftToRepeat;

		[Inject]
		public void Init(IInputHolder inputHolder, IArrowsVisibilityManager arrowsVisibilityManager)
		{
			_inputHolder = inputHolder;
			_arrowsVisibilityManager = arrowsVisibilityManager;
		}

		public void SetInputModifier(int modifierIntValue)
		{
			_inputHolder.PlayerInputModifier = (PlayerInputModifier)modifierIntValue;
			_arrowsVisibilityManager.Show();
		}

		public void Update()
		{
			if (!Input.anyKey && !Input.anyKeyDown)
			{
				_timeLeftToRepeat = InitialTimeLeftToRepeat;
				return;
			}

			if (Input.GetKeyDown(KeyCode.M))
			{
				_inputHolder.PlayerInputModifier = PlayerInputModifier.Move;
				_arrowsVisibilityManager.Show();
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
			else if (KeyDownOrRepeating(KeyCode.UpArrow) || KeyDownOrRepeating(KeyCode.Keypad8))
			{
				_inputHolder.PlayerInput = PlayerInput.MoveUp;
			}
			else if (KeyDownOrRepeating(KeyCode.LeftArrow) || KeyDownOrRepeating(KeyCode.Keypad4))
			{
				_inputHolder.PlayerInput = PlayerInput.MoveLeft;
			}
			else if (KeyDownOrRepeating(KeyCode.DownArrow) || KeyDownOrRepeating(KeyCode.Keypad2))
			{
				_inputHolder.PlayerInput = PlayerInput.MoveDown;
			}
			else if (KeyDownOrRepeating(KeyCode.RightArrow) || KeyDownOrRepeating(KeyCode.Keypad6))
			{
				_inputHolder.PlayerInput = PlayerInput.MoveRight;
			}
			else if (KeyDownOrRepeating(KeyCode.Keypad7))
			{
				_inputHolder.PlayerInput = PlayerInput.MoveUpLeft;
			}
			else if (KeyDownOrRepeating(KeyCode.Keypad9))
			{
				_inputHolder.PlayerInput = PlayerInput.MoveUpRight;
			}
			else if (KeyDownOrRepeating(KeyCode.Keypad1))
			{
				_inputHolder.PlayerInput = PlayerInput.MoveDownLeft;
			}
			else if (KeyDownOrRepeating(KeyCode.Keypad3))
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
