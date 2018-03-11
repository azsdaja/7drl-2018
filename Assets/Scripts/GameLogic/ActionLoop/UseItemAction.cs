using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.GridRelated;
using Assets.Scripts.RNG;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop
{
	public class UseItemAction : GameAction
	{
		private readonly ItemDefinition _item;
		private readonly IEntitySpawner _entitySpawner;
		private readonly IEntityDetector _entityDetector;
		private readonly IUiConfig _uiConfig;
		private readonly IRandomNumberGenerator _rng;
		private readonly ITextEffectPresenter _textEffectPresenter;

		public UseItemAction(ActorData actorData, ItemDefinition item, float energyCost, IEntitySpawner entitySpawner, 
			IUiConfig uiConfig, IActionEffectFactory actionEffectFactory, IEntityDetector entityDetector, IRandomNumberGenerator rng, ITextEffectPresenter textEffectPresenter)
			: base(actorData, energyCost, actionEffectFactory)
		{
			_item = item;
			_entitySpawner = entitySpawner;
			_uiConfig = uiConfig;
			_entityDetector = entityDetector;
			_rng = rng;
			_textEffectPresenter = textEffectPresenter;
		}

		public override IEnumerable<IActionEffect> Execute()
		{
			_uiConfig.ItemHolder.RemoveItem(_uiConfig.ItemHolder.SelectedItemIndex);

			if (_item.ItemType == ItemType.PotionOfRecoverTail)
			{
				_textEffectPresenter.ShowTextEffect(ActorData.LogicalPosition, "Squeak! At last!", Color.yellow);
			}
			else
			{
				_textEffectPresenter.ShowTextEffect(ActorData.LogicalPosition, "Ha!", Color.yellow);
			}

			if (_item.ItemType == ItemType.Weapon)
			{
				ItemDefinition previousWeapon = ActorData.WeaponWeld;
				_uiConfig.ItemHolder.AddItem(previousWeapon);

				ActorData.WeaponWeld = _item;
				Action effectAction = () => ((ActorBehaviour) ActorData.Entity).WeaponAnimator.Awake();
				var effect = new LambdaEffect(effectAction);
				yield return effect;
			}
			else if (_item.ItemType == ItemType.Food)
			{
				ActorData.Health += (int) (ActorData.MaxHealth * 0.3f);
				if (ActorData.Health > ActorData.MaxHealth)
					ActorData.Health = ActorData.MaxHealth;
			}
			else if (_item.ItemType == ItemType.PotionOfHealing)
			{
				ActorData.Health = ActorData.MaxHealth;
			}
			else if (_item.ItemType == ItemType.PotionOfBuddy)
			{
				_entitySpawner.SpawnActor(ActorType.Buddy, ActorData.LogicalPosition);
			}
			else if (_item.ItemType == ItemType.PotionOfFriend)
			{
				_entitySpawner.SpawnActor(ActorType.Friend, ActorData.LogicalPosition);
			}
			else if (_item.ItemType == ItemType.PotionOfRecoverTail)
			{
				Sprite withTailSprite = Resources.Load<Sprite>("Sprites/Characters/player_with_tail");
				ActorData.Entity.SpriteRenderer.sprite = withTailSprite;
				ActorData.HasTail = true;
			}
			else if (_item.ItemType == ItemType.PotionOfLight)
			{
				IEnumerable<ActorData> actorsAround = _entityDetector.DetectActors(ActorData.LogicalPosition, 4).Where(a => a != ActorData);
				foreach (var actorData in actorsAround)
				{
					actorData.Energy -= 2 + _rng.NextFloat() * 2f;
					string text = "";
					if (actorData.ActorType != ActorType.Dog)
					{
						text = _rng.Choice(new[] {"My eyes!", "I can't see!", "Oh!", "Squeak!"});
					}
					else
					{
						text = "Squeak!";
					}
					_textEffectPresenter.ShowTextEffect(actorData.LogicalPosition, text);
				}
			}
		}
	}
}