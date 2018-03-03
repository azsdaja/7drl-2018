using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[Serializable]
	public class HerdMemberData
	{
		[SerializeField, HideInInspector]
		private ActorBehaviour _protector;

		[SerializeField, HideInInspector]
		private ActorBehaviour _child;

		[ShowInInspector]
		public ActorBehaviour Child
		{
			get { return _child; }
			set { _child = value; }
		}

		[ShowInInspector]
		public ActorBehaviour Protector
		{
			get { return _protector; }
			set { _protector = value; }
		}
	}
}