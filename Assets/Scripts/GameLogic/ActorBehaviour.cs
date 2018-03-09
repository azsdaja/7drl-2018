using System.Text;
using Assets.Scripts.GameLogic.ActionLoop;
using Assets.Scripts.GameLogic.GameCore;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.GameLogic
{
	public class ActorBehaviour : GameEntity
	{
		[SerializeField] private ActorData _actorData;

		private IActorController _actorController;
		private IActorInitializer _actorInitializer;
		private IUiConfig _uiConfig;
		private IGameConfig _gameConfig;

		public override EntityData EntityData
		{
			get { return ActorData; }
		}

		public ActorData ActorData
		{
			get { return _actorData; }
		}

		[Inject]
		public void Init(IActorController actorController, IActorInitializer actorInitializer, IUiConfig uiConfig, IGameConfig gameConfig)
		{
			_actorController = actorController;
			_actorInitializer = actorInitializer;
			_uiConfig = uiConfig;
			_gameConfig = gameConfig;
		}

		new void Awake()
		{
			base.Awake();
			_actorInitializer.Initialize(ActorData);
		}

		public bool GiveControl()
		{
			bool passControlToNextActor = _actorController.GiveControl(ActorData);
		
			return passControlToNextActor;
		}

		public Transform Transform { get { return transform; } }
		public WeaponAnimator WeaponAnimator;

		void OnMouseEnter()
		{
			if (!_gameConfig.ModeConfig.ShowActorTooltip) return;

			Canvas tooltipPooled = _uiConfig.TooltipPooled;
			tooltipPooled.gameObject.SetActive(true);
			tooltipPooled.transform.position = transform.position;
			tooltipPooled.transform.Find("Text").GetComponent<Text>().text = GetTooltipDescription();
		}

		void OnMouseExit()
		{
			_uiConfig.TooltipPooled.gameObject.SetActive(false);
		}

		private string GetTooltipDescription()
		{
			var description = new StringBuilder();
			description.AppendLine(name);
			return description.ToString();
		}

		void Update()
		{
			SpriteRenderer.sortingOrder = -ActorData.LogicalPosition.y;
		}

		public class Factory : Zenject.Factory<ActorBehaviour> { }
	}
}