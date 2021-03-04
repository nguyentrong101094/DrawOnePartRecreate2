using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component allows you to change the painting radius based on the paint pressure.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dModifyRadiusPressure")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Modify/Modify Radius Pressure")]
	public class P3dModifyRadiusPressure : MonoBehaviour, IModify, IModifyRadius
	{
		public enum BlendType
		{
			Replace,
			Multiply,
			Increment
		}

		/// <summary>The change in radius when the pressure is 1, based on the current blend.</summary>
		public float Radius { set { radius = value; } get { return radius; } } [SerializeField] private float radius = 1.0f;

		/// <summary>The way the radius value will be blended with the current one.</summary>
		public BlendType Blend { set { blend = value; } get { return blend; } } [SerializeField] private BlendType blend;

		public void ModifyRadius(float pressure, ref float currentRadius)
		{
			var targetRadius = default(float);

			switch (blend)
			{
				case BlendType.Replace:
				{
					targetRadius = radius;
				}
				break;

				case BlendType.Multiply:
				{
					targetRadius = currentRadius * radius;
				}
				break;

				case BlendType.Increment:
				{
					targetRadius = currentRadius + radius;
				}
				break;
			}

			currentRadius += (targetRadius - currentRadius) * pressure;
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dModifyRadiusPressure))]
	public class P3dModifyRadiusPressure_Editor : P3dEditor<P3dModifyRadiusPressure>
	{
		protected override void OnInspector()
		{
			Draw("radius", "The change in radius when the pressure is 1, based on the current blend.");
			Draw("blend", "The way the radius value will be blended with the current one.");
		}
	}
}
#endif