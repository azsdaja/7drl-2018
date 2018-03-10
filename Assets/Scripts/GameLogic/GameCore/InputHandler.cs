using System;
using System.Collections.Generic;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.GameLogic.GameCore
{
	public class InputHandler : MonoBehaviour
	{
		private IInputHolder _inputHolder;
		private IGameContext _gameContext;
		private IUiConfig _uiConfig;
		private IArrowsVisibilityManager _arrowsVisibilityManager;
		private IWeaponColorizer _weaponColorizer;
		private const float InitialTimeLeftToRepeat = .35f;
		private const float RepeatInterval = .06f;
		private float _timeLeftToRepeat = InitialTimeLeftToRepeat;
		private Dictionary<PlayerInputModifier, Color> _modifiersToColors;

		[Inject]
		public void Init(IInputHolder inputHolder, IArrowsVisibilityManager arrowsVisibilityManager, 
			IWeaponColorizer weaponColorizer, IGameContext gameContext, IUiConfig uiConfig)
		{
			_inputHolder = inputHolder;
			_arrowsVisibilityManager = arrowsVisibilityManager;
			_weaponColorizer = weaponColorizer;
			_gameContext = gameContext;
			_uiConfig = uiConfig;
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

			//if(Input.anyKeyDown && !(Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3) 
			//	|| Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.E))) // jak zmieniasz E, to w obu miejscach!
			//{
			//	_uiConfig.ItemHolder.DeselectItem();
			//	_uiConfig.TooltipPresenter.Panel.gameObject.SetActive(false);
			//}

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
			else if (Input.GetKeyDown(KeyCode.E)) // jak zmieniasz E, to w obu miejscach!
			{
				_inputHolder.PlayerInput = PlayerInput.UseCurrentItem;
			}
			else if (KeyDownOrRepeating(KeyCode.Alpha1))
			{
				SelectItem(0);
			}
			else if (KeyDownOrRepeating(KeyCode.Alpha2))
			{
				SelectItem(1);
			}
			else if (KeyDownOrRepeating(KeyCode.Alpha3))
			{
				SelectItem(2);
			}
			else if (KeyDownOrRepeating(KeyCode.Alpha4))
			{
				SelectItem(3);
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
			else if (KeyDownOrRepeating(KeyCode.Keypad3) || 
				(KeyDownOrRepeating(KeyCode.Comma) && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
				)
			{
				_inputHolder.PlayerInput = PlayerInput.MoveDownRight;
			}
		}

		private void SelectItem(int index)
		{
			ItemDefinition selectedItem = _uiConfig.ItemHolder.SelectItem(index);
			if (selectedItem != null)
			{
				_uiConfig.TooltipPresenter.Present(selectedItem);
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
