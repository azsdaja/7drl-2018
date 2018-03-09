using Assets.Scripts.GameLogic.GameCore;
using UnityEngine;
using Zenject;

namespace Assets.Scripts
{
	public class SceneRestarter : MonoBehaviour
	{
		private IGameContext _gameContext;

		[Inject]
		public void Init(IGameContext gameContext)
		{
			_gameContext = gameContext;
		}

		public void RestartScene()
		{
			Application.LoadLevel(0);
		}
	}
}
