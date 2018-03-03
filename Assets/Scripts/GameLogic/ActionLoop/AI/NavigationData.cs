using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop.AI
{
	[Serializable]
	public class NavigationData
	{
		[SerializeField, HideInInspector] private List<Vector2Int> _pathToFollow;
		[SerializeField, HideInInspector] private Vector2Int? _nextNode;
		[SerializeField, HideInInspector] private Vector2Int? _destination;
		[SerializeField, HideInInspector] private Stack<Vector2Int> _stepsToNextNode;

		[ShowInInspector]
		public List<Vector2Int> PathToFollow
		{
			get { return _pathToFollow; }
			set { _pathToFollow = value; }
		}

		[ShowInInspector]
		public Stack<Vector2Int> StepsToNextNode
		{
			get { return _stepsToNextNode; }
			set { _stepsToNextNode = value; }
		}

		[ShowInInspector]
		public Vector2Int? NextNode
		{
			get { return _nextNode; }
			set { _nextNode = value; }
		}

		[ShowInInspector]
		public Vector2Int? Destination
		{
			get { return _destination; }
			set { _destination = value; }
		}
	}
}