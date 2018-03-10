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
		[SerializeField, HideInInspector] private Weapon _weapon;

		[ShowInInspector]
		public ItemType ItemType
		{
			get { return _itemType; }
			set { _itemType = value; }
		}

		[ShowInInspector]
		public Weapon Weapon
		{
			get { return _weapon; }
			set { _weapon = value; }
		}
	}
}
