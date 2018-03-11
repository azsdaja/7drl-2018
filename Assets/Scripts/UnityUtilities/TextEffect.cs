using TMPro;
using UnityEngine;

namespace Assets.Scripts.UnityUtilities
{
	public class TextEffect : MonoBehaviour
	{
		private string _text;

		private Color _initialColor;
		private Color _transparentInitialColor;
		private GUIStyle _guiStyle;

		private float _duration;
		private float _timePassed;

		public void Initialize(string text, float duration = 1f)
		{
			_text = text;
			_initialColor = Color.white;
			_transparentInitialColor = new Color(1, 1, 1, 0);
			_duration = duration;
		}

		public void Initialize(string text, Color color, float duration = 1f)
		{
			_text = text;
			_initialColor = color;
			_transparentInitialColor = new Color(color.r, color.g, color.b, 0);
			_duration = duration;
		}

		void Start()
		{
			Font font = Resources.Load<Font>("Fonts/SDS_8x8");
			_guiStyle = new GUIStyle { normal = { textColor = _initialColor }, font = font };
		}

		void OnGUI()
		{
			_timePassed += Time.unscaledDeltaTime;
			float progress = _timePassed / _duration;
			if (progress >= 1f)
			{
				Destroy(gameObject);
				return;
			}

			var currentColor = Color.Lerp(_initialColor, _transparentInitialColor, progress);
			_guiStyle.normal.textColor = currentColor;

			Vector3 positionOnScreen = GetPositionOnScreen(progress);

			var rect = new Rect(0, 0, 100, 60);
			int xTweak = -20;
			int yTweak = 30;
			rect.x = positionOnScreen.x + xTweak;
			rect.y = Screen.height - positionOnScreen.y - rect.height + yTweak;

			GUI.Label(rect, _text, _guiStyle);
		}

		private Vector3 GetPositionOnScreen(float progress)
		{
			const float finalYOffset = .1f;
			float currentPositionY = Mathf.Lerp(transform.position.y, transform.position.y + finalYOffset, progress);
			var currentPosition = new Vector3(transform.position.x, currentPositionY, transform.position.z);
			Vector3 positionOnScreen = Camera.main.WorldToScreenPoint(currentPosition);
			return positionOnScreen;
		}
	}
}
