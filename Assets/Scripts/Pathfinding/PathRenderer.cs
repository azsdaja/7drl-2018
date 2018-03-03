using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.UnityUtilities;
using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
	public class PathRenderer : IPathRenderer
	{
		private readonly Material _lineMaterial;
		private readonly IGridInfoProvider _gridInfoProvider;
		private readonly INaturalLineCalculator _naturalLineCalculator;

		public PathRenderer(Material lineMaterial, IGridInfoProvider gridInfoProvider, INaturalLineCalculator naturalLineCalculator)
		{
			_lineMaterial = lineMaterial;
			_gridInfoProvider = gridInfoProvider;
			_naturalLineCalculator = naturalLineCalculator;
		}

		public void ShowPath(IList<Vector2Int> pathPoints, float score)
		{
			if (!pathPoints.Any()) return;
			var line = new GameObject("PathFragment");
			var lineRenderer = line.AddComponent<LineRenderer>();
			SetupLineRenderer(pathPoints, lineRenderer);
			int middlePathPointIndex = pathPoints.Count / 2;
			Vector2Int labelPositionInGrid = pathPoints[middlePathPointIndex];
			Vector3 labelPosition = _gridInfoProvider.GetCellCenterWorld(labelPositionInGrid);
			line.transform.position = labelPosition;
			var label = line.AddComponent<Label>();
			label.Text = score.ToString();

			foreach (var vector2Int in pathPoints)
			{
				var node = new GameObject("PathNode");
				SpriteRenderer nodeSprite = node.AddComponent<SpriteRenderer>();
				nodeSprite.sortingLayerName = "HUD";
				nodeSprite.transform.position = _gridInfoProvider.GetCellCenterWorld(vector2Int);
				nodeSprite.sprite = Resources.Load<Sprite>(@"Sprites\Characters\Misc_tiles\19");
				Object.Destroy(node.gameObject, 10.0f);
			}

			Object.Destroy(line.gameObject, 8.0f);
		}

		public void ShowNaturalWay(List<Vector2Int> jumpPoints, float score)
		{
			IList<Vector2Int> naturalWay = _naturalLineCalculator.GetFirstLongestNaturalLine(jumpPoints, _gridInfoProvider.IsWalkable);

			var line = new GameObject("PathFragment");
			var lineRenderer = line.AddComponent<LineRenderer>();
			SetupLineRenderer(naturalWay, lineRenderer);
			lineRenderer.startColor = Color.blue;
			lineRenderer.endColor = Color.blue;

			Object.Destroy(line.gameObject, 2.5f);
		}

		private void SetupLineRenderer(IList<Vector2Int> pathPoints, LineRenderer lineRenderer)
		{
			lineRenderer.startColor = Color.green;
			lineRenderer.endColor = Color.green;
			lineRenderer.material = _lineMaterial;
			lineRenderer.sortingLayerName = "HUD";
			IList<Vector3> jumpPointsInWorld = pathPoints.Select(p => _gridInfoProvider.GetCellCenterWorld(p)).ToList();
			lineRenderer.startWidth = .05f;
			lineRenderer.endWidth = .05f;
			lineRenderer.positionCount = jumpPointsInWorld.Count;
			lineRenderer.SetPositions(jumpPointsInWorld.ToArray());
		}
	}
}
