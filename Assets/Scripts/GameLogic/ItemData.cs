using System;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[Serializable]
	public class ItemData : EntityData
	{
		[SerializeField, HideInInspector] private ItemType _itemType;

		[ShowInInspector]
		public ItemType ItemType
		{
			get { return _itemType; }
			set { _itemType = value; }
		}
	}
}
