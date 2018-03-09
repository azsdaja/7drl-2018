using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GameLogic.GameCore
{
	[CreateAssetMenu(fileName = "ActorConfig", menuName = "Configuration/ActorConfig", order = 1)]
	public class ActorConfig : ScriptableObject
	{
		public ActorDefinition[] Definitions;

		public ActorDefinition GetDefinition(ActorType actorType)
		{
			try
			{
				return Definitions.Single(d => d.ActorType == actorType);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
			
		}
	}
}