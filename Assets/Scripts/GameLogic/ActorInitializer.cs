using System;
using System.Collections.Generic;
using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.Pathfinding;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	class ActorInitializer : IActorInitializer
	{
		private readonly IGridInfoProvider _gridInfoProvider;
		private readonly IGameContext _gameContext;
		private readonly IUiConfig _uiConfig;

		public ActorInitializer(IGridInfoProvider gridInfoProvider, IGameContext gameContext, IUiConfig uiConfig)
		{
			_gridInfoProvider = gridInfoProvider;
			_gameContext = gameContext;
			_uiConfig = uiConfig;
		}

		public void Initialize(ActorData actorData)
		{
			//if (actorData.ControlledByPlayer)
			//{
			//	Func<float> healthNormalizedGetter = () => ((float) actorData.Health) / actorData.MaxHealth;
			//	_uiConfig.HealthBar.Initialize(healthNormalizedGetter);
			//	return;
			//}
		}
	}
}