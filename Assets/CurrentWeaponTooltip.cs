using UnityEngine;
using UnityEngine.UI;

public class CurrentWeaponTooltip : MonoBehaviour
{
	public Text LabelWearingUpper;
	public GameObject TooltipPanel; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.Alpha0))
		{
			//TooltipPanel.SetActive(true);
		}
	}


}
