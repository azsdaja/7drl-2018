﻿using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameLogic.GameCore
{
	public class UiConfig : MonoBehaviour, IUiConfig
	{
		public ProgressBar HealthBar;
		public Canvas TooltipPooled;
		public Button RestartButton;
		public GameObject Arrows;
		public AdvanceManager AdvanceManager;
		public ItemHolder ItemHolder;
		public GameObject CurrentWeaponHolder;
		public TooltipPresenter TooltipPresenter;
		public TooltipPresenter TooltipCurrentWeaponPresenter;
		public GameFinisher GameFinisher;
		public BasherMessage BasherMessage;

		public Button WalkAbilityButton;
		public Button DaringBlowAbilityButton;
		public Button PushAbilityButton;

		Canvas IUiConfig.TooltipPooled{ get { return TooltipPooled; }}
		ProgressBar IUiConfig.HealthBar { get { return HealthBar; }}
		Button IUiConfig.RestartButton { get { return RestartButton; }}
		GameObject IUiConfig.Arrows { get { return Arrows; } }
		AdvanceManager IUiConfig.AdvanceManager { get { return AdvanceManager; } }
		ItemHolder IUiConfig.ItemHolder { get { return ItemHolder; } }
		GameObject IUiConfig.CurrentWeaponHolder { get { return CurrentWeaponHolder; } }
		GameFinisher IUiConfig.GameFinisher { get { return GameFinisher; } }
		TooltipPresenter IUiConfig.TooltipPresenter { get { return TooltipPresenter; } }
		TooltipPresenter IUiConfig.TooltipCurrentWeaponPresenter { get { return TooltipCurrentWeaponPresenter; } }
		BasherMessage IUiConfig.BasherMessage { get { return BasherMessage; } }

		Button IUiConfig.WalkAbilityButton { get { return WalkAbilityButton; } }
		Button IUiConfig.DaringBlowAbilityButton { get { return DaringBlowAbilityButton; } }
		Button IUiConfig.PushAbilityButton { get { return PushAbilityButton; } }
	}
}