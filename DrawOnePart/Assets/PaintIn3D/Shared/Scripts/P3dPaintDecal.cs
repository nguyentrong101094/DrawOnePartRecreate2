using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This allows you to paint a decal at a hit point. A hit point can be found using a companion component like: P3dDragRaycast, P3dOnCollision, P3dOnParticleCollision.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dPaintDecal")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Paint/Paint Decal")]
	public class P3dPaintDecal : MonoBehaviour, IHit, IHitPoint, IHitLine, IHitQuad
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
			public Vector3   Direction;
			public Color     Color;
			public float     Opacity;
			public float     Hardness;
			public float     Wrapping;
			public Texture   Texture;
			public Texture   Shape;
			public Vector4   ShapeChannel;
			public Vector2   NormalFront;
			public Vector2   NormalBack;
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
				cachedSpotMaterials = P3dPaintableManager.BuildMaterialsBlendModes("Hidden/Paint in 3D/Decal");
				cachedLineMaterials = P3dPaintableManager.BuildMaterialsBlendModes("Hidden/Paint in 3D/Decal", "P3D_LINE");
				cachedQuadMaterials = P3dPaintableManager.BuildMaterialsBlendModes("Hidden/Paint in 3D/Decal", "P3D_QUAD");
			}

			public override void Apply()
			{
				Blend.Apply(Material);

				Material.SetVector(P3dShader._Position, Position);
				Material.SetVector(P3dShader._EndPosition, EndPosition);
				Material.SetVector(P3dShader._Position2, Position2);
				Material.SetVector(P3dShader._EndPosition2, EndPosition2);
				Material.SetMatrix(P3dShader._Matrix, Matrix.inverse);
				Material.SetVector(P3dShader._Direction, Direction);
				Material.SetColor(P3dShader._Color, Color);
				Material.SetFloat(P3dShader._Opacity, Opacity);
				Material.SetFloat(P3dShader._Hardness, Hardness);
				Material.SetFloat(P3dShader._Wrapping, Wrapping);
				Material.SetTexture(P3dShader._Texture, Texture);
				Material.SetTexture(P3dShader._Shape, Shape);
				Material.SetVector(P3dShader._ShapeChannel, ShapeChannel);
				Material.SetVector(P3dShader._NormalFront, NormalFront);
				Material.SetVector(P3dShader._NormalBack, NormalBack);
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
				Direction    = Matrix.MultiplyVector(Vector3.forward).normalized;
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
				command.Direction    = Direction;
				command.Color        = Color;
				command.Opacity      = Opacity;
				command.Hardness     = Hardness;
				command.Wrapping     = Wrapping;
				command.Texture      = Texture;
				command.Shape        = Shape;
				command.ShapeChannel = ShapeChannel;
				command.NormalFront  = NormalFront;
				command.NormalBack   = NormalBack;
				command.TileTexture  = TileTexture;
				command.TileMatrix   = TileMatrix;
				command.TileBlend    = TileBlend;

				return command;
			}

			public Vector3 SetShape(Quaternion worldRotation, Vector3 scale, float radius, float aspect)
			{
				var worldSize = scale * radius;

				if (aspect > 1.0f)
				{
					worldSize.y /= aspect;
				}
				else
				{
					worldSize.x *= aspect;
				}

				Matrix    = Matrix4x4.TRS(Vector3.zero, worldRotation, worldSize);
				Direction = worldRotation * Vector3.forward;

				return worldSize;
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

			public void SetMaterial(P3dBlendMode blendMode, Texture texture, Texture shape, P3dChannel shapeChannel, float hardness, float wrapping, float normalBack, float normalFront, float normalFade, Color color, float opacity, Texture tileTexture, Matrix4x4 tileMatrix, float tileBlend)
			{
				switch (Extrusions)
				{
					case 0: Material = cachedSpotMaterials[blendMode]; break;
					case 1: Material = cachedLineMaterials[blendMode]; break;
					case 2: Material = cachedQuadMaterials[blendMode]; break;
				}

				Blend        = blendMode;
				Color        = color;
				Opacity      = opacity;
				Hardness     = hardness;
				Wrapping     = wrapping;
				Texture      = texture;
				Shape        = shape;
				ShapeChannel = P3dHelper.IndexToVector((int)shapeChannel);
				TileTexture  = tileTexture;
				TileMatrix   = tileMatrix;
				TileBlend    = tileBlend;

				var pointA = normalFront - 1.0f - normalFade;
				var pointB = normalFront - 1.0f;
				var pointC = 1.0f - normalBack + normalFade;
				var pointD = 1.0f - normalBack;

				NormalFront = new Vector2(pointA, P3dHelper.Reciprocal(pointB - pointA));
				NormalBack  = new Vector2(pointC, P3dHelper.Reciprocal(pointD - pointC));
			}
		}

		/// <summary>Only the P3dModel/P3dPaintable GameObjects whose layers are within this mask will be eligible for painting.</summary>
		public LayerMask Layers { set { layers = value; } get { return layers; } } [SerializeField] private LayerMask layers = -1;

		/// <summary>If this is set, then only the specified P3dModel/P3dPaintable will be painted, regardless of the layer setting.</summary>
		public P3dModel TargetModel { set { targetModel = value; } get { return targetModel; } } [SerializeField] private P3dModel targetModel;

		/// <summary>Only the <b>P3dPaintableTexture</b> components with a matching group will be painted by this component.</summary>
		public P3dGroup Group { set { group = value; } get { return group; } } [SerializeField] private P3dGroup group;

		/// <summary>If this is set, then only the specified P3dPaintableTexture will be painted, regardless of the layer or group setting.</summary>
		public P3dPaintableTexture TargetTexture { set { targetTexture = value; } get { return targetTexture; } } [SerializeField] private P3dPaintableTexture targetTexture;

		/// <summary>This component will paint using this blending mode.
		/// NOTE: See <b>P3dBlendMode</b> documentation for more information.</summary>
		public P3dBlendMode BlendMode { set { blendMode = value; } get { return blendMode; } } [SerializeField] private P3dBlendMode blendMode = P3dBlendMode.AlphaBlend;

		/// <summary>The decal that will be painted.</summary>
		public Texture Texture { set { texture = value; } get { return texture; } } [SerializeField] private Texture texture;

		/// <summary>This allows you to specify the shape of the decal. This is optional for most blending modes, because they usually derive their shape from the RGB or A values. However, if you're using the <b>Replace</b> blending mode, then you must manually specify the shape.</summary>
		public Texture Shape { set { shape = value; } get { return shape; } } [SerializeField] private Texture shape;

		/// <summary>This allows you specify the texture channel used when sampling <b>Shape</b>.</summary>
		public P3dChannel ShapeChannel { set { shapeChannel = value; } get { return shapeChannel; } } [SerializeField] private P3dChannel shapeChannel = P3dChannel.Alpha;

		/// <summary>The color of the paint.</summary>
		public Color Color { set { color = value; } get { return color; } } [SerializeField] private Color color = Color.white;

		/// <summary>The opacity of the brush.</summary>
		public float Opacity { set { opacity = value; } get { return opacity; } } [Range(0.0f, 1.0f)] [SerializeField] private float opacity = 1.0f;

		/// <summary>The angle of the decal in degrees.</summary>
		public float Angle { set { angle = value; } get { return angle; } } [Range(-180.0f, 180.0f)] [SerializeField] private float angle;

		/// <summary>This allows you to control the mirroring and aspect ratio of the decal.
		/// 1, 1 = No scaling.
		/// -1, 1 = Horizontal Flip.</summary>
		public Vector3 Scale { set { scale = value; } get { return scale; } } [SerializeField] private Vector3 scale = Vector3.one;

		/// <summary>The radius of the paint brush.</summary>
		public float Radius { set { radius = value; } get { return radius; } } [SerializeField] private float radius = 0.1f;

		/// <summary>This allows you to control the sharpness of the near+far depth cut-off point.</summary>
		public float Hardness { set { hardness = value; } get { return hardness; } } [SerializeField] private float hardness = 3.0f;

		/// <summary>This allows you to control how much the decal can wrap around uneven paint surfaces.</summary>
		public float Wrapping { set { wrapping = value; } get { return wrapping; } } [SerializeField] [Range(0.0f, 1.0f)] private float wrapping = 1.0f;

		/// <summary>This allows you to control how much the paint can wrap around the front of surfaces.
		/// For example, if you want paint to wrap around curved surfaces then set this to a higher value.
		/// NOTE: If you set this to 0 then paint will not be applied to front facing surfaces.</summary>
		public float NormalFront { set { normalFront = value; } get { return normalFront; } } [Range(0.0f, 2.0f)] [SerializeField] private float normalFront = 0.2f;

		/// <summary>This works just like <b>Normal Front</b>, except for back facing surfaces.
		/// NOTE: If you set this to 0 then paint will not be applied to back facing surfaces.</summary>
		public float NormalBack { set { normalBack = value; } get { return normalBack; } } [Range(0.0f, 2.0f)] [SerializeField] private float normalBack;

		/// <summary>This allows you to control the smoothness of the normal cut-off point.</summary>
		public float NormalFade { set { normalFade = value; } get { return normalFade; } } [Range(0.001f, 0.5f)] [SerializeField] private float normalFade = 0.01f;

		/// <summary>This allows you to apply a tiled detail texture to your decals. This tiling will be applied in world space using triplanar mapping.</summary>
		public Texture TileTexture { set { tileTexture = value; } get { return tileTexture; } } [SerializeField] private Texture tileTexture;

		/// <summary>This allows you to adjust the tiling position + rotation + scale using a <b>Transform</b>.</summary>
		public Transform TileTransform { set { tileTransform = value; } get { return tileTransform; } } [SerializeField] private Transform tileTransform;

		/// <summary>This allows you to control the triplanar mapping sharpness between each axis.</summary>
		public float TileBlend { set { tileBlend = value; } get { return tileBlend; } } [Range(1.0f, 10.0f)] [SerializeField] private float tileBlend = 3.0f;

		/// <summary>This method will invert the scale.x value.</summary>
		[ContextMenu("Flip Horizontal")]
		public void FlipHorizontal()
		{
			scale.x = -scale.x;
		}

		/// <summary>This method will invert the scale.y value.</summary>
		[ContextMenu("Flip Vertical")]
		public void FlipVertical()
		{
			scale.y = -scale.y;
		}

		/// <summary>This method multiplies the radius by the specified value.</summary>
		public void IncrementOpacity(float delta)
		{
			opacity = Mathf.Clamp01(opacity + delta);
		}

		/// <summary>This method increments the angle by the specified amount of degrees, and wraps it to the -180..180 range.</summary>
		public void IncrementAngle(float degrees)
		{
			angle = Mathf.Repeat(angle + 180.0f + degrees, 360.0f) - 180.0f;
		}

		/// <summary>This method multiplies the scale by the specified value.</summary>
		public void MultiplyScale(float multiplier)
		{
			scale *= multiplier;
		}

		/// <summary>This method multiplies the hardness by the specified value.</summary>
		public void MultiplyHardness(float multiplier)
		{
			hardness *= multiplier;
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
			var finalOpacity  = opacity;
			var finalRadius   = radius;
			var finalHardness = hardness;
			var finalColor    = color;
			var finalAngle    = angle;
			var finalTexture  = texture;
			var finalMatrix   = tileTransform != null ? tileTransform.localToWorldMatrix : Matrix4x4.identity;
			var finalAspect   = P3dHelper.GetAspect(shape, texture);

			P3dPaintableManager.BuildModifiers(gameObject);
			P3dPaintableManager.ModifyColor(pressure, ref finalColor);
			P3dPaintableManager.ModifyAngle(pressure, ref finalAngle);
			P3dPaintableManager.ModifyOpacity(pressure, ref finalOpacity);
			P3dPaintableManager.ModifyHardness(pressure, ref finalHardness);
			P3dPaintableManager.ModifyRadius(pressure, ref finalRadius);
			P3dPaintableManager.ModifyTexture(pressure, ref finalTexture);

			rotation = rotation * Quaternion.Euler(0.0f, 0.0f, finalAngle);

			var finalSize = Command.Instance.SetShape(rotation, scale, finalRadius, finalAspect);

			Command.Instance.SetMaterial(blendMode, texture, shape, shapeChannel, finalHardness, wrapping, normalBack, normalFront, normalFade, finalColor, finalOpacity, tileTexture, finalMatrix, tileBlend);

			return finalSize;
		}
#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

			Gizmos.DrawWireCube(Vector3.zero, scale * radius * 2.0f);
		}
#endif
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dPaintDecal))]
	public class P3dPaintDecal_Editor : P3dEditor<P3dPaintDecal>
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
			BeginError(Any(t => t.Texture == null));
				Draw("texture", "The decal that will be painted.");
			EndError();
			EditorGUILayout.BeginHorizontal();
				BeginError(Any(t => t.BlendMode == P3dBlendMode.Replace && t.Shape == null));
					Draw("shape", "This allows you to specify the shape of the decal. This is optional for most blending modes, because they usually derive their shape from the RGB or A values. However, if you're using the Replace blending mode, then you must manually specify the shape.");
				EndError();
				EditorGUILayout.PropertyField(serializedObject.FindProperty("shapeChannel"), GUIContent.none, GUILayout.Width(50));
			EditorGUILayout.EndHorizontal();
			Draw("color", "The color of the paint.");
			Draw("opacity", "The opacity of the brush.");

			Separator();

			Draw("angle", "The angle of the decal in degrees.");
			Draw("scale", "This allows you to control the mirroring and aspect ratio of the decal.\n\n1, 1 = No scaling.\n-1, 1 = Horizontal Flip.");
			BeginError(Any(t => t.Radius <= 0.0f));
				Draw("radius", "The radius of the paint brush.");
			EndError();
			BeginError(Any(t => t.Hardness <= 0.0f));
				Draw("hardness", "This allows you to control the sharpness of the near+far depth cut-off point.");
			EndError();
			Draw("wrapping", "This allows you to control how much the decal can wrap around uneven paint surfaces.");

			Separator();

			Draw("normalFront", "This allows you to control how much the paint can wrap around the front of surfaces (e.g. if you want paint to wrap around curved surfaces then set this to a higher value).\n\nNOTE: If you set this to 0 then paint will not be applied to front facing surfaces.");
			Draw("normalBack", "This works just like Normal Front, except for back facing surfaces.\n\nNOTE: If you set this to 0 then paint will not be applied to back facing surfaces.");
			Draw("normalFade", "This allows you to control the smoothness of the depth cut-off point.");

			Separator();

			Draw("tileTexture", "This allows you to apply a tiled detail texture to your decals. This tiling will be applied in world space using triplanar mapping.");
			Draw("tileTransform", "This allows you to adjust the tiling position + rotation + scale using a Transform.");
			Draw("tileBlend", "This allows you to control the triplanar mapping sharpness between each axis.");
		}
	}
}
#endif