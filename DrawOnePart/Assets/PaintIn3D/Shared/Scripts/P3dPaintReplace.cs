using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component implements the replace paint mode, which will replace all pixels in the specified texture.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dPaintReplace")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Paint/Paint Replace")]
	public class P3dPaintReplace : MonoBehaviour, IHit, IHitPoint
	{
		public class Command : P3dCommand
		{
			public static Command Instance = new Command();
			
			public Texture Texture;
			public Color   Color;

			private static Stack<Command> pool = new Stack<Command>();

			private static Material cachedMaterial;

			public override bool RequireMesh { get { return false; } }

			static Command()
			{
				cachedMaterial = P3dPaintableManager.BuildMaterial("Hidden/Paint in 3D/Replace");
			}

			public override void Apply()
			{
				Material.SetTexture(P3dShader._Texture, Texture);
				Material.SetColor(P3dShader._Color, Color);
			}

			public override void Pool()
			{
				pool.Push(this);
			}

			public override void Transform(Matrix4x4 posMatrix, Matrix4x4 rotMatrix)
			{
			}

			public override P3dCommand SpawnCopy()
			{
				var command = SpawnCopy(pool);

				command.Texture = Texture;
				command.Color   = Color;

				return command;
			}

			public void SetMaterial(Texture texture, Color color)
			{
				Blend    = P3dBlendMode.Replace;
				Material = cachedMaterial;
				Texture  = texture;
				Color    = color;
			}
		}

		/// <summary>Only the <b>P3dPaintableTexture</b> components with a matching group will be painted by this component.</summary>
		public P3dGroup Group { set { group = value; } get { return group; } } [SerializeField] private P3dGroup group;

		/// <summary>The texture that will be painted.</summary>
		public Texture Texture { set { texture = value; } get { return texture; } } [SerializeField] private Texture texture;

		/// <summary>The color of the paint.</summary>
		public Color Color { set { color = value; } get { return color; } } [SerializeField] private Color color = Color.white;

		public static void Blit(RenderTexture renderTexture, Texture texture, Color tint)
		{
			Command.Instance.SetMaterial(texture, tint);

			Command.Instance.Apply();

			P3dHelper.Blit(renderTexture, Command.Instance.Material);
		}

		public static void BlitFast(RenderTexture renderTexture, Texture texture, Color tint)
		{
			Command.Instance.SetMaterial(texture, tint);

			Command.Instance.Apply();

			Graphics.Blit(default(Texture), renderTexture, Command.Instance.Material);
		}

		public void HandleHitPoint(bool preview, int priority, Collider collider, Vector3 worldPosition, Quaternion worldRotation, float pressure)
		{
			if (collider != null)
			{
				var model = collider.GetComponent<P3dModel>();

				if (model != null)
				{
					var paintableTextures = P3dPaintableTexture.Filter(model, group);

					if (paintableTextures.Count > 0)
					{
						var finalColor   = color;
						var finalTexture = texture;

						P3dPaintableManager.BuildModifiers(gameObject);
						P3dPaintableManager.ModifyColor(pressure, ref finalColor);
						P3dPaintableManager.ModifyTexture(pressure, ref finalTexture);

						Command.Instance.SetState(preview, priority);
						Command.Instance.SetMaterial(finalTexture, finalColor);

						for (var i = paintableTextures.Count - 1; i >= 0; i--)
						{
							var paintableTexture = paintableTextures[i];

							P3dPaintableManager.Submit(Command.Instance, model, paintableTexture);
						}
					}
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dPaintReplace))]
	public class P3dPaintReplace_Editor : P3dEditor<P3dPaintReplace>
	{
		protected override void OnInspector()
		{
			Draw("group", "Only the P3dPaintableTexture components with a matching group will be painted by this component.");

			Separator();

			Draw("texture", "The texture that will be painted.");
			Draw("color", "The color of the paint.");
		}
	}
}
#endif