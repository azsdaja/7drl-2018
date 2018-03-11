using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasherMessage : MonoBehaviour
{
	public Text Message;

	public void SetMessage(string message)
	{
		Message.text = message;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
