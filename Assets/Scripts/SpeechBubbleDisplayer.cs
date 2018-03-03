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
			if (_actorBehaviour.ActorData.NeedData.CurrentNeed == NeedType.Aggresion)
			{
				if (_bubbleRenderer == null)
				{
					var bubble = Resources.Load<SpriteRenderer>("Prefabs/SpeechBubble");
					_bubbleRenderer = Instantiate(bubble, transform);
					_bubbleRenderer.transform.localPosition = Vector3.zero;
				}
				_bubbleRenderer.enabled = _actorBehaviour.IsVisible;
			}
			else if (_bubbleRenderer != null)
			{
				_bubbleRenderer.enabled = false;
			}
		}
	}
}
