using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.GameLogic.Animation;
using Assets.Scripts.Pathfinding;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.GameLogic
{
	public abstract class GameEntity : MonoBehaviour, IGameEntity
	{
		private SpriteRenderer _spriteRenderer;
		private IGridInfoProvider _gridInfoProvider;

		public IEntityAnimator EntityAnimator { get; private set; }
		public abstract EntityData EntityData { get; }
		public Vector3 Position
		{
			get { return transform.position; }
			set { transform.position = value; }
		}
	
		[Inject]
		public void Init(IGridInfoProvider gridInfoProvider)
		{
			_gridInfoProvider = gridInfoProvider;
			EntityData.LogicalPosition = _gridInfoProvider.WorldToCell(transform.position).ToVector2Int();
		}

		protected virtual void Start()
		{
			EntityAnimator = GetComponent<EntityAnimator>();
			_spriteRenderer = GetComponent<SpriteRenderer>();
			_spriteRenderer.enabled = false;
		}

		public virtual void Show()
		{
			_spriteRenderer.enabled = true;
			Transform needsDiplayers = transform.Find("NeedsDisplayers");
			if (needsDiplayers != null)
			{
				needsDiplayers.gameObject.SetActive(true);
			}

			
			Transform healthDiplayer = transform.Find("HealthDisplayer");
			if (healthDiplayer != null)
			{
				healthDiplayer.gameObject.SetActive(true);
			}
		}

		public virtual void Hide()
		{
			_spriteRenderer.enabled = false;
			Transform needsDiplayers = transform.Find("NeedsDisplayers");
			if (needsDiplayers != null)
			{
				needsDiplayers.gameObject.SetActive(false);
			}

			Transform healthDiplayer = transform.Find("HealthDisplayer");
			if (healthDiplayer != null)
			{
				healthDiplayer.gameObject.SetActive(false);
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