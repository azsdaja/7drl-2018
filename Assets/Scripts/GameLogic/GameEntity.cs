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