using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

		var maxSwords = _maxSwordsCalculator.Calculate(_actorBehaviour.ActorData);
		_actorBehaviour.ActorData.MaxSwords = maxSwords;
		_actorBehaviour.ActorData.Swords = maxSwords;
		_maxSwordsShown = maxSwords;
		InitializeActiveSwords(maxSwords);
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
		
		int actorSwords = _actorBehaviour.ActorData.Swords;
		if (actorSwords != _swordsShown)
		{
			UpdateActiveSwords(actorSwords);
		}

		int reallyShown = SwordsSprites.Count(s => s.enabled);
		if (reallyShown != actorSwords)
		{
			
		}
	}

	private void InitializeActiveSwords(int actorSwords)
	{
		for (int i = 0; i < SwordsSprites.Count; i++)
		{
			if (i < actorSwords)
			{
				SwordsSprites[i].enabled = true;
				_appearCoroutine = AppearSword(i);
				StartCoroutine(_appearCoroutine);
			}
			else
			{
				SwordsSprites[i].enabled = false;
			}
		}
		_swordsShown = actorSwords;
	}

	public void UpdateActiveSwords(int actorSwords)
	{
		Color colorForAppearing = new Color(0f, 1f, 0f);
		Color colorForShrinking = new Color(1f, .5f, .5f);
		_swordsShown = actorSwords;
		for (int i = 0; i < SwordsSprites.Count; i++)
		{
			if (i < actorSwords)
			{
				if (SwordsSprites[i].color == Color.clear)
				{
					_appearCoroutine = AppearSword(i);
					StartCoroutine(_appearCoroutine);
					SwordsSprites[i].color = colorForAppearing;
				}
			}
			else
			{
				if (SwordsSprites[i].color == Color.white)
				{
					_shrinkCoroutine = ShrinkSword(i);
					StartCoroutine(_shrinkCoroutine);
					SwordsSprites[i].color = colorForShrinking;
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
		SwordsSprites[index].color = Color.clear;
	}

	private IEnumerator AppearSword(int index)
	{
		for (float progress = 0f; progress < 1f; progress += .1f)
		{
			SwordsSprites[index].transform.localScale = Vector3.one * _swordAppearCurve.Evaluate(progress);
			yield return new WaitForSeconds(.03f);
		}
		SwordsSprites[index].color = Color.white;
		SwordsSprites[index].enabled = true;
		SwordsSprites[index].transform.localScale = Vector3.one;
	}

	private void SetFrameSpriteToMaxSwords(int maxSwordsInitial)
	{
		Sprite indicatorSprite = Resources.Load<Sprite>("Sprites/SwordsIndicator" + maxSwordsInitial);
		_frameSpriteRenderer.sprite = indicatorSprite;
	}
}