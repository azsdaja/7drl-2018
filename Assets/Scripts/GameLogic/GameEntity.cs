﻿using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.GameLogic.Animation;
using Assets.Scripts.GameLogic.Configuration;
using Assets.Scripts.Pathfinding;
using Assets.Scripts.RNG;
using Assets.Scripts.UI;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.GameLogic
{
	public abstract class GameEntity : MonoBehaviour, IGameEntity
	{
		private SpriteRenderer _spriteRenderer;
		private IGridInfoProvider _gridInfoProvider;
		private ITextEffectPresenter _textEffectPresenter;
		private IRandomNumberGenerator _rng;

		public IEntityAnimator EntityAnimator { get; private set; }
		public abstract EntityData EntityData { get; }
		public Vector3 Position
		{
			get { return transform.position; }
			set { transform.position = value; }
		}
	
		[Inject]
		public void Init(IGridInfoProvider gridInfoProvider, ITextEffectPresenter textEffectPresenter, IRandomNumberGenerator rng)
		{
			_gridInfoProvider = gridInfoProvider;
			_textEffectPresenter = textEffectPresenter;
			EntityData.LogicalPosition = _gridInfoProvider.WorldToCell(transform.position).ToVector2Int();
			_rng = rng;
		}

		protected virtual void Awake()
		{
			EntityAnimator = GetComponent<EntityAnimator>();
			_spriteRenderer = GetComponent<SpriteRenderer>();
			_spriteRenderer.enabled = false;
			Hide();
		}

		public virtual void Show()
		{
			_spriteRenderer.enabled = true;

			for (int i = 0; i < transform.childCount; i++)
			{
				if (transform.GetChild(i).name == "ControlArrows")
					continue;
				transform.GetChild(i).gameObject.SetActive(true);
			}
			if (this is ActorBehaviour)
			{
				var actorData = ((ActorBehaviour) this).ActorData;
				if (actorData.Team != Team.Beasts)
				{
					string text = "";
					if (actorData.ActorType == ActorType.Dog)
					{
						text = _rng.Choice(new[] {"Woof!", "Whrrrr!", "Woof! Woof!"});
					}
					else if (actorData.ActorType == ActorType.BruisedRat)
					{
						text = _rng.Choice(new[] {"Give me your guts!", "Squeak! Squeak!", "Ghhhrrr!"});
					}
					else
					{
						text = _rng.Choice(new[]
						{
							"You?!", "Back to your ward!", "Squeak!", "He's there!", "", "", "En garde!", "Have at you!",
							"Comrades, help me!", "Aah!"
						});
					}
					_textEffectPresenter.ShowTextEffect(EntityData.LogicalPosition, text);
				}
			}
		}

		public virtual void Hide()
		{
			_spriteRenderer.enabled = false;

			for (int i = 0; i < transform.childCount; i++)
			{
				transform.GetChild(i).gameObject.SetActive(false);
			}
		}

		public bool IsVisible { get { return _spriteRenderer.enabled; } }

		public SpriteRenderer SpriteRenderer
		{
			get { return _spriteRenderer; }
			set { _spriteRenderer = value; }
		}

		public void RefreshWorldPosition()
		{
			transform.position = _gridInfoProvider.GetCellCenterWorld(EntityData.LogicalPosition);
		}
	}
}