using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This class stores lists of IHit__ instances, allowing components like P3dHit__ to easily invoke hit events.</summary>
	public class P3dHitCache
	{
		[System.NonSerialized]
		private bool cached;

		[System.NonSerialized]
		private List<IHitPoint> hitPoints = new List<IHitPoint>();

		[System.NonSerialized]
		private List<IHitLine> hitLines = new List<IHitLine>();

		[System.NonSerialized]
		private List<IHitQuad> hitQuads = new List<IHitQuad>();

		[System.NonSerialized]
		private List<IHitRaycast> hitRaycasts = new List<IHitRaycast>();

		[System.NonSerialized]
		private static List<IHit> hits = new List<IHit>();

		public bool Cached
		{
			get
			{
				return cached;
			}
		}
#if UNITY_EDITOR
		private static HashSet<object> tempHits = new HashSet<object>();

		public void Inspector(GameObject gameObject, bool direct, bool point, bool line, bool quad)
		{
			Cache(gameObject);

			tempHits.Clear();

			if (direct == true)
			{
				for (var i = 0; i < hitRaycasts.Count; i++)
				{
					tempHits.Add(hitRaycasts[i]);
				}
			}

			if (point == true)
			{
				for (var i = 0; i < hitPoints.Count; i++)
				{
					tempHits.Add(hitPoints[i]);
				}
			}

			if (line == true)
			{
				for (var i = 0; i < hitLines.Count; i++)
				{
					tempHits.Add(hitLines[i]);
				}
			}

			if (quad == true)
			{
				for (var i = 0; i < hitQuads.Count; i++)
				{
					tempHits.Add(hitQuads[i]);
				}
			}

			if (hits.Count == 0)
			{
				EditorGUILayout.HelpBox("This component isn't sending hit events to anything.", MessageType.Warning);
			}
			else
			{
				var output = "This component is sending hit events to:";

				foreach (var hit in tempHits)
				{
					output += "\n" + hit;
				}

				EditorGUILayout.HelpBox(output, MessageType.Info);
			}
		}
#endif
		public void InvokePoints(GameObject gameObject, bool preview, int priority, Collider collider, Vector3 position, Quaternion rotation, float pressure)
		{
			if (cached == false)
			{
				Cache(gameObject);
			}

			for (var i = 0; i < hitPoints.Count; i++)
			{
				hitPoints[i].HandleHitPoint(preview, priority, collider, position, rotation, pressure);
			}
		}

		public void InvokeLines(GameObject gameObject, bool preview, int priority, Vector3 position, Vector3 endPosition, Quaternion rotation, float pressure)
		{
			if (cached == false)
			{
				Cache(gameObject);
			}

			for (var i = 0; i < hitLines.Count; i++)
			{
				hitLines[i].HandleHitLine(preview, priority, position, endPosition, rotation, pressure);
			}
		}

		public void InvokeQuads(GameObject gameObject, bool preview, int priority, Vector3 position, Vector3 endPosition, Vector3 position2, Vector3 endPosition2, Quaternion rotation, float pressure)
		{
			if (cached == false)
			{
				Cache(gameObject);
			}

			for (var i = 0; i < hitQuads.Count; i++)
			{
				hitQuads[i].HandleHitQuad(preview, priority, position, endPosition, position2, endPosition2, rotation, pressure);
			}
		}

		public void InvokeRaycast(GameObject gameObject, bool preview, int priority, RaycastHit hit, float pressure)
		{
			if (cached == false)
			{
				Cache(gameObject);
			}

			for (var i = 0; i < hitRaycasts.Count; i++)
			{
				hitRaycasts[i].HandleHitRaycast(preview, priority, hit, pressure);
			}
		}

		public void Clear()
		{
			cached = false;

			hitPoints.Clear();
			hitLines.Clear();
			hitQuads.Clear();
			hitRaycasts.Clear();
		}

		private void Cache(GameObject gameObject)
		{
			cached = true;

			gameObject.GetComponentsInChildren(hits);

			hitPoints.Clear();
			hitLines.Clear();
			hitQuads.Clear();
			hitRaycasts.Clear();

			for (var i = 0; i < hits.Count; i++)
			{
				var hit = hits[i];

				var hitPoint = hit as IHitPoint; if (hitPoint != null) { hitPoints.Add(hitPoint); }

				var hitLine = hit as IHitLine; if (hitLine != null) { hitLines.Add(hitLine); }

				var hitQuad = hit as IHitQuad; if (hitQuads != null) { hitQuads.Add(hitQuad); }

				var hitDirect = hit as IHitRaycast; if (hitDirect != null) { hitRaycasts.Add(hitDirect); }
			}
		}
	}
}