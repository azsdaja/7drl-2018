using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameCore;
using TMPro;
using UnityEngine;
using Zenject;

public class MaxSwordsModifiersPresenter : MonoBehaviour
{
	private IGameContext _gameContext;
	private ActorData _playerActor;
	private TextMeshProUGUI _text;

	[Inject]
	public void Init(IGameContext gameContext)
	{
		_gameContext = gameContext;
		_text = GetComponent<TextMeshProUGUI>();
	}

	// Update is called once per frame
	void Update ()
	{
		if (_playerActor == null)
		{
			_playerActor = _gameContext.PlayerActor.ActorData;
			return;
		}
		int fromTraits = 0;
		if (_playerActor.WeaponWeld.WeaponDefinition.AllowsFarCombat)
		{
			if (_playerActor.Traits.Contains(Trait.LongWeaponsExpert))
				++fromTraits;
			if (_playerActor.Traits.Contains(Trait.LongWeaponsMaster))
				++fromTraits;
		}
		else
		{
			if (_playerActor.Traits.Contains(Trait.ShortWeaponsExpert))
				++fromTraits;
			if (_playerActor.Traits.Contains(Trait.ShortWeaponsMaster))
				++fromTraits;
		}

		string pattern = "Max <sprite=0> modifiers:\n" +
		              "Base: 1<sprite=0>\n" +
		              "{0}" +
		              "Weapon: +{1}<sprite=0>\n" +
		              "{2}" +
		              "{3}" +
		              "{4}";
		_text.text = String.Format(pattern,
			fromTraits == 0? "" : "Traits: " + fromTraits + "<sprite=0>\n",
			_playerActor.WeaponWeld.WeaponDefinition.Swords,
			_playerActor.HealthProgress >= .5f ? "" : "<color=#ff0000>Wounded</color>: -1<sprite=0>\n",
			!_playerActor.Traits.Contains(Trait.Nimble) && _playerActor.HasLittleSpace ? "<color=#ff0000>Tight area</color>: -1<sprite=0>\n" : "",
			_playerActor.IsInCloseCombat && _playerActor.WeaponWeld.WeaponDefinition.CloseCombatModifier !=0 
			? "<color=#ff0000>Close combat penalty</color>: "+ _playerActor.WeaponWeld.WeaponDefinition.CloseCombatModifier 
																+ "<sprite=0>\n" 
			: ""
			);
	}
}
