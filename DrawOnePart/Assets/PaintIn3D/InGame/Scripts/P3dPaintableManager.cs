using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component automatically updates all P3dModel and P3dPaintableTexture instances at the end of the frame, batching all paint operations together.</summary>
	[DisallowMultipleComponent]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dPaintableManager")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Paintable Manager")]
	public class P3dPaintableManager : P3dLinkedBehaviour<P3dPaintableManager>
	{
		[System.NonSerialized]
		public static int MatrixCount;

		[System.NonSerialized]
		public static int ClonerCount;

		[System.NonSerialized]
		private static List<Matrix4x4> tempPosMatrices = new List<Matrix4x4>();

		[System.NonSerialized]
		private static List<Matrix4x4> tempRotMatrices = new List<Matrix4x4>();

		[System.NonSerialized]
		private static List<IClone> tempCloners = new List<IClone>();

		[System.NonSerialized]
		private static List<IModify> tempModifiers = new List<IModify>();

		public static P3dPaintableManager GetOrCreateInstance()
		{
			if (InstanceCount == 0)
			{
				var paintableManager = new GameObject(typeof(P3dPaintableManager).Name);

				paintableManager.hideFlags = HideFlags.DontSave;
				
				paintableManager.AddComponent<P3dPaintableManager>();
			}

			return FirstInstance;
		}

		public static Material[] BuildMaterialsBlendModes(string shaderName, string keyword = null)
		{
			var shader    = P3dShader.Load(shaderName);
			var materials = new Material[P3dBlendMode.COUNT];
			
			for (var index = 0; index < P3dBlendMode.COUNT; index++)
			{
				var material = P3dShader.Build(shader);

				P3dShader.BuildBlendMode(material, index);

				materials[index] = material;
			}

			if (string.IsNullOrEmpty(keyword) == false)
			{
				for (var index = 0; index < P3dBlendMode.COUNT; index++)
				{
					materials[index].EnableKeyword(keyword);
				}
			}

			return materials;
		}

		public static Material BuildMaterial(string shaderName)
		{
			var shader = P3dShader.Load(shaderName);

			return P3dShader.Build(shader);
		}

		public static void BuildModifiers(GameObject gameObject)
		{
			gameObject.GetComponents(tempModifiers);
		}

		public static void ModifyColor(float pressure, ref Color color)
		{
			for (var i = 0; i < tempModifiers.Count; i++)
			{
				var modifier = tempModifiers[i] as IModifyColor;

				if (modifier != null)
				{
					modifier.ModifyColor(pressure, ref color);
				}
			}
		}

		public static void ModifyAngle(float pressure, ref float angle)
		{
			for (var i = 0; i < tempModifiers.Count; i++)
			{
				var modifier = tempModifiers[i] as IModifyAngle;

				if (modifier != null)
				{
					modifier.ModifyAngle(pressure, ref angle);
				}
			}
		}

		public static void ModifyOpacity(float pressure, ref float opacity)
		{
			for (var i = 0; i < tempModifiers.Count; i++)
			{
				var modifier = tempModifiers[i] as IModifyOpacity;

				if (modifier != null)
				{
					modifier.ModifyOpacity(pressure, ref opacity);
				}
			}
		}

		public static void ModifyHardness(float pressure, ref float hardness)
		{
			for (var i = 0; i < tempModifiers.Count; i++)
			{
				var modifier = tempModifiers[i] as IModifyHardness;

				if (modifier != null)
				{
					modifier.ModifyHardness(pressure, ref hardness);
				}
			}
		}

		public static void ModifyRadius(float pressure, ref float radius)
		{
			for (var i = 0; i < tempModifiers.Count; i++)
			{
				var modifier = tempModifiers[i] as IModifyRadius;

				if (modifier != null)
				{
					modifier.ModifyRadius(pressure, ref radius);
				}
			}
		}

		public static void ModifyTexture(float pressure, ref Texture texture)
		{
			for (var i = 0; i < tempModifiers.Count; i++)
			{
				var modifier = tempModifiers[i] as IModifyTexture;

				if (modifier != null)
				{
					modifier.ModifyTexture(pressure, ref texture);
				}
			}
		}

		public static void SubmitAll(P3dCommand command, Vector3 position, float radius, int layerMask, P3dGroup group, P3dModel targetModel, P3dPaintableTexture targetTexture)
		{
			DoSubmitAll(command, position, radius, layerMask, group, targetModel, targetTexture);

			// Repeat paint?
			BuildCloners();

			for (var c = 0; c < ClonerCount; c++)
			{
				for (var m = 0; m < MatrixCount; m++)
				{
					var copy = command.SpawnCopy();

					Clone(copy, c, m);

					DoSubmitAll(copy, position, radius, layerMask, group, targetModel, targetTexture);

					copy.Pool();
				}
			}
		}

		private static void DoSubmitAll(P3dCommand command, Vector3 position, float radius, int layerMask, P3dGroup group, P3dModel targetModel, P3dPaintableTexture targetTexture)
		{
			if (targetModel != null)
			{
				if (targetTexture != null)
				{
					Submit(command, targetModel, targetTexture);
				}
				else
				{
					SubmitAll(command, targetModel, group);
				}
			}
			else
			{
				if (targetTexture != null)
				{
					Submit(command, targetTexture.CachedPaintable, targetTexture);
				}
				else
				{
					SubmitAll(command, position, radius, layerMask, group);
				}
			}
		}

		private static void SubmitAll(P3dCommand command, Vector3 position, float radius, int layerMask, P3dGroup group)
		{
			var models = P3dModel.FindOverlap(position, radius, layerMask);

			for (var i = models.Count - 1; i >= 0; i--)
			{
				SubmitAll(command, models[i], group);
			}
		}

		private static void SubmitAll(P3dCommand command, P3dModel model, P3dGroup group)
		{
			var paintableTextures = P3dPaintableTexture.Filter(model, group);

			for (var i = paintableTextures.Count - 1; i >= 0; i--)
			{
				Submit(command, model, paintableTextures[i]);
			}
		}

		public static void Submit(P3dCommand command, P3dModel model, P3dPaintableTexture paintableTexture)
		{
			var copy = command.SpawnCopy();

			if (copy.Blend.Index == P3dBlendMode.REPLACE_ORIGINAL)
			{
				copy.Blend.Color   = paintableTexture.Color;
				copy.Blend.Texture = paintableTexture.Texture;
			}

			paintableTexture.AddCommand(copy);
		}

		public static void BuildCloners(List<IClone> cloners = null)
		{
			tempCloners.Clear();
			tempPosMatrices.Clear();
			tempRotMatrices.Clear();

			tempPosMatrices.Add(Matrix4x4.identity);
			tempRotMatrices.Add(Matrix4x4.identity);

			if (cloners != null)
			{
				for (var i = 0; i < cloners.Count; i++)
				{
					var cloner = cloners[i];

					if (cloner != null)
					{
						tempCloners.Add(cloner);
					}
				}
			}
			else
			{
				var cloner = P3dClone.FirstInstance;

				for (var i = 0; i < P3dClone.InstanceCount; i++)
				{
					tempCloners.Add(cloner);

					cloner = cloner.NextInstance;
				}
			}

			MatrixCount = 1;
			ClonerCount = tempCloners.Count;
		}

		public static void Clone(P3dCommand command, int clonerIndex, int matrixIndex)
		{
			if (matrixIndex == 0)
			{
				MatrixCount = tempPosMatrices.Count;
			}

			var posMatrix = tempPosMatrices[matrixIndex];
			var rotMatrix = tempRotMatrices[matrixIndex];

			tempCloners[clonerIndex].Transform(ref posMatrix, ref rotMatrix);

			tempPosMatrices.Add(posMatrix);
			tempRotMatrices.Add(rotMatrix);

			command.Transform(posMatrix, rotMatrix);
		}

		protected virtual void LateUpdate()
		{
			if (this == FirstInstance && P3dModel.InstanceCount > 0)
			{
				ClearAll();
				UpdateAll();
			}
			else
			{
				P3dHelper.Destroy(gameObject);
			}
		}

		private void ClearAll()
		{
			var model = P3dModel.FirstInstance;

			for (var i = 0; i < P3dModel.InstanceCount; i++)
			{
				model.Prepared = false;

				model = model.NextInstance;
			}
		}

		private void UpdateAll()
		{
			var paintableTexture = P3dPaintableTexture.FirstInstance;

			for (var i = 0; i < P3dPaintableTexture.InstanceCount; i++)
			{
				paintableTexture.ExecuteCommands(true);

				paintableTexture = paintableTexture.NextInstance;
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dPaintableManager))]
	public class P3dPaintableManager_Editor : P3dEditor<P3dPaintableManager>
	{
		[InitializeOnLoad]
		public class ExecutionOrder
		{
			static ExecutionOrder()
			{
				ForceExecutionOrder(100);
			}
		}

		protected override void OnInspector()
		{
			EditorGUILayout.HelpBox("This component automatically updates all P3dModel and P3dPaintableTexture instances at the end of the frame, batching all paint operations together.", MessageType.Info);
		}
	}
}
#endif