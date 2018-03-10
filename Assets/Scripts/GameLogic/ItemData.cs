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
		[SerializeField, HideInInspector] private WeaponDefinition _weaponDefinition;
		[SerializeField, HideInInspector] private ItemDefinition _itemItemDefinition;

		[ShowInInspector]
		public ItemType ItemType
		{
			get { return _itemType; }
			set { _itemType = value; }
		}

		[ShowInInspector]
		public WeaponDefinition WeaponDefinition
		{
			get { return _weaponDefinition; }
			set { _weaponDefinition = value; }
		}

		[ShowInInspector]
		public ItemDefinition ItemDefinition
		{
			get { return _itemItemDefinition; }
			set { _itemItemDefinition = value; }
		}
	}
}
