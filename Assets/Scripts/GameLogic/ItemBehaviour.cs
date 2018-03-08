using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class ItemBehaviour : GameEntity
	{
		[SerializeField]
		private ItemData _itemData;

		public override EntityData EntityData
		{
			get { return _itemData; }
		}

		public ItemData ItemData { get { return _itemData; } }

		public new void Awake()
		{
			base.Awake();
		}

		public class Factory : Zenject.Factory<ItemBehaviour>{}
	}
}
