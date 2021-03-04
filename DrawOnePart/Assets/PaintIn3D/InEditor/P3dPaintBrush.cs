using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	public class P3dPaintBrush : ScriptableObject
	{
		public string Category { set { category = value; } get { return category; } } [SerializeField] private string category;

		public Texture Shape { set { shape = value; } get { return shape; } } [SerializeField] private Texture shape;

		public P3dChannel ShapeChannel { set { shapeChannel = value; } get { return shapeChannel; } } [SerializeField] private P3dChannel shapeChannel;

		public Color Color { set { color = value; } get { return color; } } [SerializeField] private Color color = Color.white;

		public float Angle { set { angle = value; } get { return angle; } } [SerializeField] private float angle;

		public float Radius { set { radius = value; } get { return radius; } } [SerializeField] private float radius = 1.0f;

		private static List<P3dPaintBrush> cachedInstances = new List<P3dPaintBrush>();

		private static bool cachedInstancesSet;

		public static void UpdateCachedInstances()
		{
			cachedInstancesSet = true;

			cachedInstances.Clear();

#if UNITY_EDITOR
			foreach (var guid in AssetDatabase.FindAssets("t:P3dPaintBrush"))
			{
				var brush = AssetDatabase.LoadAssetAtPath<P3dPaintBrush>(AssetDatabase.GUIDToAssetPath(guid));

				if (brush != null)
				{
					cachedInstances.Add(brush);
				}
			}
#endif
		}

		/// <summary>This static property returns a list of all cached <b>P3dGroupData</b> instances.
		/// NOTE: This will be empty in-game.</summary>
		public static List<P3dPaintBrush> CachedInstances
		{
			get
			{
				if (cachedInstancesSet == false)
				{
					UpdateCachedInstances();
				}

				return cachedInstances;
			}
		}

#if UNITY_EDITOR
		[MenuItem("Assets/Create/Paint in 3D/Paint Brush")]
		private static void CreateAsset()
		{
			var asset = CreateInstance<P3dPaintBrush>();
			var guids = Selection.assetGUIDs;
			var path  = guids.Length > 0 ? AssetDatabase.GUIDToAssetPath(guids[0]) : null;

			if (string.IsNullOrEmpty(path) == true)
			{
				path = "Assets";
			}
			else if (AssetDatabase.IsValidFolder(path) == false)
			{
				path = System.IO.Path.GetDirectoryName(path);
			}

			var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + typeof(P3dPaintBrush).ToString() + ".asset");

			AssetDatabase.CreateAsset(asset, assetPathAndName);

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			EditorUtility.FocusProjectWindow();

			Selection.activeObject = asset; EditorGUIUtility.PingObject(asset);
		}
#endif
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dPaintBrush))]
	public class P3dPaintBrush_Editor : P3dEditor<P3dPaintBrush>
	{
		protected override void OnInspector()
		{
			Draw("category");

			EditorGUILayout.Separator();

			foreach (var t in Targets)
			{
				if (P3dPaintBrush.CachedInstances.Contains(t) == false)
				{
					P3dPaintBrush.CachedInstances.Add(t);
				}
			}
			EditorGUILayout.BeginHorizontal();
				BeginError(Any(t => t.Shape == null));
					Draw("shape");
				EndError();
				EditorGUILayout.PropertyField(serializedObject.FindProperty("shapeChannel"), GUIContent.none, GUILayout.Width(50));
			EditorGUILayout.EndHorizontal();
			Draw("color");
			Draw("angle");
			Draw("radius");
		}
	}
}
#endif