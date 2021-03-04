#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace PaintIn3D
{
	public partial class P3dPainter
	{
		partial class SettingsData
		{
			public float PaintScale = 1.0f;

			public float PaintTile = 1.0f;

			public Color PaintColor = Color.white;
		}

		private static float LogSlider(string title, float value, float logMin, float logMax)
		{
			var rect   = P3dHelper.Reserve();
			var rectA  = rect; rectA.width = EditorGUIUtility.labelWidth + 50;
			var rectB  = rect; rectB.xMin += EditorGUIUtility.labelWidth + 52;
			var logOld = Mathf.Log10(value);
			var logNew = GUI.HorizontalSlider(rectB, logOld, logMin, logMax);

			if (logOld != logNew)
			{
				value = Mathf.Pow(10.0f, logNew);
			}

			return EditorGUI.FloatField(rectA, title, value);
		}

		private static float Slider(string title, float value, float min, float max)
		{
			var rect  = P3dHelper.Reserve();
			var rectA = rect; rectA.width = EditorGUIUtility.labelWidth + 50;
			var rectB = rect; rectB.xMin += EditorGUIUtility.labelWidth + 52;

			value = GUI.HorizontalSlider(rectB, value, min, max);

			return EditorGUI.FloatField(rectA, title, value);
		}

		private void UpdateDynamicsPanel()
		{
			P3dHelper.BeginLabelWidth(50);
				settings.PaintScale = LogSlider("Scale", settings.PaintScale, -2, 4);
				settings.PaintTile  = LogSlider("Tiling" , settings.PaintTile , -2, 4);

				EditorGUILayout.Separator();

				settings.PaintColor   = EditorGUILayout.ColorField("Color", settings.PaintColor);
				settings.PaintColor.r = Slider("Red", settings.PaintColor.r, 0.0f, 1.0f);
				settings.PaintColor.g = Slider("Green", settings.PaintColor.g, 0.0f, 1.0f);
				settings.PaintColor.b = Slider("Blue", settings.PaintColor.b, 0.0f, 1.0f);
				settings.PaintColor.a = Slider("Alpha", settings.PaintColor.a, 0.0f, 1.0f);

				float h, s, v; Color.RGBToHSV(settings.PaintColor, out h, out s, out v);

				h = Slider("Hue"       , h, 0.0f, 1.0f);
				s = Slider("Saturation", s, 0.0f, 1.0f);
				v = Slider("Value"     , v, 0.0f, 1.0f);

				var newColor = Color.HSVToRGB(h, s, v);

				settings.PaintColor.r = newColor.r;
				settings.PaintColor.g = newColor.g;
				settings.PaintColor.b = newColor.b;

			P3dHelper.EndLabelWidth();
		}
	}
}
#endif