using System;
using System.Collections.Generic;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using Assets.Scripts.GameLogic.ActionLoop.AI;
using Assets.Scripts.GridRelated;

namespace Assets.Scripts.GameLogic.ActionLoop
{
	public class ActorController : IActorController
	{
		private float EnergyToAct = 1.0f;

		private readonly IAiActionResolver _aiActionResolver;
		private readonly IPlayerActionResolver _playerActionResolver;
		private readonly ITileVisibilityUpdater _tileVisibilityUpdater;
		private readonly INeedHandler _needHandler;

		public ActorController(IAiActionResolver aiActionResolver, IPlayerActionResolver playerActionResolver, 
			ITileVisibilityUpdater tileVisibilityUpdater, INeedHandler needHandler)
		{
			_aiActionResolver = aiActionResolver;
			_playerActionResolver = playerActionResolver;
			_tileVisibilityUpdater = tileVisibilityUpdater;
			_needHandler = needHandler;
		}

		public bool GiveControl(ActorData actorData)
		{
			if (actorData.Energy < EnergyToAct)
			{
				actorData.Energy += actorData.EnergyGain;
				return true;
			}

			if (DateTime.UtcNow < actorData.BlockedUntil)
			{
				return false;
			}

			IGameAction gameAction;
			if (actorData.ControlledByPlayer)
			{
				gameAction = _playerActionResolver.GetAction(actorData);
				if(!actorData.HasFreshFieldOfView)
				{
					_tileVisibilityUpdater.UpdateTileVisibility(actorData.LogicalPosition, actorData.VisionRayLength);
					actorData.HasFreshFieldOfView = true;
				}
			}
			else
			{
				gameAction = _aiActionResolver.GetAction(actorData);
			}

			if (gameAction == null)
			{
				return false;
			}

			actorData.HasFreshFieldOfView = false;
			actorData.Energy += actorData.EnergyGain;
			actorData.Energy -= gameAction.EnergyCost;
			_needHandler.Heartbeat(actorData); // todo: if Heartbeat works well, include modifying satiation and energy for actor

			IEnumerable<IActionEffect> effects = gameAction.Execute();
			foreach (IActionEffect effect in effects)
			{
				effect.Process();
			}

			return true;
		}
	}
}