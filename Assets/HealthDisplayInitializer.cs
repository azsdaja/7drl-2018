using Assets.Scripts;
using Assets.Scripts.GameLogic;
using UnityEngine;

public class HealthDisplayInitializer : MonoBehaviour
{
	private ProgressBar _healthProgressBar;
	private ActorBehaviour _actorBehaviour;

	// Use this for initialization
	void Start ()
	{
		_healthProgressBar = GetComponent<ProgressBar>();
		_actorBehaviour = transform.parent.GetComponent<ActorBehaviour>();
		_healthProgressBar.Initialize(() => _actorBehaviour.ActorData.HealthProgress);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
