using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.GridRelated;
using Assets.Scripts.Pathfinding;
using Assets.Scripts.RNG;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.GameLogic.ActionLoop
{
	public class ActionFactory : IActionFactory
	{
		private readonly IGridInfoProvider _gridInfoProvider;
		private readonly IActionEffectFactory _actionEffectFactory;
		private readonly ITextEffectPresenter _textEffectPresenter;
		private readonly INeedHandler _needHandler;
		private readonly IRandomNumberGenerator _randomNumberGenerator;
		private readonly IDeathHandler _deathHandler;
		private readonly IEntityDetector _entityDetector;
		private readonly IEntityRemover _entityRemover;
		private readonly IGameContext _gameContext;
		private readonly IEntitySpawner _entitySpawner;
		private readonly IUiConfig _uiConfig;

		public ActionFactory(IGridInfoProvider gridInfoProvider, IActionEffectFactory actionEffectFactory, 
			ITextEffectPresenter textEffectPresenter, INeedHandler needHandler, IRandomNumberGenerator randomNumberGenerator,
			IDeathHandler deathHandler, IEntityRemover entityRemover, IEntityDetector entityDetector, IGameContext gameContext, 
			IEntitySpawner entitySpawner, IUiConfig uiConfig)
		{
			_gridInfoProvider = gridInfoProvider;
			_actionEffectFactory = actionEffectFactory;
			_textEffectPresenter = textEffectPresenter;
			_needHandler = needHandler;
			_randomNumberGenerator = randomNumberGenerator;
			_deathHandler = deathHandler;
			_entityRemover = entityRemover;
			_entityDetector = entityDetector;
			_gameContext = gameContext;
			_entitySpawner = entitySpawner;
			_uiConfig = uiConfig;
		}

		public IGameAction CreateDisplaceAction(ActorData actorData, ActorData actorAtTargetPosition)
		{
			return new DisplaceAction(actorData, actorAtTargetPosition, 1f, _actionEffectFactory);
		}

		public IGameAction CreateAttackAction(ActorData actorData, ActorData attackedActor, bool isAggressiveAttack = false)
		{
			return new AttackAction(actorData, attackedActor, 1f, _actionEffectFactory, _randomNumberGenerator, _deathHandler, isAggressiveAttack);
		}

		public IGameAction CreateMoveAction(ActorData actorData, Vector2Int actionVector)
		{
			return new MoveAction(actorData, 1f, _actionEffectFactory, actionVector, _gridInfoProvider, _entityDetector);
		}

		public IGameAction CreateDropAction(ActorData actorData, ItemData firstItem)
		{
			return new DropAction(actorData, 1f, firstItem, _actionEffectFactory);
		}

		public IGameAction CreatePickUpAction(ActorData actorData, ItemData itemToPickUp)
		{
			return new PickUpAction(actorData, 1f, itemToPickUp, _actionEffectFactory, _entitySpawner, _entityRemover, _uiConfig);
		}

		public IGameAction CreateCatchAction(ActorData actorData, ActorData caughtActor)
		{
			return new CatchAction(actorData, 1f, caughtActor, _actionEffectFactory);
		}

		public IGameAction CreateReleaseAction(ActorData actorData)
		{
			return new ReleaseAction(actorData, 1f, _actionEffectFactory);
		}

		public IGameAction CreateEatAction(ActorData actorData, ItemData foodItem)
		{
			return new EatAction(actorData, 1f, _actionEffectFactory, _needHandler, _textEffectPresenter, _entityRemover, foodItem);
		}

		public IGameAction CreateEatEnvironmentAction(ActorData actorData)
		{
			return new EatEnvironmentAction(actorData, 1f, _actionEffectFactory, _needHandler, _textEffectPresenter);
		}

		public IGameAction CreatePassAction(ActorData actorData)
		{
			Func<ActorData, IEnumerable<IActionEffect>> inlineAction = actorDataParameter =>
			{
				// todo: get rid of this „if” clause — we don't need to treat player specially.
				return Enumerable.Empty<IActionEffect>();
			};
			return new LambdaAction(actorData, 1f, _actionEffectFactory, inlineAction);
		}

		public IGameAction CreateCallAction(ActorData actorData)
		{
			Func<ActorData, IEnumerable<IActionEffect>> inlineAction = actorDataParameter =>
			{
				return new[]{new LambdaEffect(() =>
				{
					_textEffectPresenter.ShowTextEffect(actorData.LogicalPosition, "HRUMPH!");
				})};
			};
			return new LambdaAction(actorData, 1f, _actionEffectFactory, inlineAction);
		}

		public IGameAction CreateStandUpAction(ActorData actorData)
		{
			return new StandUpGameAction(actorData, 1f, _actionEffectFactory);
		}

		public IGameAction CreatePushAction(ActorData actorData, ActorData targetEnemy)
		{
			return new PushAction(actorData, targetEnemy, 1f, _actionEffectFactory, _randomNumberGenerator, _gridInfoProvider, 
				_entityDetector, _uiConfig);
		}

		public IGameAction CreateOpenDoorAction(ActorData actorData, Vector2Int targetPosition, bool isHorizontal)
		{
			return new OpenDoorAction(actorData, targetPosition, isHorizontal, 1f, _actionEffectFactory, _gameContext);
		}

		public IGameAction CreateAscendAction(ActorData actorData)
		{
			return new AscendAction(actorData, 1f, _actionEffectFactory, _gameContext);
		}

		public IGameAction CreateUseItemAction(ActorData actorData, ItemDefinition item)
		{
			return new UseItemAction(actorData, item, 1f, _entitySpawner, _uiConfig, _actionEffectFactory);
		}
	}
}