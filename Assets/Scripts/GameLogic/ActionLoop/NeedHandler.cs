using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.GridRelated;

namespace Assets.Scripts.GameLogic.ActionLoop
{
	public class NeedHandler : INeedHandler
	{
		private readonly IEntityDetector _entityDetector;
		private readonly IGameConfig _gameConfig;
		private readonly IUiConfig _uiConfig;
		private readonly IDeathHandler _deathHandler;
		private readonly IMaxSwordCalculator _maxSwordCalculator;
		private readonly IPlayerSpaceResolver _playerSpaceResolver;

		public NeedHandler(IEntityDetector entityDetector, IGameConfig gameConfig, IUiConfig uiConfig, 
			IDeathHandler deathHandler, IMaxSwordCalculator maxSwordCalculator, IPlayerSpaceResolver playerSpaceResolver)
		{
			_entityDetector = entityDetector;
			_gameConfig = gameConfig;
			_uiConfig = uiConfig;
			_deathHandler = deathHandler;
			_maxSwordCalculator = maxSwordCalculator;
			_playerSpaceResolver = playerSpaceResolver;
		}

		public void Heartbeat(ActorData actorData)
		{

			++actorData.RoundsCount;

			if (actorData.Health < actorData.MaxHealth)
				++actorData.Health;

			IEnumerable<ActorData> enemiesNearby = _entityDetector.DetectActors(actorData.LogicalPosition, 2)
				.Where(a => a.Team != actorData.Team);
			if (enemiesNearby.Any(a => Vector2IntUtilities.IsOneStep(a.LogicalPosition, actorData.LogicalPosition)))
			{
				actorData.IsInCloseCombat = true;
			}
			else
			{
				actorData.IsInCloseCombat = false;
			}

			actorData.HasLittleSpace = _playerSpaceResolver.ResolveIfPlayerHasLittleSpace(actorData);

			int maxSwords = _maxSwordCalculator.Calculate(actorData);
			actorData.MaxSwords = maxSwords;
			if (actorData.Swords > maxSwords)
				actorData.Swords = maxSwords;

			if (actorData.Swords < maxSwords && actorData.RoundsCount % actorData.Weapon.RecoveryTime == 0)
			{
				++actorData.Swords;
			}

			if (actorData.Xp > 30 * actorData.Level)
			{
				++actorData.Level;
				_uiConfig.AdvanceManager.gameObject.SetActive(true);
			}
		}

		public void ModifyNeed(NeedData needData, NeedType needType, float delta)
		{
			needData.ModifySatisfaction(needType, delta);
		}

		public void ModifyNeedWithFactor(NeedData needData, NeedType needType, NeedFactor needFactor, float delta)
		{
			needData.ModifyNeedWithFactor(needType, needFactor, delta);
		}
	}
}