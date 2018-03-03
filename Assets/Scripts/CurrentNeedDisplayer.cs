using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameCore;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts
{
	public class CurrentNeedDisplayer : MonoBehaviour {

		private ActorBehaviour _actorBehaviour;

		private IGameConfig _gameConfig;

		public Text CurrentNeedText;

		[Inject]
		public void Init(IGameConfig gameConfig)
		{
			_gameConfig = gameConfig;
		}

		// Use this for initialization
		void Start () {
			_actorBehaviour = transform.parent.parent.GetComponent<ActorBehaviour>();
			gameObject.SetActive(_gameConfig.ModeConfig.ShowCurrentNeed);
		}
	
		// Update is called once per frame
		void Update ()
		{
			CurrentNeedText.text = _actorBehaviour.ActorData.NeedData.CurrentNeed.ToString();
		}
	}
}
