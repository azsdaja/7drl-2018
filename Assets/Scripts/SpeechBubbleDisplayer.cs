using Assets.Scripts.GameLogic;
using UnityEngine;

namespace Assets.Scripts
{
	public class SpeechBubbleDisplayer : MonoBehaviour
	{
		private ActorBehaviour _actorBehaviour;

		private SpriteRenderer _bubbleRenderer;

		// Use this for initialization
		void Start ()
		{
			_actorBehaviour = transform.parent.GetComponent<ActorBehaviour>();
		}
	
		// Update is called once per frame
		void Update () {
		}
	}
}
