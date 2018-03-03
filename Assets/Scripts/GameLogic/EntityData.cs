using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[Serializable]
	public class EntityData
	{
		[SerializeField, HideInInspector] private GameEntity _entity;

		[SerializeField, HideInInspector] private Vector2Int _logicalPosition;

		[ShowInInspector]
		public Vector2Int LogicalPosition
		{
			get { return _logicalPosition; }
			set { _logicalPosition = value; }
		}

		[ShowInInspector]
		public GameEntity Entity
		{
			get { return _entity; }
			set { _entity = value; }
		}
	}
}