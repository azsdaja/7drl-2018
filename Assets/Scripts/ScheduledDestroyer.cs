using UnityEngine;

namespace Assets.Scripts
{
	public class ScheduledDestroyer : MonoBehaviour
	{
		[SerializeField] private float _secondsToDestroy;

		// Use this for initialization
		void Start () {
			Destroy(gameObject, _secondsToDestroy);
		}
	
		// Update is called once per frame
		void Update ()
		{
		}
	}
}
