using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.GameLogic.GameCore
{
	public class GameManager : MonoBehaviour
	{
		private IList<ActorBehaviour> _actors;
		private int _currentActorIndex;
		private Stopwatch _frameLengthStopwatch;
		private IGameContext _gameContext;
		private IUiConfig _uiConfig;

		[Inject]
		public void Init(IGameContext gameContext, IUiConfig uiConfig)
		{
			_gameContext = gameContext;
			_uiConfig = uiConfig;
		}

		// Use this for initialization
		void Start ()
		{
			_uiConfig.RestartButton.gameObject.SetActive(false);
			_frameLengthStopwatch = new Stopwatch();
			_gameContext.Actors = new HashSet<ActorBehaviour>(FindObjectsOfType<ActorBehaviour>().Where(b => b.enabled));
			_actors = _gameContext.Actors.ToList();
		}
	
		// Update is called once per frame
		void Update ()
		{
			_frameLengthStopwatch.Start();

			int giveControlCountPerActor = 5; // empiric
			int actorsToProcessInThisFrame = _actors.Count * giveControlCountPerActor;
			_actors = _gameContext.Actors.ToList();
			for (int i = 0; i < actorsToProcessInThisFrame; i++)
			{
				ActorBehaviour currentActor = _actors[_currentActorIndex];
				bool passControlToNextActor = true;
				
				if (currentActor.ActorData.Health > 0)
				{
					passControlToNextActor = currentActor.GiveControl();
				}
				if (passControlToNextActor == false) return;

				_currentActorIndex = (_currentActorIndex + 1) % _gameContext.Actors.Count;

				// if this frame is taking too long time (giving FPS<100), finish it, so that UI can catch up
				const int frameLengthForFps100 = 10;
				if (_frameLengthStopwatch.ElapsedMilliseconds > frameLengthForFps100)
				{
					_frameLengthStopwatch.Reset();
					return;
				}
			}
		}
	}
}