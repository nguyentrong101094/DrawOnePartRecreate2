using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This allows you to paint a sphere at a hit point. A hit point can be found using a companion component like: P3dDragRaycast, P3dOnCollision, P3dOnParticleCollision.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dPaintSphere")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Paint/Paint Sphere")]
	public class P3dPaintSphere : MonoBehaviour, IHit, IHitPoint, IHitLine, IHitQuad
	{
		public class Command : P3dCommand
		{
			public static Command Instance = new Command();

			public Vector3   Position;
			public Vector3   EndPosition;
			public Vector3   Position2;
			public Vector3   EndPosition2;
			public int       Extrusions;
			public Matrix4x4 Matrix;
			public Color     Color;
			public float     Opacity;
			public float     Hardness;
			public Texture   TileTexture;
			public Matrix4x4 TileMatrix;
			public float     TileBlend;

			private static Stack<Command> pool = new Stack<Command>();

			private static Material[] cachedSpotMaterials;

			private static Material[] cachedLineMaterials;

			private static Material[] cachedQuadMaterials;

			public override bool RequireMesh { get { return true; } }

			static Command()
			{
				cachedSpotMaterials = P3dPaintableManager.BuildMaterialsBlendModes("Hidden/Paint in 3D/Sphere");
				cachedLineMaterials = P3dPaintableManager.BuildMaterialsBlendModes("Hidden/Paint in 3D/Sphere", "P3D_LINE");
				cachedQuadMaterials = P3dPaintableManager.BuildMaterialsBlendModes("Hidden/Paint in 3D/Sphere", "P3D_QUAD");
			}

			public override void Apply()
			{
				Blend.Apply(Material);
				
				Material.SetVector(P3dShader._Position, Position);
				Material.SetVector(P3dShader._EndPosition, EndPosition);
				Material.SetVector(P3dShader._Position2, Position2);
				Material.SetVector(P3dShader._EndPosition2, EndPosition2);
				Material.SetMatrix(P3dShader._Matrix, Matrix.inverse);
				Material.SetColor(P3dShader._Color, Color);
				Material.SetFloat(P3dShader._Opacity, Opacity);
				Material.SetFloat(P3dShader._Hardness, Hardness);
				Material.SetTexture(P3dShader._TileTexture, TileTexture);
				Material.SetMatrix(P3dShader._TileMatrix, TileMatrix);
				Material.SetFloat(P3dShader._TileBlend, TileBlend);
			}

			public override void Pool()
			{
				pool.Push(this);
			}

			public override void Transform(Matrix4x4 posMatrix, Matrix4x4 rotMatrix)
			{
				Position     = posMatrix.MultiplyPoint(Position);
				EndPosition  = posMatrix.MultiplyPoint(EndPosition);
				Position2    = posMatrix.MultiplyPoint(Position2);
				EndPosition2 = posMatrix.MultiplyPoint(EndPosition2);
				Matrix       = rotMatrix * Matrix;
			}

			public override P3dCommand SpawnCopy()
			{
				var command = SpawnCopy(pool);

				command.Position     = Position;
				command.EndPosition  = EndPosition;
				command.Position2    = Position2;
				command.EndPosition2 = EndPosition2;
				command.Extrusions   = Extrusions;
				command.Matrix       = Matrix;
				command.Color        = Color;
				command.Opacity      = Opacity;
				command.Hardness     = Hardness;
				command.TileTexture  = TileTexture;
				command.TileMatrix   = TileMatrix;
				command.TileBlend    = TileBlend;

				return command;
			}

			public void SetShape(float radius)
			{
				Matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * radius);
			}

			public void SetShape(Vector3 radius, Quaternion rotation)
			{
				Matrix = Matrix4x4.TRS(Vector3.zero, rotation, radius);
			}

			public void SetLocation(Vector3 position)
			{
				Extrusions = 0;
				Position   = position;
			}

			public void SetLocation(Vector3 position, Vector3 endPosition)
			{
				Extrusions  = 1;
				Position    = position;
				EndPosition = endPosition;
			}

			public void SetLocation(Vector3 position, Vector3 endPosition, Vector3 position2, Vector3 endPosition2)
			{
				Extrusions   = 2;
				Position     = position;
				EndPosition  = endPosition;
				Position2    = position2;
				EndPosition2 = endPosition2;
			}

			public void SetMaterial(P3dBlendMode blendMode, float hardness, Color color, float opacity, Texture tileTexture, Matrix4x4 tileMatrix, float tileBlend)
			{
				switch (Extrusions)
				{
					case 0: Material = cachedSpotMaterials[blendMode]; break;
					case 1: Material = cachedLineMaterials[blendMode]; break;
					case 2: Material = cachedQuadMaterials[blendMode]; break;
				}

				Blend       = blendMode;
				Hardness    = hardness;
				Color       = color;
				Opacity     = opacity;
				TileTexture = tileTexture;
				TileMatrix  = tileMatrix;
				TileBlend   = tileBlend;
			}
		}

		public enum RotationType
		{
			World,
			Normal
		}

		/// <summary>Only the P3dModel/P3dPaintable GameObjects whose layers are within this mask will be eligible for painting.</summary>
		public LayerMask Layers { set { layers = value; } get { return layers; } } [SerializeField] private LayerMask layers = -1;

		/// <summary>Only the <b>P3dPaintableTexture</b> components with a matching group will be painted by this component.</summary>
		public P3dGroup Group { set { group = value; } get { return group; } } [SerializeField] private P3dGroup group;

		/// <summary>If this is set, then only the specified P3dModel/P3dPaintable will be painted, regardless of the layer setting.</summary>
		public P3dModel TargetModel { set { targetModel = value; } get { return targetModel; } } [SerializeField] private P3dModel targetModel;

		/// <summary>If this is set, then only the specified P3dPaintableTexture will be painted, regardless of the layer or group setting.</summary>
		public P3dPaintableTexture TargetTexture { set { targetTexture = value; } get { return targetTexture; } } [SerializeField] private P3dPaintableTexture targetTexture;

		/// <summary>This component will paint using this blending mode.
		/// NOTE: See <b>P3dBlendMode</b> documentation for more information.</summary>
		public P3dBlendMode BlendMode { set { blendMode = value; } get { return blendMode; } } [SerializeField] private P3dBlendMode blendMode = P3dBlendMode.AlphaBlend;

		/// <summary>The color of the paint.</summary>
		public Color Color { set { color = value; } get { return color; } } [SerializeField] private Color color = Color.white;

		/// <summary>The opacity of the brush.</summary>
		public float Opacity { set { opacity = value; } get { return opacity; } } [Range(0.0f, 1.0f)] [SerializeField] private float opacity = 1.0f;

		/// <summary>If you want to override the scale of the sphere to paint an ellipse, then set the scale here.</summary>
		public Vector3 Scale { set { scale = value; } get { return scale; } } [SerializeField] private Vector3 scale = Vector3.one;

		/// <summary>This allows you to control how the ellipse is rotated.</summary>
		public RotationType RotateTo { set { rotateTo = value; } get { return rotateTo; } } [SerializeField] private RotationType rotateTo;

		/// <summary>The radius of the paint brush.</summary>
		public float Radius { set { radius = value; } get { return radius; } } [SerializeField] private float radius = 0.1f;

		/// <summary>The hardness of the paint brush.</summary>
		public float Hardness { set { hardness = value; } get { return hardness; } } [SerializeField] private float hardness = 1.0f;

		/// <summary>This allows you to apply a tiled detail texture to your decals. This tiling will be applied in world space using triplanar mapping.</summary>
		public Texture TileTexture { set { tileTexture = value; } get { return tileTexture; } } [SerializeField] private Texture tileTexture;

		/// <summary>This allows you to adjust the tiling position + rotation + scale using a <b>Transform</b>.</summary>
		public Transform TileTransform { set { tileTransform = value; } get { return tileTransform; } } [SerializeField] private Transform tileTransform;

		/// <summary>This allows you to control the triplanar mapping sharpness between each axis.</summary>
		public float TileBlend { set { tileBlend = value; } get { return tileBlend; } } [Range(1.0f, 10.0f)] [SerializeField] private float tileBlend = 3.0f;

		/// <summary>This method multiplies the radius by the specified value.</summary>
		public void IncrementOpacity(float delta)
		{
			opacity = Mathf.Clamp01(opacity + delta);
		}

		/// <summary>This method multiplies the radius by the specified value.</summary>
		public void MultiplyRadius(float multiplier)
		{
			radius *= multiplier;
		}

		/// <summary>This method multiplies the scale by the specified value.</summary>
		public void MultiplyScale(float multiplier)
		{
			scale *= multiplier;
		}

		/// <summary>This method paints all pixels at the specified point using the shape of a sphere.</summary>
		public void HandleHitPoint(bool preview, int priority, Collider collider, Vector3 position, Quaternion rotation, float pressure)
		{
			Command.Instance.SetState(preview, priority);
			Command.Instance.SetLocation(position);

			var worldSize     = HandleHitCommon(rotation, pressure);
			var worldRadius   = P3dHelper.GetRadius(worldSize);
			var worldPosition = position;

			P3dPaintableManager.SubmitAll(Command.Instance, worldPosition, worldRadius, layers, group, targetModel, targetTexture);
		}

		/// <summary>This method paints all pixels between the two specified points using the shape of a sphere.</summary>
		public void HandleHitLine(bool preview, int priority, Vector3 position, Vector3 endPosition, Quaternion rotation, float pressure)
		{
			Command.Instance.SetState(preview, priority);
			Command.Instance.SetLocation(position, endPosition);

			var worldSize     = HandleHitCommon(rotation, pressure);
			var worldRadius   = P3dHelper.GetRadius(worldSize, position, endPosition);
			var worldPosition = P3dHelper.GetPosition(position, endPosition);

			P3dPaintableManager.SubmitAll(Command.Instance, worldPosition, worldRadius, layers, group, targetModel, targetTexture);
		}

		/// <summary>This method paints all pixels between two pairs of points using the shape of a sphere.</summary>
		public void HandleHitQuad(bool preview, int priority, Vector3 position, Vector3 endPosition, Vector3 position2, Vector3 endPosition2, Quaternion rotation, float pressure)
		{
			Command.Instance.SetState(preview, priority);
			Command.Instance.SetLocation(position, endPosition, position2, endPosition2);

			var worldSize     = HandleHitCommon(rotation, pressure);
			var worldRadius   = P3dHelper.GetRadius(worldSize, position, endPosition, position2, endPosition2);
			var worldPosition = P3dHelper.GetPosition(position, endPosition, position2, endPosition2);

			P3dPaintableManager.SubmitAll(Command.Instance, worldPosition, worldRadius, layers, group, targetModel, targetTexture);
		}

		private Vector3 HandleHitCommon(Quaternion rotation, float pressure)
		{
			var finalOpacity    = opacity;
			var finalRadius     = radius;
			var finalHardness   = hardness;
			var finalColor      = color;
			var finalTileMatrix = tileTransform != null ? tileTransform.localToWorldMatrix : Matrix4x4.identity;

			P3dPaintableManager.BuildModifiers(gameObject);
			P3dPaintableManager.ModifyColor(pressure, ref finalColor);
			P3dPaintableManager.ModifyOpacity(pressure, ref finalOpacity);
			P3dPaintableManager.ModifyRadius(pressure, ref finalRadius);
			P3dPaintableManager.ModifyHardness(pressure, ref finalHardness);

			var finalSize = scale * finalRadius;

			Command.Instance.SetShape(finalSize, rotateTo == RotationType.World ? Quaternion.identity : rotation);

			Command.Instance.SetMaterial(BlendMode, finalHardness, finalColor, finalOpacity, tileTexture, finalTileMatrix, tileBlend);

			return finalSize;
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dPaintSphere))]
	public class P3dClickToPaintSphere_Editor : P3dEditor<P3dPaintSphere>
	{
		private bool expandLayers;
		private bool expandGroups;

		protected override void OnInspector()
		{
			BeginError(Any(t => t.Layers == 0 && t.TargetModel == null));
				DrawExpand(ref expandLayers, "layers", "Only the P3dModel/P3dPaintable GameObjects whose layers are within this mask will be eligible for painting.");
			EndError();
			if (expandLayers == true || Any(t => t.TargetModel != null))
			{
				BeginIndent();
					Draw("targetModel", "If this is set, then only the specified P3dModel/P3dPaintable will be painted, regardless of the layer setting.");
				EndIndent();
			}
			DrawExpand(ref expandGroups, "group", "Only the P3dPaintableTexture components with a matching group will be painted by this component.");
			if (expandGroups == true || Any(t => t.TargetTexture != null))
			{
				BeginIndent();
					Draw("targetTexture", "If this is set, then only the specified P3dPaintableTexture will be painted, regardless of the layer or group setting.");
				EndIndent();
			}

			Separator();

			Draw("blendMode", "This component will paint using this blending mode.\n\nNOTE: See P3dBlendMode documentation for more information.");
			Draw("color", "The color of the paint.");
			Draw("opacity", "The opacity of the brush.");

			Separator();

			Draw("scale", "If you want to override the scale of the sphere to paint an ellipse, then set the scale here.");
			Draw("rotateTo", "This allows you to control how the ellipse is rotated.");
			Draw("radius", "The radius of the paint brush.");
			Draw("hardness", "This allows you to control the sharpness of the near+far depth cut-off point.");

			Separator();

			Draw("tileTexture", "This allows you to apply a tiled detail texture to your decals. This tiling will be applied in world space using triplanar mapping.");
			Draw("tileTransform", "This allows you to adjust the tiling position + rotation + scale using a Transform.");
			Draw("tileBlend", "This allows you to control the triplanar mapping sharpness between each axis.");
		}
	}
}
#endif