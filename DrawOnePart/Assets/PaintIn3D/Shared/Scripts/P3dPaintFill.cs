using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component implements the replace paint mode, which will modify all pixels in the specified texture in the same way.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dPaintFill")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Paint/Paint Fill")]
	public class P3dPaintFill : MonoBehaviour, IHit, IHitPoint
	{
		public class Command : P3dCommand
		{
			public static Command Instance = new Command();

			public Texture Texture;
			public Color   Color;
			public float   Opacity;
			public float   Minimum;

			private static Stack<Command> pool = new Stack<Command>();

			private static Material[] cachedMaterials;

			public override bool RequireMesh { get { return false; } }

			static Command()
			{
				cachedMaterials = P3dPaintableManager.BuildMaterialsBlendModes("Hidden/Paint in 3D/Fill");
			}

			public override void Apply()
			{
				Blend.Apply(Material);

				Material.SetTexture(P3dShader._Texture, Texture);
				Material.SetColor(P3dShader._Color, Color);
				Material.SetFloat(P3dShader._Opacity, Opacity);
				Material.SetVector(P3dShader._Minimum, new Vector4(Minimum, Minimum, Minimum, Minimum));
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
				command.Opacity = Opacity;
				command.Minimum = Minimum;

				return command;
			}

			public void SetMaterial(P3dBlendMode blendMode, Texture texture, Color color, float opacity, float minimum)
			{
				Blend    = blendMode;
				Material = cachedMaterials[blendMode];
				Texture  = texture;
				Color    = color;
				Opacity  = opacity;
				Minimum  = minimum;
			}
		}

		public enum RotationType
		{
			World,
			Normal
		}

		/// <summary>Only the <b>P3dPaintableTexture</b> components with a matching group will be painted by this component.</summary>
		public P3dGroup Group { set { group = value; } get { return group; } } [SerializeField] private P3dGroup group;

		/// <summary>This component will paint using this blending mode.
		/// NOTE: See <b>P3dBlendMode</b> documentation for more information.</summary>
		public P3dBlendMode BlendMode { set { blendMode = value; } get { return blendMode; } } [SerializeField] private P3dBlendMode blendMode = P3dBlendMode.AlphaBlend;

		/// <summary>The color of the paint.</summary>
		public Texture Texture { set { texture = value; } get { return texture; } } [SerializeField] private Texture texture;

		/// <summary>The color of the paint.</summary>
		public Color Color { set { color = value; } get { return color; } } [SerializeField] private Color color = Color.white;

		/// <summary>The opacity of the brush.</summary>
		public float Opacity { set { opacity = value; } get { return opacity; } } [Range(0.0f, 1.0f)] [SerializeField] private float opacity = 1.0f;

		/// <summary>The minimum RGBA value change. This is useful if you're doing very subtle color changes over time.</summary>
		public float Minimum { set { minimum = value; } get { return minimum; } } [Range(0.0f, 1.0f)] [SerializeField] private float minimum;

		/// <summary>This method multiplies the radius by the specified value.</summary>
		public void IncrementOpacity(float delta)
		{
			opacity = Mathf.Clamp01(opacity + delta);
		}

		public static RenderTexture Blit(RenderTexture main, P3dBlendMode blendMode, Texture texture, Color color, float opacity, float minimum)
		{
			var swap = P3dHelper.GetRenderTexture(main.descriptor);

			Blit(ref main, ref swap, blendMode, texture, color, opacity, minimum);

			P3dHelper.ReleaseRenderTexture(swap);

			return main;
		}

		public static void Blit(ref RenderTexture main, ref RenderTexture swap, P3dBlendMode blendMode, Texture texture, Color color, float opacity, float minimum)
		{
			Command.Instance.SetMaterial(blendMode, texture, color, opacity, minimum);

			Command.Instance.Apply(main);

			P3dHelper.Blit(swap, Command.Instance.Material);

			P3dHelper.Swap(ref main, ref swap);
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
						var finalOpacity = opacity;
						var finalTexture = texture;

						P3dPaintableManager.BuildModifiers(gameObject);
						P3dPaintableManager.ModifyColor(pressure, ref finalColor);
						P3dPaintableManager.ModifyOpacity(pressure, ref finalOpacity);
						P3dPaintableManager.ModifyTexture(pressure, ref finalTexture);

						Command.Instance.SetState(preview, priority);
						Command.Instance.SetMaterial(blendMode, finalTexture, finalColor, opacity, minimum);

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
	[CustomEditor(typeof(P3dPaintFill))]
	public class P3dPaintFill_Editor : P3dEditor<P3dPaintFill>
	{
		protected override void OnInspector()
		{
			Draw("group", "Only the P3dPaintableTexture components with a matching group will be painted by this component.");

			Separator();

			Draw("blendMode", "This component will paint using this blending mode.\n\nNOTE: See P3dBlendMode documentation for more information.");
			Draw("texture", "The texture of the paint.");
			Draw("color", "The color of the paint.");
			Draw("opacity", "The opacity of the brush.");
			Draw("minimum", "The minimum RGBA value change. This is useful if you're doing very subtle color changes over time.");
		}
	}
}
#endif