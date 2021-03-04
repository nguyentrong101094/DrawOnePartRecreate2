using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component allows you to change the painting hardness based on the paint pressure.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dModifyHardnessPressure")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Modify/Modify Hardness Pressure")]
	public class P3dModifyHardnessPressure : MonoBehaviour, IModify, IModifyHardness
	{
		public enum BlendType
		{
			Replace,
			Multiply,
			Increment
		}

		/// <summary>The change in hardness when the pressure is 1, based on the current blend.</summary>
		public float Hardness { set { hardness = value; } get { return hardness; } } [SerializeField] private float hardness = 1.0f;

		/// <summary>The way the hardness value will be blended with the current one.</summary>
		public BlendType Blend { set { blend = value; } get { return blend; } } [SerializeField] private BlendType blend;

		public void ModifyHardness(float pressure, ref float currentHardness)
		{
			var targetHardness = default(float);

			switch (blend)
			{
				case BlendType.Replace:
				{
					targetHardness = hardness;
				}
				break;

				case BlendType.Multiply:
				{
					targetHardness = currentHardness * hardness;
				}
				break;

				case BlendType.Increment:
				{
					targetHardness = currentHardness + hardness;
				}
				break;
			}

			currentHardness += (targetHardness - currentHardness) * pressure;
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dModifyHardnessPressure))]
	public class P3dModifyHardnessPressure_Editor : P3dEditor<P3dModifyHardnessPressure>
	{
		protected override void OnInspector()
		{
			Draw("hardness", "The change in hardness when the pressure is 1, based on the current blend.");
			Draw("blend", "The way the hardness value will be blended with the current one.");
		}
	}
}
#endif