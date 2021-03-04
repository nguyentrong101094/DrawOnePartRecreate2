using UnityEngine;

namespace PaintIn3D
{
	/// <summary>This is the base class for all components that repeat paint commands (e.g. mirroring).</summary>
	public abstract class P3dClone : P3dLinkedBehaviour<P3dClone>, IClone
	{
		public abstract void Transform(ref Matrix4x4 posMatrix, ref Matrix4x4 rotMatrix);
	}
}