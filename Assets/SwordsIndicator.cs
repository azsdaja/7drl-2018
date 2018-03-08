using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.GameLogic;
using UnityEngine;
using Zenject;

public class SwordsIndicator : MonoBehaviour
{
	public AnimationCurve _swordAppearCurve;
	private ActorBehaviour _actorBehaviour;
	private int _swordsShown;
	private int _maxSwordsShown;
	private IEnumerator _shrinkCoroutine;
	private IEnumerator _appearCoroutine;
	private IMaxSwordCalculator _maxSwordsCalculator;
	private SpriteRenderer _frameSpriteRenderer;

	public List<SpriteRenderer> SwordsSprites;

	[Inject]
	public void Init(IMaxSwordCalculator maxSwordCalculator)
	{
		_maxSwordsCalculator = maxSwordCalculator;
	}

	// Use this for initialization
	void Awake()
	{
		_actorBehaviour = transform.parent.GetComponent<ActorBehaviour>();
		_frameSpriteRenderer = GetComponent<SpriteRenderer>();

		_maxSwordsShown = _maxSwordsCalculator.Calculate(_actorBehaviour.ActorData);
		_swordsShown = _maxSwordsShown;
		SetFrameSpriteToMaxSwords(_maxSwordsShown);
	}

	// Update is called once per frame

	void Update ()
	{
		int playerMaxSwords = _actorBehaviour.ActorData.MaxSwords;
		if (_maxSwordsShown != playerMaxSwords)
		{
			SetFrameSpriteToMaxSwords(playerMaxSwords);
			_maxSwordsShown = playerMaxSwords;
		}

		int playerSwords = _actorBehaviour.ActorData.Swords;
		if (playerSwords != _swordsShown)
		{
			UpdateActiveSwords(playerSwords);
		}
	}

	private void UpdateActiveSwords(int playerSwords)
	{
		_swordsShown = playerSwords;
		for (int i = 0; i < SwordsSprites.Count; i++)
		{
			if (i < playerSwords)
			{
				if (!SwordsSprites[i].enabled)
				{
					SwordsSprites[i].enabled = true;
					_appearCoroutine = AppearSword(i);
					StartCoroutine(_appearCoroutine);
				}
			}
			else
			{
				if (SwordsSprites[i].enabled)
				{
					_shrinkCoroutine = ShrinkSword(i);
					StartCoroutine(_shrinkCoroutine);
				}
			}
		}
	}

	private IEnumerator ShrinkSword(int index)
	{
		for (float progress = 0f; progress < 1f; progress += .1f)
		{
			SwordsSprites[index].transform.localScale = Vector3.one * (1-progress);
			yield return new WaitForSeconds(.03f);
		}
		SwordsSprites[index].enabled = false;
	}

	private IEnumerator AppearSword(int index)
	{
		for (float progress = 0f; progress < 1f; progress += .1f)
		{
			SwordsSprites[index].transform.localScale = Vector3.one * _swordAppearCurve.Evaluate(progress);
			yield return new WaitForSeconds(.03f);
		}
		SwordsSprites[index].enabled = true;
		SwordsSprites[index].transform.localScale = Vector3.one;
	}

	private void SetFrameSpriteToMaxSwords(int maxSwordsInitial)
	{
		Sprite indicatorSprite = Resources.Load<Sprite>("Sprites/SwordsIndicator" + maxSwordsInitial);
		_frameSpriteRenderer.sprite = indicatorSprite;
	}
}