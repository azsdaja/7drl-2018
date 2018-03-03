using UnityEngine;

namespace Assets.Scripts
{
	public class SceneRestarter : MonoBehaviour
	{

		public void RestartScene()
		{
			Application.LoadLevel(0);
		}
	}
}
