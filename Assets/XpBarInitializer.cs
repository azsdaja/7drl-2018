using Assets.Scripts;
using Assets.Scripts.GameLogic.GameCore;
using UnityEngine;
using Zenject;

public class XpBarInitializer : MonoBehaviour
{
	private ProgressBar _xpBar;
	private IGameContext _gameContext;
	private bool _initialized;

	[Inject]
	public void Init(IGameContext gameContext)
	{
		_gameContext = gameContext;
	}

	// Use this for initialization
	void Start()
	{
		_xpBar = GetComponent<ProgressBar>();
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (_initialized || _gameContext.PlayerActor == null) return;

		_xpBar.Initialize(() => (float)_gameContext.PlayerActor.ActorData.Xp / 100, 0f);
		_initialized = true;
	}
}
