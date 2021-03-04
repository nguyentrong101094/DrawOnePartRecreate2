﻿using UnityEngine;

namespace PaintIn3D
{
	/// <summary>This interface allows you to make components that can paint 3D points with a specified orientation.</summary>
	public interface IHitPoint
	{
		void HandleHitPoint(bool preview, int priority, Collider collider, Vector3 position, Quaternion rotation, float pressure);
	}
}