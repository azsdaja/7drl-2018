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

	public List<TraitHolder> AllHolders;
	public List<TraitHolder> HoldersAvailable;
	public List<TraitHolder> OwnedHolders;
	public Image Picker;
	public TextMeshProUGUI TraitDescriptor;
	public Text AdvanceHeader;
	public Text AdvanceFooter;

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

		if (!AllHolders.Any())
			return;

		if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Keypad2))
		{
			if (_currentHolderIndex + 1 < AllHolders.Count)
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
		//if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Keypad6))
		//{
		//	if (_currentHolderIndex + 5 < Holders.Count)
		//	{
		//		_currentHolderIndex += 5;
		//	}
		//}
		//
		//if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Keypad4))
		//{
		//	if (_currentHolderIndex - 5 > 0)
		//	{
		//		_currentHolderIndex -= 5;
		//	}
		//}

		TraitHolder currentHolder = AllHolders[_currentHolderIndex];
		Picker.transform.localPosition = AllHolders[_currentHolderIndex].transform.localPosition + new Vector3(-.3f, 0, 0);
		TraitDescriptor.text = AllHolders[_currentHolderIndex].Description.Replace(@"\r\n", "\r\n");
		if (OwnedHolders.Contains(currentHolder))
		{
			AdvanceFooter.text = "Owned";
			AdvanceFooter.color = Color.green;
		}
		else if (!HoldersAvailable.Contains(currentHolder))
		{
			AdvanceFooter.text = "Needs unlocking";
			AdvanceFooter.color = Color.red;
		}
		else
		{
			AdvanceFooter.text = "Press enter to acquire";
			AdvanceFooter.color = Color.blue;
		}

		if (!OwnedHolders.Contains(currentHolder) && HoldersAvailable.Contains(currentHolder) 
			&& (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)))
		{
			currentHolder.GetComponent<Image>().color = Color.gray;
			ActorData playerActor = _gameContext.PlayerActor.ActorData;
			var chosenTrait = currentHolder.Trait;
			switch (chosenTrait)
			{
				case Trait.Accurate:
					playerActor.Accuracy += .1f;
					break;
				case Trait.Tough:
					playerActor.MaxHealth = (int)(playerActor.MaxHealth * 1.3f);
					playerActor.Health = (int)(playerActor.Health * 1.3f);
					TraitHolder tough2Holder = AllHolders.First(h => h.Trait == Trait.Tough2);
					tough2Holder.GetComponent<Image>().color = Color.white;
					HoldersAvailable.Add(tough2Holder);
					break;
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
				case Trait.ShortWeaponsExpert:
					TraitHolder shortWeaponsMasterHolder = AllHolders.First(h => h.Trait == Trait.ShortWeaponsMaster);
					shortWeaponsMasterHolder.GetComponent<Image>().color = Color.white;
					HoldersAvailable.Add(shortWeaponsMasterHolder);
					break;
				case Trait.LongWeaponsExpert:
					TraitHolder longWeaponsMasterHolder = AllHolders.First(h => h.Trait == Trait.LongWeaponsMaster);
					longWeaponsMasterHolder.GetComponent<Image>().color = Color.white;
					HoldersAvailable.Add(longWeaponsMasterHolder);
					break;
				default: break;
			}
			playerActor.Traits.Add(chosenTrait);
			HoldersAvailable.RemoveAll(holder => holder.Trait == chosenTrait);
			OwnedHolders.Add(currentHolder);
			currentHolder.GetComponent<Image>().color = new Color(0.3f, 0.7f, 0.3f);
			_gameContext.ControlBlocked = false;
			gameObject.SetActive(false);
			_currentHolderIndex = 0;
		}
	}
}
