using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop.Actions
{
	[CreateAssetMenu(fileName = "ItemDefinition", menuName = "7DRL Assets/ItemDefinition", order = 0)]
	public class ItemDefinition : ScriptableObject
	{
		public ItemType ItemType;
		public Sprite Sprite;
		public string Name;
		[TextArea]
		public string Description;

		public WeaponDefinition WeaponDefinition;
	}
}
