using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameCore;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts
{
	public class NeedDisplayer : MonoBehaviour
	{
		private ActorBehaviour _actorBehaviour;
		private IGameConfig _gameConfig;

		public NeedType NeedType;
		public Image BarValue;
		private static Color _freshGreen;

		[Inject]
		public void Init(IGameConfig gameConfig)
		{
			_gameConfig = gameConfig;
		}

		// Use this for initialization
		void Start () {
			_actorBehaviour = transform.parent.parent.GetComponent<ActorBehaviour>();
			_freshGreen = new Color(.3f, 1f, .13f);
			gameObject.SetActive(_gameConfig.ModeConfig.ShowNeedBars);
		}
	
		// Update is called once per frame
		void Update ()
		{
		}
	}
}
