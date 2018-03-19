using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.GridRelated;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

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
		private readonly IGameContext _gameContext;
		private readonly ITextEffectPresenter _textEffectPresenter;

		public NeedHandler(IEntityDetector entityDetector, IGameConfig gameConfig, IUiConfig uiConfig, 
			IDeathHandler deathHandler, IMaxSwordCalculator maxSwordCalculator, IPlayerSpaceResolver playerSpaceResolver, IGameContext gameContext, ITextEffectPresenter textEffectPresenter)
		{
			_entityDetector = entityDetector;
			_gameConfig = gameConfig;
			_uiConfig = uiConfig;
			_deathHandler = deathHandler;
			_maxSwordCalculator = maxSwordCalculator;
			_playerSpaceResolver = playerSpaceResolver;
			_gameContext = gameContext;
			_textEffectPresenter = textEffectPresenter;
		}

		public void Heartbeat(ActorData actorData)
		{
			++actorData.RoundsCount;

			BoundsInt lastLevelBounds = new BoundsInt(-18, -103, 0, 49, 72, 1);
			BoundsInt almostLastLevelBounds = new BoundsInt(-16, -101, 0, 45, 68, 1);

			if (actorData.ActorType == ActorType.Player && _gameContext.CurrentDungeonIndex >= _gameContext.Dungeons.Count &&
			    !almostLastLevelBounds.Contains(actorData.LogicalPosition.ToVector3Int()))
			{
				_textEffectPresenter.ShowTextEffect(actorData.LogicalPosition, "Almost free!", Color.yellow);
			}

			if (actorData.ActorType == ActorType.Player && _gameContext.CurrentDungeonIndex >= _gameContext.Dungeons.Count && 
				!lastLevelBounds.Contains(actorData.LogicalPosition.ToVector3Int()))
			{
				var finisher = _uiConfig.GameFinisher;
				finisher.gameObject.SetActive(true);
				finisher.Initialize(actorData);
			}

			//if (actorData.Health < actorData.MaxHealth)
			//	++actorData.Health;

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

			if (actorData.Swords < maxSwords)
			{
				if (actorData.WeaponWeld.WeaponDefinition.RecoveryTime == RecoveryTime.ThreePerSeven)
				{
					int roundNumber = actorData.RoundsCount % 21;
					if (new[]{0, 3, 5, 8, 10, 13, 15, 18, 20}.Contains(roundNumber))
					{
						++actorData.Swords;
					}
				}
				else if (actorData.WeaponWeld.WeaponDefinition.RecoveryTime == RecoveryTime.OnePerTwo &&
				         actorData.RoundsCount % 2 == 0)
				{
						++actorData.Swords;
				}
				else if (actorData.WeaponWeld.WeaponDefinition.RecoveryTime == RecoveryTime.ThreePerFive)
				{
					int roundNumber = actorData.RoundsCount % 15;
					if (new[] {0, 2, 5, 7, 10, 12}.Contains(roundNumber))
					{
						++actorData.Swords;
					}
				}
			}

			if (actorData.Entity.transform.GetComponentInChildren<SwordsIndicator>() == null /*inactive*/)
			{
			}
			else
			{
				actorData.Entity.transform.GetComponentInChildren<SwordsIndicator>().UpdateActiveSwords(actorData.Swords);
				// kind of ugly workaround, but seems to help
			}

			if (actorData.ControlledByPlayer)
			{
				if (actorData.Xp >= _gameConfig.XpForLevels[actorData.Level + 1])
				{
					++actorData.Level;
					_uiConfig.AdvanceManager.gameObject.SetActive(true);
					GameObject.Find("PlayerLevelIndicator").GetComponent<Text>().text = "Player level: " + actorData.Level;
				}
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