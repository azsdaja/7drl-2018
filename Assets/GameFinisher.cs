using System.Text;
using Assets.Scripts.GameLogic;
using UnityEngine;
using UnityEngine.UI;

public class GameFinisher : MonoBehaviour
{
	public Text Summary;
	public Image FinalLook;

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
The difficulty was {5}.";
		string summary = string.Format(summaryFormat,
			actorData.RoundsCount,
			actorData.Xp,
			actorData.Level,
			actorData.HasFinishedDuel
				? "You have honorably finished the duel with your neighbour, and you " +
				  (actorData.HasWonDuel ? "won it." : "lost it.")
				: "You ran away during the duel with your neighbour.",
			actorData.HasTail
				? "You have found the secret ending and regrew your tail."
				: "You have not found the secret ending.",
			actorData.Difficulty == 1 ? "hard" : (actorData.Difficulty == 0 ? "normal" : "easy")
		);
		Summary.text = summary;
		FinalLook.sprite = actorData.Entity.SpriteRenderer.sprite;
	}
}
