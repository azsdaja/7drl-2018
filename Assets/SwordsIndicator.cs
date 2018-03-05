using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.GameLogic;
using UnityEngine;

public class SwordsIndicator : MonoBehaviour
{
	public AnimationCurve _swordAppearCurve;
	private ActorBehaviour _actorBehaviour;
	private int _swordsShown;
	private IEnumerator _shrinkCoroutine;
	private IEnumerator _appearCoroutine;

	public List<SpriteRenderer> SwordsSprites;

	// Use this for initialization
	void Start ()
	{
		_actorBehaviour = transform.parent.GetComponent<ActorBehaviour>();
		_swordsShown = _actorBehaviour.ActorData.Swords;
		Sprite indicatorSprite = Resources.Load<Sprite>("Sprites/SwordsIndicator" + _actorBehaviour.ActorData.MaxSwords);
		GetComponent<SpriteRenderer>().sprite = indicatorSprite;
	}
	
	// Update is called once per frame
	void Update ()
	{
		int playerSwords = _actorBehaviour.ActorData.Swords;
		if (playerSwords != _swordsShown)
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
}
