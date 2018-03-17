using System;
using System.Linq;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using UnityEngine;

namespace Assets.Scripts.GameLogic.GameCore
{
	[CreateAssetMenu(fileName = "ItemConfig", menuName = "Configuration/ItemConfig", order = 1)]
	public class ItemConfig : ScriptableObject
	{
		public ItemDefinition[] Definitions;
		public ItemDefinition[] ItemPoolWeak;
		public ItemDefinition[] ItemPoolMedium;
		public ItemDefinition[] ItemPoolStrong;

		public ItemDefinition GetDefinition(ItemType itemType)
		{
			try
			{
				return Definitions.Single(d => d.ItemType == itemType);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}

		}
	}
}