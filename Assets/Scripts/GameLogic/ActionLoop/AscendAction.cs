using System;
using System.Collections.Generic;
using Assets.Cinemachine.Base.Runtime.Behaviours;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using Assets.Scripts.GameLogic.GameCore;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop
{
	public class AscendAction : GameAction
	{
		private readonly IGameContext _gameContext;

		public AscendAction(ActorData actorData, float energyCost, IActionEffectFactory actionEffectFactory, IGameContext gameContext) 
			: base(actorData, energyCost, actionEffectFactory)
		{
			_gameContext = gameContext;
		}

		public override IEnumerable<IActionEffect> Execute()
		{
			++_gameContext.CurrentDungeonIndex;
			_gameContext.PlayerActor.ActorData.LogicalPosition = _gameContext.Dungeons[_gameContext.CurrentDungeonIndex].StairsLocation;
			Action action = () =>
			{
				_gameContext.PlayerActor.RefreshWorldPosition();
			};

			yield return new LambdaEffect(action);
		}
	}
}