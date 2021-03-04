using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component allows you to change the painting opacity based on the paint pressure.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dModifyOpacityPressure")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Modify/Modify Opacity Pressure")]
	public class P3dModifyOpacityPressure : MonoBehaviour, IModify, IModifyOpacity
	{
		public enum BlendType
		{
			Replace,
			Multiply,
			Increment
		}

		/// <summary>The change in opacity when the pressure is 1, based on the current blend.</summary>
		public float Opacity { set { opacity = value; } get { return opacity; } } [SerializeField] private float opacity = 1.0f;

		/// <summary>The way the opacity value will be blended with the current one.</summary>
		public BlendType Blend { set { blend = value; } get { return blend; } } [SerializeField] private BlendType blend;

		public void ModifyOpacity(float pressure, ref float currentOpacity)
		{
			var targetOpacity = default(float);

			switch (blend)
			{
				case BlendType.Replace:
				{
					targetOpacity = opacity;
				}
				break;

				case BlendType.Multiply:
				{
					targetOpacity = currentOpacity * opacity;
				}
				break;

				case BlendType.Increment:
				{
					targetOpacity = currentOpacity + opacity;
				}
				break;
			}

			currentOpacity += (targetOpacity - currentOpacity) * pressure;
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dModifyOpacityPressure))]
	public class P3dModifyOpacityPressure_Editor : P3dEditor<P3dModifyOpacityPressure>
	{
		protected override void OnInspector()
		{
			Draw("opacity", "The change in opacity when the pressure is 1, based on the current blend.");
			Draw("blend", "The way the opacity value will be blended with the current one.");
		}
	}
}
#endif