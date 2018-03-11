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

		public string GetDescription()
		{
			if (Name == "Key")
			{
				return "A key you found in a loaf of bread.";
			}
			if (ItemType == ItemType.Weapon)
			{
				string descriptionPattern = @"Max combat capacity: +{0}<sprite=0><br>
<sprite=0> recovery: 1<sprite=0> per {1} turns<br>
Max damage: {2}<br>
Close combat modifier: {3}<sprite=0><br>
Long: {4}";
				var description = string.Format(descriptionPattern,
					WeaponDefinition.Swords,
					WeaponDefinition.RecoveryTime,
					WeaponDefinition.MaxDamage,
					WeaponDefinition.CloseCombatModifier,
					WeaponDefinition.AllowsFarCombat ? "<color=#10ff00>yes</color>" : "<color=#ff0000>no</color>");
				return description;
			}
			else return Description;
		}

		public WeaponDefinition WeaponDefinition;
	}
}
