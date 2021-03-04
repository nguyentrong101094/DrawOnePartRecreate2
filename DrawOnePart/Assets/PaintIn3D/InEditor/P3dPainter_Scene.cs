#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace PaintIn3D
{
	public partial class P3dPainter
	{
		private void UpdateScenePanel()
		{
			P3dHelper.BeginLabelWidth(60.0f);
				cameraOrigin = EditorGUILayout.Vector3Field("Origin", cameraOrigin);
				cameraDistance = Mathf.Max(0.0001f, EditorGUILayout.FloatField("Distance", cameraDistance));

				previewUtil.ambientColor = EditorGUILayout.ColorField("Am Color", previewUtil.ambientColor);

				DrawLightData(settings.Light0);
				DrawLightData(settings.Light1);
			P3dHelper.EndLabelWidth();
		}

		private void DrawLightData(LightData lightData)
		{
			EditorGUILayout.Separator();

			lightData.Pitch     = EditorGUILayout.FloatField("Pitch", lightData.Pitch);
			lightData.Yaw       = EditorGUILayout.FloatField("Yaw", lightData.Yaw);
			lightData.Intensity = EditorGUILayout.FloatField("Intensity", lightData.Intensity);
			lightData.Color     = EditorGUILayout.ColorField("Color", lightData.Color);
		}
	}
}
#endif