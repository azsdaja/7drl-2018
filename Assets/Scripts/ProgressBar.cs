using System;
using Assets.Scripts.CSharpUtilities;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	public class ProgressBar : MonoBehaviour
	{
		private static Color _freshGreen;
		private Func<float> _progressGetter;
		private float _lastValue;
		private Canvas _canvas;

		public Image BarValue;

		public void Initialize(Func<float> progressGetter, float initialValue = 1f)
		{
			_progressGetter = progressGetter;
			_lastValue = initialValue;
			BarValue.fillAmount = initialValue;
		}

		// Use this for initialization
		void Start ()
		{
			_freshGreen = new Color(.3f, 1f, .13f);
			_canvas = GetComponent<Canvas>();
		}
	
		// Update is called once per frame
		void Update ()
		{
			if (_progressGetter == null)
				return;

			float progress = _progressGetter();

			if (progress >= 1)
			{
				_canvas.enabled = false;
				return;
			}
			_canvas.enabled = true;
			if (Math.Abs(progress - _lastValue) < 0.01f) return;

			_lastValue = progress;
			BarValue.fillAmount = progress;
			Color barColor = ColorUtilities.Lerp3(Color.red, Color.yellow, _freshGreen, progress);
			BarValue.color = barColor;
		}
	}
}
