using Assets.Scripts.GameLogic.ActionLoop.Actions;
using UnityEngine;
using UnityEngine.UI;

public class ItemHolder : MonoBehaviour
{
	public int Limit = 4;
	public ItemDefinition[] Items;
	public Image[] HeldItemsImages;

	public int SelectedItemIndex = -1;

	void Start()
	{
		Items = new ItemDefinition[4];
	}

	public void AddItem(ItemDefinition item)
	{
		for (var itemIndex = 0; itemIndex < Items.Length; itemIndex++)
		{
			ItemDefinition itemInHolder = Items[itemIndex];
			if (itemInHolder == null)
			{
				Items[itemIndex] = item;
				HeldItemsImages[itemIndex].sprite = item.Sprite;
				HeldItemsImages[itemIndex].color = Color.white;
				return;
			}
		}
		Debug.LogError("Item " + item.ItemType + " lost because there was no place for it!");
	}

	public void RemoveItem(int itemIndex)
	{
		Items[itemIndex] = null;
		HeldItemsImages[itemIndex].color = Color.clear;
	}

	public ItemDefinition SelectItem(int index)
	{
		DeselectItem();

		if (Items[index] != null)
		{
			HeldItemsImages[index].transform.localScale = HeldItemsImages[index].transform.localScale * 1.5f;
		}

		SelectedItemIndex = index;

		return Items[index];
	}

	public void DeselectItem()
	{
		if (SelectedItemIndex == -1) return;

		HeldItemsImages[SelectedItemIndex].transform.localScale = HeldItemsImages[SelectedItemIndex].transform.localScale * 0.67f;
		SelectedItemIndex = -1;
	}

	public ItemDefinition CurrentItem()
	{
		if (SelectedItemIndex < 0)
		{
			return null;
		}
		return Items[SelectedItemIndex];
	}
}
