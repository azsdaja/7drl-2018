using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.GameLogic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	public class HealthDisplayer : MonoBehaviour {

		private static Color _freshGreen;
		private ActorBehaviour _actorBehaviour;
		private Canvas _canvas;

		public Image BarValue;

		// Use this for initialization
		void Start () {
			_canvas = GetComponent<Canvas>();
			_actorBehaviour = transform.parent.GetComponent<ActorBehaviour>();
			_freshGreen = new Color(.3f, 1f, .13f);
		}
	
		// Update is called once per frame
		void Update ()
		{
			float healthToMaxHealth = ((float)_actorBehaviour.ActorData.Health) / _actorBehaviour.ActorData.MaxHealth;
			BarValue.fillAmount = healthToMaxHealth;
			_canvas.enabled = healthToMaxHealth < 1;

			Color barColor = ColorUtilities.Lerp3(Color.red, Color.yellow, _freshGreen, healthToMaxHealth);
			BarValue.color = barColor;
		}
	}
}
