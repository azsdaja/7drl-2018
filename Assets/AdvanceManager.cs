﻿using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameCore;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AdvanceManager : MonoBehaviour
{
	private IGameContext _gameContext;

	private int _currentHolderIndex;

	public List<TraitHolder> Holders;
	public Image Picker;
	public Text TraitDescriptor;

	[Inject]
	public void Init(IGameContext gameContext)
	{
		_gameContext = gameContext;
		
	}

	// Use this for initialization
	void Start()
	{
		_currentHolderIndex = 0;
	}

	// Update is called once per frame
	void Update()
	{
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
					break;
				default: break;
			}
			playerActor.Traits.Add(chosenTrait);
			Holders.RemoveAll(holder => holder.Trait == chosenTrait);
			_gameContext.ControlBlocked = false;
			gameObject.SetActive(false);
		}
	}
}
