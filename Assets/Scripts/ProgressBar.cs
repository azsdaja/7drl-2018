using System;
using Assets.Scripts.CSharpUtilities;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	public class ProgressBar : MonoBehaviour
	{
		private bool _active = true;
		private static Color _freshGreen;
		private Func<float> _progressGetter;
		private float _lastValue;

		public Image BarValue;

		public void Initialize(Func<float> progressGetter)
		{
			_progressGetter = progressGetter;
		}

		// Use this for initialization
		void Start ()
		{
			_freshGreen = new Color(.3f, 1f, .13f);
		}
	
		// Update is called once per frame
		void Update ()
		{
			if (!_active) return;
			if (_progressGetter == null) return;

			float progress = _progressGetter();
			if (Math.Abs(progress - _lastValue) < 0.01f) return;

			_lastValue = progress;
			BarValue.fillAmount = progress;
			Color barColor = ColorUtilities.Lerp3(Color.red, Color.yellow, _freshGreen, progress);
			BarValue.color = barColor;
		}
	}
}
