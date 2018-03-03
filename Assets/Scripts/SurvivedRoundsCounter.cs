using Assets.Scripts.GameLogic.GameCore;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts
{
	public class SurvivedRoundsCounter : MonoBehaviour
	{
		private IGameContext _gameContext;
		private Text _counter;

		[Inject]
		public void Init(IGameContext gameContext)
		{
			_gameContext = gameContext;
		}

		// Use this for initialization
		void Start ()
		{
			_counter = GetComponent<Text>();
		}

		// Update is called once per frame
		void Update ()
		{
		}
	}
}
