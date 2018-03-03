﻿using UnityEngine;

namespace Assets.Scripts.UnityUtilities
{
	public class Label : MonoBehaviour
	{
		public string Text = "";
	
		void OnGUI()
		{
			Vector3 positionOnScreen = Camera.main.WorldToScreenPoint(transform.position);
			var rect = new Rect(0, 0, 100, 60);
			rect.x = positionOnScreen.x;
			rect.y = Screen.height - positionOnScreen.y - rect.height;
			GUI.Label(rect, Text);
		}
	}
}
