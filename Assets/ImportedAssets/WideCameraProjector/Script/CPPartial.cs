using UnityEngine;
using System;
using ProjecterLib;
using UnityEngine.Rendering;

public partial class WideCameraProjector : MonoBehaviour
{

    [Serializable]
    public class Options
    {
        [Space]
        public Texture background = null;
        public bool keepRatio = true;
        public Rect rect = new Rect(0, 0, 1, 1);
       
        [Space]
        public AntiAliasValue antiAliasing = AntiAliasValue.Off;
        public FilterMode filtering = FilterMode.Bilinear;

    }

    Camera _cam;
    public Camera cam
    {
        get
        {
            if (!_cam) _cam = GetComponent<Camera>();
            return _cam;
        }
    }

    public void ValidateRenderTexture(ref RenderTexture _tex)
    {
        var _dim = TextureDimension.Cube;
        var _res = resolution;
        var _fil = options.filtering;
        var _aas = options.antiAliasing;

        if (_tex && _tex.IsCreated() && (_tex.dimension != _dim || _tex.width != _res || _tex.filterMode != _fil || _tex.antiAliasing != (int)_aas))
        {
            _tex.Release();
        }
        if (!_tex)
        {
            _tex = new RenderTexture(_res, _res, 8)
            {
                name = "WideCameraProjector Screen Grabber",
                hideFlags = HideFlags.DontSave,
                dimension = _dim
            };
        }
        if (!_tex.IsCreated())
        {
            _tex.width = _tex.height = _res;
            _tex.depth = 8;
            _tex.filterMode = _fil;
            _tex.antiAliasing = (int)_aas;
            _tex.Create();
        }
    }
    
    public bool VR { get { return cam.stereoTargetEye != StereoTargetEyeMask.None && UnityEngine.XR.XRSettings.enabled; } }

    public void ValidateMaterial(ref Material _mat, RenderTexture _tex, Camera.MonoOrStereoscopicEye eyetarget, bool properties)
    {
        if (!_mat)
        {
            _mat = new Material(CPUtility.blitShaderCubic)
            {
                name = "WideCameraProjector Blitter",
                hideFlags = HideFlags.DontSave,
                shader = CPUtility.blitShaderCubic
            };
        }
        if (properties || !_mat)
        {
            CPUtility.SetProjecterKeyword(_mat, projection);
            {
                CPUtility.SetProjecterDirection(_mat, Quaternion.Inverse(transform.rotation));
                CPUtility.SetTexture(_mat, _tex);
            }
            CPUtility.SetProjectionFOV(_mat, fieldOfView, cam.fieldOfView);
            CPUtility.SetProjectionRect(_mat, options.rect, options.keepRatio ? GetAspect() : 1);
            CPUtility.SetEyeBlindSide(_mat, eyetarget);
        }
    }
}