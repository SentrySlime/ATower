using UnityEngine;
using System;
using ProjecterLib;
using System.Linq;

[ExecuteInEditMode, RequireComponent(typeof(Camera)), AddComponentMenu("Effects/Projecter/Wide Camera Projector", 1)]
public partial class WideCameraProjector : MonoBehaviour
{

    [Range(0, 360)]
    public float fieldOfView = 180f;
    public ProjecterMode projection;
    public int resolution = 512;

    public Options options = new Options();

    // temporary variables
    CameraClearFlags m_clearFlags;
    int m_cullingMask;
    DepthTextureMode m_depthMode;

    CPGrabber m_monoEye;
    CPGrabber m_leftEye;
    CPGrabber m_rightEye;

    [ContextMenu("Setup Components")]
    private void Reset()
    {
        foreach (var grabber in GetComponentsInChildren<CPGrabber>())
        {
            switch (grabber.stereoTarget)
            {
                case Camera.MonoOrStereoscopicEye.Left:
                    m_leftEye = grabber;
                    break;
                case Camera.MonoOrStereoscopicEye.Right:
                    m_rightEye = grabber;
                    break;
                case Camera.MonoOrStereoscopicEye.Mono:
                    m_monoEye = grabber;
                    break;
            }
        }
        if (VR)
        {
            if (!m_leftEye)
                m_leftEye = CPSetup.Setup("Left", transform, Camera.MonoOrStereoscopicEye.Left);
            if (!m_rightEye)
                m_rightEye = CPSetup.Setup("Right", transform, Camera.MonoOrStereoscopicEye.Right);
        } else
        {
            if (!m_monoEye)
                m_monoEye = CPSetup.Setup("Mono", transform, Camera.MonoOrStereoscopicEye.Mono);
        }
    }

    void Start()
    {
    }

    void OnPreCull()
    {
        GrabTextures();
        m_cullingMask = cam.cullingMask;
        m_clearFlags = cam.clearFlags;
        m_depthMode = cam.depthTextureMode;
        cam.cullingMask = 0;
        cam.clearFlags = CameraClearFlags.Nothing;
        cam.depthTextureMode = DepthTextureMode.None;
    }

    void OnPostRender()
    {
        cam.cullingMask = m_cullingMask;
        cam.clearFlags = m_clearFlags;
        cam.depthTextureMode = m_depthMode;
    }

    int mask;
#if UNITY_EDITOR
    protected string masks;
#endif

    void GrabTextures()
    {
        try
        {
            mask = CPMasks.GetPossibleMasks(transform.rotation, projection, fieldOfView * Mathf.Deg2Rad, GetAspect());
#if UNITY_EDITOR
            masks = Convert.ToString(mask, 2).PadLeft(6, '0');
#endif
            if (VR)
            {
                m_leftEye.Render(mask);
                m_rightEye.Render(mask);
            }
            else
            {
                m_monoEye.Render(mask);
            }
        }
        catch (Exception)
        {
            Reset();
        }
    }

    // Classic render target pipeline for RT-based effects
    // Note that any effect that happens after this stack will work in LDR
    [ImageEffectTransformsToLDR]
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (options.background)
            Graphics.Blit(options.background, destination);
        if (VR)
        {
            m_leftEye.Blit(destination);
            m_rightEye.Blit(destination);
        }
        else
        {
            m_monoEye.Blit(destination);
        }
    }

    public float GetAspect()
    {
        var tex = cam.targetTexture;
        if (tex)
            return (float)tex.width / tex.height;
        else
            return cam.aspect;
    }
}

