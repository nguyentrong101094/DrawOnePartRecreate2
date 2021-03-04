using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component implements the replace channels paint mode, which will replace all pixels in the specified textures and channel weights.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dPaintReplaceChannels")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Paint/Paint Replace Channels")]
	public class P3dPaintReplaceChannels : MonoBehaviour, IHit, IHitPoint
	{
		public class Command : P3dCommand
		{
			public static Command Instance = new Command();
			
			public Texture TextureR;
			public Texture TextureG;
			public Texture TextureB;
			public Texture TextureA;
			public Vector4 ChannelR;
			public Vector4 ChannelG;
			public Vector4 ChannelB;
			public Vector4 ChannelA;

			private static Stack<Command> pool = new Stack<Command>();

			private static Material cachedMaterial;

			public override bool RequireMesh { get { return false; } }

			static Command()
			{
				cachedMaterial = P3dPaintableManager.BuildMaterial("Hidden/Paint in 3D/Replace Channels");
			}

			public override void Apply()
			{
				Material.SetTexture(P3dShader._TextureR, TextureR);
				Material.SetTexture(P3dShader._TextureG, TextureG);
				Material.SetTexture(P3dShader._TextureB, TextureB);
				Material.SetTexture(P3dShader._TextureA, TextureA);
				Material.SetVector(P3dShader._ChannelR, ChannelR);
				Material.SetVector(P3dShader._ChannelG, ChannelG);
				Material.SetVector(P3dShader._ChannelB, ChannelB);
				Material.SetVector(P3dShader._ChannelA, ChannelA);
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

				command.TextureR = TextureR;
				command.TextureG = TextureG;
				command.TextureB = TextureB;
				command.TextureA = TextureA;
				command.ChannelR = ChannelR;
				command.ChannelG = ChannelG;
				command.ChannelB = ChannelB;
				command.ChannelA = ChannelA;

				return command;
			}

			public void SetMaterial(Texture textureR, Texture textureG, Texture textureB, Texture textureA, Vector4 channelR, Vector4 channelG, Vector4 channelB, Vector4 channelA)
			{
				Blend    = P3dBlendMode.Replace;
				Material = cachedMaterial;
				TextureR = textureR;
				TextureG = textureG;
				TextureB = textureB;
				TextureA = textureA;
				ChannelR = channelR;
				ChannelG = channelG;
				ChannelB = channelB;
				ChannelA = channelA;
			}
		}

		/// <summary>Only the <b>P3dPaintableTexture</b> components with a matching group will be painted by this component.</summary>
		public P3dGroup Group { set { group = value; } get { return group; } } [SerializeField] private P3dGroup group;

		public Texture TextureR { set { textureR = value; } get { return textureR; } } [SerializeField] private Texture textureR;
		public Texture TextureG { set { textureG = value; } get { return textureG; } } [SerializeField] private Texture textureG;
		public Texture TextureB { set { textureB = value; } get { return textureB; } } [SerializeField] private Texture textureB;
		public Texture TextureA { set { textureA = value; } get { return textureA; } } [SerializeField] private Texture textureA;

		public Vector4 ChannelR { set { channelR = value; } get { return channelR; } } [SerializeField] private Vector4 channelR = new Vector4(1, 0, 0, 0);
		public Vector4 ChannelG { set { channelR = value; } get { return channelG; } } [SerializeField] private Vector4 channelG = new Vector4(1, 0, 0, 0);
		public Vector4 ChannelB { set { channelR = value; } get { return channelB; } } [SerializeField] private Vector4 channelB = new Vector4(1, 0, 0, 0);
		public Vector4 ChannelA { set { channelR = value; } get { return channelA; } } [SerializeField] private Vector4 channelA = new Vector4(1, 0, 0, 0);

		public static void Blit(RenderTexture renderTexture, Texture textureR, Texture textureG, Texture textureB, Texture textureA, Vector4 channelR, Vector4 channelG, Vector4 channelB, Vector4 channelA)
		{
			Command.Instance.SetMaterial(textureR, textureG, textureB, textureA, channelR, channelG, channelB, channelA);

			Command.Instance.Apply();

			P3dHelper.Blit(renderTexture, Command.Instance.Material);
		}

		public static void BlitFast(RenderTexture renderTexture, Texture textureR, Texture textureG, Texture textureB, Texture textureA, Vector4 channelR, Vector4 channelG, Vector4 channelB, Vector4 channelA)
		{
			Command.Instance.SetMaterial(textureR, textureG, textureB, textureA, channelR, channelG, channelB, channelA);

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
						Command.Instance.SetState(preview, priority);
						Command.Instance.SetMaterial(textureR, textureG, textureB, textureA, channelR, channelG, channelB, channelA);

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
	[CustomEditor(typeof(P3dPaintReplaceChannels))]
	public class P3dPaintReplaceChannels_Editor : P3dEditor<P3dPaintReplaceChannels>
	{
		protected override void OnInspector()
		{
			Draw("group", "Only the P3dPaintableTexture components with a matching group will be painted by this component.");

			Separator();

			Draw("textureR", "");
			Draw("textureG", "");
			Draw("textureB", "");
			Draw("textureA", "");
			Draw("channelR", "");
			Draw("channelG", "");
			Draw("channelB", "");
			Draw("channelA", "");
		}
	}
}
#endif