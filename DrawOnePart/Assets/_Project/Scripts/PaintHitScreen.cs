﻿using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
    public class PaintHitScreen : P3dConnectablePoints
    {
        public System.EventHandler<Vector3> onFingerHitBegin;
        public System.EventHandler<Vector3> onFingerHitUpdate;

        // This stores extra information for each finger unique to this component
        class Link
        {
            public P3dInputManager.Finger Finger;
            public float Distance;
        }

        public enum OrientationType
        {
            WorldUp,
            CameraUp
        }

        public enum NormalType
        {
            HitNormal,
            RayDirection
        }

        /// <summary>Orient to a specific camera?
        /// None = MainCamera.</summary>
        public Camera Camera { set { _camera = value; } get { return _camera; } }
        [SerializeField] private Camera _camera;

        /// <summary>If you want the paint to continuously apply while moving the mouse, this allows you to set how many pixels are between each step (0 = no drag).</summary>
        public float Spacing { set { spacing = value; } get { return spacing; } }
        [SerializeField] private float spacing = 5.0f;

        /// <summary>The layers you want the raycast to hit.</summary>
        public LayerMask Layers { set { layers = value; } get { return layers; } }
        [SerializeField] private LayerMask layers = Physics.DefaultRaycastLayers;

        /// <summary>If you want painting to require a specific key on desktop platforms, then you can specify it here.</summary>
        public KeyCode Key { set { key = value; } get { return key; } }
        [SerializeField] private KeyCode key = KeyCode.Mouse0;

        /// <summary>How should the hit point be oriented?
        /// WorldUp = It will be rotated to the normal, where the up vector is world up.
        /// CameraUp = It will be rotated to the normal, where the up vector is world up.</summary>
        public OrientationType Orientation { set { orientation = value; } get { return orientation; } }
        [SerializeField] private OrientationType orientation = OrientationType.CameraUp;

        /// <summary>Which normal should the hit point rotation be based on?</summary>
        public NormalType Normal { set { normal = value; } get { return normal; } }
        [SerializeField] private NormalType normal;

        /// <summary>If you want the raycast hit point to be offset from the surface a bit, this allows you to set by how much in world space.</summary>
        public float Offset { set { offset = value; } get { return offset; } }
        [SerializeField] private float offset;

        /// <summary>If you want the hit point to be offset upwards when using touch input, this allows you to specify the physical distance the hit will be offset by on the screen. This is useful if you find paint hard to see because it's underneath your finger.</summary>
        public float TouchOffset { set { touchOffset = value; } get { return touchOffset; } }
        [SerializeField] private float touchOffset;

        /// <summary>Show a painting preview under the mouse?</summary>
        public bool ShowPreview { set { showPreview = value; } get { return showPreview; } }
        [SerializeField] private bool showPreview = true;

        /// <summary>This allows you to override the order this paint gets applied to the object during the current frame.</summary>
        public int Priority { set { priority = value; } get { return priority; } }
        [SerializeField] private int priority;

        /// <summary>Should painting triggered from this component be eligible for being undone?</summary>
        public bool StoreStates { set { storeStates = value; } get { return storeStates; } }
        [SerializeField] private bool storeStates = true;

        [System.NonSerialized]
        private List<Link> links = new List<Link>();

        [System.NonSerialized]
        private P3dInputManager inputManager = new P3dInputManager();

        public bool ClearStates { set { clearStates = value; } get { return clearStates; } }
        [SerializeField] private bool clearStates = true;

        protected void LateUpdate()
        {
            inputManager.Update(key);

            // Use mouse hover preview?
            if (showPreview == true)
            {
                if (Input.touchCount == 0 && Input.GetKey(key) == false && P3dInputManager.PointOverGui(Input.mousePosition) == false)
                {
                    PaintAt(Input.mousePosition, true, 1.0f, this);
                }
                else
                {
                    BreakHits(this);
                    //ClearAll();
                }
            }

            for (var i = inputManager.Fingers.Count - 1; i >= 0; i--)
            {
                var finger = inputManager.Fingers[i];
                var down = finger.Down;
                var up = finger.Up;

                Paint(finger, down, up);
            }
        }

        private void Paint(P3dInputManager.Finger finger, bool down, bool up)
        {
            var link = GetLink(finger);

            if (spacing > 0.0f)
            {
                var tail = finger.SmoothPositions[0];

                if (down == true)
                {
                    link.Distance = 0.0f;

                    if (storeStates == true)
                    {
                        P3dStateManager.StoreAllStates();
                    }

                    PaintAt(tail, false, finger.Pressure, link);
                    onFingerHitBegin?.Invoke(this, finger.PositionA);
                }

                for (var i = 1; i < finger.SmoothPositions.Count; i++)
                {
                    var head = finger.SmoothPositions[i];
                    var dist = Vector2.Distance(tail, head);
                    var steps = Mathf.FloorToInt((link.Distance + dist) / spacing);

                    for (var j = 0; j < steps; j++)
                    {
                        var remainder = spacing - link.Distance;

                        tail = Vector2.MoveTowards(tail, head, remainder);

                        PaintAt(tail, false, finger.Pressure, link);

                        dist -= remainder;

                        link.Distance = 0.0f;
                    }

                    link.Distance += dist;

                    tail = head;
                }
            }
            else
            {
                if (showPreview == true)
                {
                    if (up == true)
                    {
                        if (storeStates == true)
                        {
                            P3dStateManager.StoreAllStates();
                        }

                        PaintAt(finger.PositionA, false, finger.Pressure, link);
                    }
                    else
                    {
                        PaintAt(finger.PositionA, true, finger.Pressure, link);
                    }
                }
                else if (down == true)
                {
                    if (storeStates == true)
                    {
                        P3dStateManager.StoreAllStates();
                    }

                    PaintAt(finger.PositionA, false, finger.Pressure, link);
                }
            }

            if (up == true)
            {
                BreakHits(link);
                //ClearAll();
            }
        }

        private void PaintAt(Vector2 screenPosition, bool preview, float pressure, object owner)
        {
            var camera = P3dHelper.GetCamera(_camera);

            if (camera != null)
            {
                if (touchOffset != 0.0f && Input.touchCount > 0)
                {
                    screenPosition.y += touchOffset * P3dInputManager.ScaleFactor;
                }

                var ray = camera.ScreenPointToRay(screenPosition);
                var hit = default(RaycastHit);

                if (Physics.Raycast(ray, out hit, float.PositiveInfinity, layers) == true)
                {
                    var finalUp = orientation == OrientationType.CameraUp ? camera.transform.up : Vector3.up;
                    var finalPosition = hit.point + hit.normal * offset;
                    var finalNormal = normal == NormalType.HitNormal ? -hit.normal : ray.direction;
                    var finalRotation = Quaternion.LookRotation(finalNormal, finalUp);

                    hitCache.InvokeRaycast(gameObject, preview, priority, hit, pressure);

                    SubmitPoint(preview, priority, hit.collider, finalPosition, finalRotation, pressure, owner);
                    if (!preview)
                    {
                        onFingerHitUpdate?.Invoke(this, finalPosition);
                    }
                    return;
                }
            }

            BreakHits(owner);
        }

        private Link GetLink(P3dInputManager.Finger finger)
        {
            for (var i = links.Count - 1; i >= 0; i--)
            {
                var link = links[i];

                if (link.Finger == finger)
                {
                    return link;
                }
            }

            var newLink = new Link();

            newLink.Finger = finger;

            links.Add(newLink);

            return newLink;
        }

        public void ClearAll(object sender, System.EventArgs args)
        {
            var paintableTexture = P3dPaintableTexture.FirstInstance;

            for (var i = 0; i < P3dPaintableTexture.InstanceCount; i++)
            {
                paintableTexture.Clear();

                if (clearStates == true)
                {
                    paintableTexture.ClearStates();
                }

                paintableTexture = paintableTexture.NextInstance;
            }
        }
    }
}