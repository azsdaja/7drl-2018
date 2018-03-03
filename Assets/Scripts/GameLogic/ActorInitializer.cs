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
			if (actorData.ControlledByPlayer)
			{
				actorData.MaxHealth = 50;
				actorData.MaxSatiation = 200;
				actorData.Satiation = (int) (actorData.MaxSatiation * 0.8);
				actorData.Health = actorData.MaxHealth;
				Func<float> healthNormalizedGetter = () => ((float) actorData.Health) / actorData.MaxHealth;
				Func<float> satiationNormalizedGetter = () => ((float) actorData.Satiation) / actorData.MaxSatiation;
				_uiConfig.HealthBar.Initialize(healthNormalizedGetter);
				return;
			}
			actorData.Satiation = 99999;
			Sprite actorSprite = actorData.Entity.SpriteRenderer.sprite;
			Func<Vector2Int> positionGetter = () =>
			{
				Vector3Int cellPosition = _gridInfoProvider.WorldToCell(actorData.Entity.transform.position);
				return cellPosition.ToVector2Int();
			};

			switch (actorData.ActorType)
			{
				case ActorType.HerdAnimalImmature:
				{
					const float initialNeedValue = 1f;
					IDictionary<NeedType, float> satisfactions = new Dictionary<NeedType, float>
					{
						{NeedType.Hunger, initialNeedValue},
						{NeedType.Rest, initialNeedValue},
						{NeedType.Safety, 0.8f},
					};
					IDictionary<NeedFactor, float> factors = new Dictionary<NeedFactor, float>
					{
						{NeedFactor.EnemyProximity, 0f},
						{NeedFactor.FriendProximity, 0f}
					};
					actorData.NeedData = new NeedData(satisfactions, factors);
					actorData.MaxHealth = 30;
					actorData.Health = actorData.MaxHealth;
					break;
				}
				case ActorType.HerdAnimalMother:
				case ActorType.HerdAnimalFather:
				{
					const float initialNeedValue = 1f;
					IDictionary<NeedType, float> satisfactions = new Dictionary<NeedType, float>
					{
						{NeedType.Hunger, initialNeedValue},
						{NeedType.Rest, initialNeedValue},
						{NeedType.Safety, initialNeedValue},
						{NeedType.Care, initialNeedValue},
						{NeedType.Aggresion, initialNeedValue},
					};
					IDictionary<NeedFactor, float> factors = new Dictionary<NeedFactor, float>
					{
						{NeedFactor.EnemyProximity, 0f},
						{NeedFactor.FriendProximity, 0f},
						{NeedFactor.DeerlingProximity, 0f}
					};
					actorData.NeedData = new NeedData(satisfactions, factors);
					actorData.MaxHealth = 80;
					actorData.Health = actorData.MaxHealth;
					break;
				}
				default: throw new InvalidOperationException();
			}
		}
	}
}