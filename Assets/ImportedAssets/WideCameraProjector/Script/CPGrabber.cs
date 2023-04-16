using System;
using System.Linq;
using UnityEngine;

namespace ProjecterLib
{

    [ExecuteInEditMode, RequireComponent(typeof(Camera)), AddComponentMenu("Effects/Projecter/Wide Camera Projector Grabber", 2)]
    public class CPGrabber : MonoBehaviour
    {

        public Camera.MonoOrStereoscopicEye stereoTarget;

        [NonSerialized]
        Camera _cam;
        [NonSerialized]
        WideCameraProjector _controller;
        RenderTexture _tex;
        Material _mat;

        public Camera cam
        {
            get { return _cam ?? (_cam = GetComponent<Camera>()); }
        }

        public WideCameraProjector controller
        {
            get { return _controller ?? (_controller = GetComponentInParent<WideCameraProjector>()); }
        }
        
        void OnPreCull()
        {
            // Prepare parameters and Assign the render texture to camera
            HookAndCopy();
        }

        public void Render(int mask = 63)
        {
            if (isActiveAndEnabled)
            {
                controller.ValidateRenderTexture(ref _tex);
#if UNITY_2017 || UNITY_5
                cam.RenderToCubemap(_tex, mask);
#else
                cam.RenderToCubemap(_tex, mask, stereoTarget);
#endif
            }
        }

        public void Blit(RenderTexture dest)
        {
            if (isActiveAndEnabled)
            {
                // blit expects Render() has been called before.
                controller.ValidateMaterial(ref _mat, _tex, stereoTarget, true);
                Graphics.Blit(_tex, dest, _mat, -1);
            }
        }

        void Cleanup()
        {
            CPUtility.DestroyRenderTexture(_tex);
            CPUtility.Destroy(_mat);
        }

        void OnDisable()
        {
            Cleanup();
        }

        void OnDestroy()
        {
            CPUtility.Destroy(_tex);
        }

        public void HookAndCopy()
        {
            if (!controller)
                return;
            
            var c = controller.cam;
            cam.cullingMask = c.cullingMask;
            cam.nearClipPlane = c.nearClipPlane;
            cam.farClipPlane = c.farClipPlane;
            cam.depth = c.depth;
            cam.clearFlags = c.clearFlags;
            cam.depth = c.depth - 1;
            cam.useOcclusionCulling = c.useOcclusionCulling;
            cam.stereoSeparation = c.stereoSeparation;
            cam.stereoConvergence = c.stereoConvergence;
        }
    }
}