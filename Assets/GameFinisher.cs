using System.Text;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameCore;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameFinisher : MonoBehaviour
{
	public Text Summary;
	public Image FinalLook;

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
		
	}

	public void Initialize(ActorData actorData)
	{
		string summaryFormat = @"It took you {0} turns to finish. 
You earned {1} experience points and reached level {2}.
{3}{4}
{5}
The difficulty was {6}.";
		string summary = string.Format(summaryFormat,
			actorData.RoundsCount,
			actorData.Xp,
			actorData.Level,
			actorData.HasFinishedDuel
				? "You have honorably finished the duel with Farwis and you " +
				  (actorData.HasWonDuel ? "won it." : "lost it. ")
				: "You haven't finished the duel with Farwis. ",
			_gameContext.BasherDead ? "He died as a hero while trying to save you." : "",
			actorData.HasTail
				? "You have found the secret ending and regrew your tail."
				: "You have not found the secret ending.",
			actorData.Difficulty == 1 ? "hard" : (actorData.Difficulty == 0 ? "normal" : "easy")
		);
		Summary.text = summary;
		FinalLook.sprite = actorData.Entity.SpriteRenderer.sprite;
	}
}
