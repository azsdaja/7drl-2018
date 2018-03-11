using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameCore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AdvanceManager : MonoBehaviour
{
	private IGameContext _gameContext;
	private IUiConfig _uiConfig;

	private int _currentHolderIndex;

	public List<TraitHolder> Holders;
	public Image Picker;
	public TextMeshProUGUI TraitDescriptor;
	public Text AdvanceHeader;

	[Inject]
	public void Init(IGameContext gameContext, IUiConfig uiConfig)
	{
		_gameContext = gameContext;
		_uiConfig = uiConfig;
	}

	// Use this for initialization
	void Start()
	{
		_currentHolderIndex = 0;
	}

	// Update is called once per frame
	void Update()
	{
		AdvanceHeader.text = "You advance to level " + _gameContext.PlayerActor.ActorData.Level;
		_gameContext.ControlBlocked = true;

		if (!Holders.Any())
			return;

		if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Keypad2))
		{
			if (_currentHolderIndex + 1 < Holders.Count)
			{
				++_currentHolderIndex;
			}
		}

		if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Keypad8))
		{
			if (_currentHolderIndex - 1 >= 0)
			{
				--_currentHolderIndex;
			}
		}

		Picker.transform.localPosition = Holders[_currentHolderIndex].transform.localPosition + new Vector3(-.3f, 0, 0);
		TraitDescriptor.text = Holders[_currentHolderIndex].Description.Replace(@"\r\n", "\r\n");

		if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
		{
			Holders[_currentHolderIndex].GetComponent<Image>().color = Color.gray;
			ActorData playerActor = _gameContext.PlayerActor.ActorData;
			var chosenTrait = Holders[_currentHolderIndex].Trait;
			switch (chosenTrait)
			{
				case Trait.Accurate:
					playerActor.Accuracy += .1f;
					break;
				case Trait.Fencer:
				case Trait.Fencer2:
					playerActor.SwordsFromSkill += 1;
					break;
				case Trait.Tough:
				case Trait.Tough2:
					playerActor.MaxHealth = (int) (playerActor.MaxHealth * 1.3f);
					playerActor.Health = (int) (playerActor.Health * 1.3f);
					break;
				case Trait.DaringBlow:
					_uiConfig.DaringBlowAbilityButton.gameObject.SetActive(true);
					break;
				case Trait.Push:
					_uiConfig.PushAbilityButton.gameObject.SetActive(true);
					break;
				default: break;
			}
			playerActor.Traits.Add(chosenTrait);
			Holders.RemoveAll(holder => holder.Trait == chosenTrait);
			_gameContext.ControlBlocked = false;
			gameObject.SetActive(false);
			_currentHolderIndex = 0;
		}
	}
}
