using System;
using Assets.Scripts;
using Assets.Scripts.GameLogic.GameCore;
using UnityEngine;
using Zenject;

public class XpBarInitializer : MonoBehaviour
{
	private ProgressBar _xpBar;
	private IGameContext _gameContext;
	private IGameConfig _gameConfig;
	private bool _initialized;

	[Inject]
	public void Init(IGameContext gameContext, IGameConfig gameConfig)
	{
		_gameContext = gameContext;
		_gameConfig = gameConfig;
	}

	// Use this for initialization
	void Awake()
	{
		_xpBar = GetComponent<ProgressBar>();
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (_initialized || _gameContext.PlayerActor == null) return;

		Func<float> progressCalculator = () =>
		{
			

			int toCurrentLevel = _gameConfig.XpForLevels[_gameContext.PlayerActor.ActorData.Level];
			int toNextLevel = _gameConfig.XpForLevels[_gameContext.PlayerActor.ActorData.Level + 1];
			int xpAfterCurrentLevel = _gameContext.PlayerActor.ActorData.Xp - toCurrentLevel;
			int fromCurrentToNext = toNextLevel - toCurrentLevel;
			float progress = (float) xpAfterCurrentLevel / fromCurrentToNext;
			return progress;
		};


		_xpBar.Initialize(progressCalculator, 0f);
		_initialized = true;
	}
}
