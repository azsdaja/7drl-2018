using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[Serializable]
	public class AiData
	{
		[SerializeField, HideInInspector]
		private int _turnsLeftToStayWhileResting;

		[SerializeField, HideInInspector]
		private HerdMemberData _herdMemberData;

		public int TurnsLeftToStayWhileResting
		{
			get { return _turnsLeftToStayWhileResting; }
			set { _turnsLeftToStayWhileResting = value; }
		}

		[ShowInInspector]
		public HerdMemberData HerdMemberData
		{
			get { return _herdMemberData; }
			set { _herdMemberData = value; }
		}
	}
}