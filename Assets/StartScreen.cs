using Assets.Scripts.GameLogic.GameCore;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class StartScreen : MonoBehaviour
{
	public Text[] Texts;
	public Image[] Illustrations;
	public int CurrentIndex = 0;

	private IGameContext _gameContext;

	[Inject]
	public void Init(IGameContext gameContext)
	{
		_gameContext = gameContext;
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKeyDown)
		{
			Texts[CurrentIndex].gameObject.SetActive(false);
			Illustrations[CurrentIndex].gameObject.SetActive(false);

			CurrentIndex++;
			if (CurrentIndex < Texts.Length)
			{
				Texts[CurrentIndex].gameObject.SetActive(true);
				Illustrations[CurrentIndex].gameObject.SetActive(true);
			}
			else
			{
				if (Input.GetKeyDown(KeyCode.H))
				{
					_gameContext.PlayerActor.ActorData.Difficulty = 1;
					_gameContext.PlayerActor.ActorData.Accuracy -= 0.07f;
				}
				if (Input.GetKeyDown(KeyCode.E))
				{
					_gameContext.PlayerActor.ActorData.Difficulty = -1;
					_gameContext.PlayerActor.ActorData.Accuracy += 0.07f;
					_gameContext.PlayerActor.ActorData.MaxHealth += 10;
				}
				gameObject.SetActive(false);
			}
		}
	}
}
