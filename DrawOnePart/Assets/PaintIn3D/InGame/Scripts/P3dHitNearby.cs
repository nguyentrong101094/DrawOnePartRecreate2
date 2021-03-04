using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component continuously fires hit events using the current Transform position.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dHitNearby")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Hit/Hit Nearby")]
	public class P3dHitNearby : P3dConnectablePoints
	{
		public enum PaintType
		{
			Update,
			FixedUpdate
		}

		/// <summary>Where in the game loop should this component paint?</summary>
		public PaintType PaintIn { set { paintIn = value; } get { return paintIn; } } [SerializeField] private PaintType paintIn;

		/// <summary>The time in seconds between each raycast.
		/// 0 = Every frame.</summary>
		public float Delay { set { delay = value; } get { return delay; } } [SerializeField] private float delay = 0.05f;

		/// <summary>Should the applied paint be applied as a preview?</summary>
		public bool Preview { set { preview = value; } get { return preview; } } [SerializeField] private bool preview;

		/// <summary>This allows you to override the order this paint gets applied to the object during the current frame.</summary>
		public int Priority { set { priority = value; } get { return priority; } } [SerializeField] private int priority;

		/// <summary>This allows you to control the pressure of the painting. This could be controlled by a VR trigger or similar for more advanced effects.</summary>
		public float Pressure { set { pressure = value; } get { return pressure; } } [Range(0.0f, 1.0f)] [SerializeField] private float pressure = 1.0f;

		[System.NonSerialized]
		private float current;

		[SerializeField]
		private Vector3 lastPosition;

		protected virtual void OnEnable()
		{
			ResetPosition();
		}

		protected virtual void Start()
		{
			ResetPosition();
		}

		protected override void Update()
		{
			base.Update();

			if (preview == true)
			{
				SubmitPoint(true, priority, null, transform.position, transform.rotation, pressure, this);
			}
			else
			{
				if (paintIn == PaintType.Update)
				{
					UpdatePaint();
				}
			}
		}

		protected virtual void FixedUpdate()
		{
			if (preview == false)
			{
				if (paintIn == PaintType.FixedUpdate)
				{
					UpdatePaint();
				}
			}
		}

		private void UpdatePaint()
		{
			current += Time.deltaTime;

			if (delay > 0.0f)
			{
				if (current >= delay)
				{
					current %= delay;

					SubmitPoint(false, priority, null, transform.position, transform.rotation, pressure, this);
				}
			}
			else
			{
				SubmitPoint(false, priority, null, transform.position, transform.rotation, pressure, this);
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dHitNearby))]
	public class P3dHitNearby_Editor : P3dConnectablePoints_Editor<P3dHitNearby>
	{
		protected override void OnInspector()
		{
			Draw("delay", "The time in seconds between each raycast.\n\n0 = Every frame.");
			Draw("paintIn", "Where in the game loop should this component paint?");

			base.OnInspector();

			Separator();

			Separator();

			Draw("preview", "Should the applied paint be applied as a preview?");
			Draw("priority", "This allows you to override the order this paint gets applied to the object during the current frame.");
			Draw("pressure", "This allows you to control the pressure of the painting. This could be controlled by a VR trigger or similar for more advanced effects.");

			Target.HitCache.Inspector(Target.gameObject, false, true, Target.ConnectHits, false);
		}
	}
}
#endif