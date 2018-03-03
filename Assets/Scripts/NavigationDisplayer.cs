using System.Linq;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.ActionLoop.AI;
using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.Pathfinding;
using UnityEngine;
using Zenject;

namespace Assets.Scripts
{
	public class NavigationDisplayer : MonoBehaviour
	{
		private ActorBehaviour _actorBehaviour;
		private LineRenderer _pathToTargetRenderer;
		private SpriteRenderer _nextNodeRenderer;
		private LineRenderer _stepsToNextNodeRenderer;
		private SpriteRenderer _destinationRenderer;

		private IGridInfoProvider _gridInfoProvider;
		private IGameConfig _gameConfig;

		[Inject]
		public void Init(IGridInfoProvider gridInfoProvider, IGameConfig gameConfig)
		{
			_gridInfoProvider = gridInfoProvider;
			_gameConfig = gameConfig;
		}

		// Use this for initialization
		void Start ()
		{
			_actorBehaviour = transform.parent.GetComponent<ActorBehaviour>();
			_pathToTargetRenderer = transform.GetChild(0).GetComponent<LineRenderer>();
			_nextNodeRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
			_stepsToNextNodeRenderer = transform.GetChild(2).GetComponent<LineRenderer>();
			_destinationRenderer = transform.GetChild(3).GetComponent<SpriteRenderer>();
			gameObject.SetActive(_gameConfig.ModeConfig.ShowPaths);
		}

		// Update is called once per frame
		void Update ()
		{
			NavigationData navigationData = _actorBehaviour.ActorData.NavigationData;

			if (navigationData.Destination.HasValue)
			{
				_destinationRenderer.enabled = true;
				_destinationRenderer.transform.position = _gridInfoProvider.GetCellCenterWorld(navigationData.Destination.Value);
			}
			else _nextNodeRenderer.enabled = false;

			if (navigationData.PathToFollow != null)
			{
				_pathToTargetRenderer.positionCount = navigationData.PathToFollow.Count;
				_pathToTargetRenderer.SetPositions(navigationData.PathToFollow.Select(position => _gridInfoProvider.GetCellCenterWorld(position)).ToArray());
			}

			if (navigationData.NextNode.HasValue)
			{
				_nextNodeRenderer.enabled = true;
				_nextNodeRenderer.transform.position = _gridInfoProvider.GetCellCenterWorld(navigationData.NextNode.Value);
			}
			else _nextNodeRenderer.enabled = false;

			if (navigationData.StepsToNextNode != null)
			{
				_stepsToNextNodeRenderer.positionCount = navigationData.StepsToNextNode.Count;
				_stepsToNextNodeRenderer.SetPositions(navigationData.StepsToNextNode.Select(position => _gridInfoProvider.GetCellCenterWorld(position)).ToArray());
			}
		}
	}
}
