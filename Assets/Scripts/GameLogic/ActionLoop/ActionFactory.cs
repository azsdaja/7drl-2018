using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using Assets.Scripts.Pathfinding;
using Assets.Scripts.RNG;
using Assets.Scripts.UI;
using UnityEngine;

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
		private readonly IEntityRemover _entityRemover;

		public ActionFactory(IGridInfoProvider gridInfoProvider, IActionEffectFactory actionEffectFactory, 
			ITextEffectPresenter textEffectPresenter, INeedHandler needHandler, IRandomNumberGenerator randomNumberGenerator, IDeathHandler deathHandler, IEntityRemover entityRemover)
		{
			_gridInfoProvider = gridInfoProvider;
			_actionEffectFactory = actionEffectFactory;
			_textEffectPresenter = textEffectPresenter;
			_needHandler = needHandler;
			_randomNumberGenerator = randomNumberGenerator;
			_deathHandler = deathHandler;
			_entityRemover = entityRemover;
		}

		public IGameAction CreateDisplaceAction(ActorData actorData, ActorData actorAtTargetPosition)
		{
			return new DisplaceAction(actorData, actorAtTargetPosition, 1f, _actionEffectFactory);
		}

		public IGameAction CreateAttackAction(ActorData actorData, ActorData attackedActor)
		{
			return new AttackAction(actorData, attackedActor, 1f, _actionEffectFactory, _randomNumberGenerator, _deathHandler);
		}

		public IGameAction CreateMoveAction(ActorData actorData, Vector2Int actionVector)
		{
			return new MoveAction(actorData, 1f, _actionEffectFactory, actionVector, _gridInfoProvider);
		}

		public IGameAction CreateDropAction(ActorData actorData, ItemData firstItem)
		{
			return new DropAction(actorData, 1f, firstItem, _actionEffectFactory);
		}

		public IGameAction CreatePickUpAction(ActorData actorData, ItemData itemToPickUp)
		{
			return new PickUpAction(actorData, 1f, itemToPickUp, _actionEffectFactory);
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
				if(!actorData.ControlledByPlayer)
					_needHandler.ModifyNeed(actorDataParameter.NeedData, NeedType.Rest, 0.1f);
				return Enumerable.Empty<IActionEffect>();
			};
			return new LambdaAction(actorData, 1f, _actionEffectFactory, inlineAction);
		}

		public IGameAction CreateCallAction(ActorData actorData)
		{
			Func<ActorData, IEnumerable<IActionEffect>> inlineAction = actorDataParameter =>
			{
				_needHandler.ModifyNeed(actorDataParameter.NeedData, NeedType.Care, 0.4f);
				_needHandler.ModifyNeed(actorDataParameter.AiData.HerdMemberData.Child.ActorData.NeedData, NeedType.Safety, - 0.3f);
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
	}
}