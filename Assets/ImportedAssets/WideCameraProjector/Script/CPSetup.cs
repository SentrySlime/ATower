using ProjecterLib;
using System.Collections.Generic;
using UnityEngine;

public class CPSetup
{
    public static CPGrabber Setup(string name, Transform parent, Camera.MonoOrStereoscopicEye eye)
    {
        var g = new GameObject(name, typeof(Camera), typeof(CPGrabber)).transform;

        g.SetParent(parent, false);

        g.GetComponent<Camera>().enabled = false;
        g.GetComponent<CPGrabber>().stereoTarget = eye;

#if UNITY_EDITOR
        if (!Application.isPlaying)
            UnityEditor.Undo.RegisterCreatedObjectUndo(g.gameObject, "WideCameraProjector Setup");
#endif
        return g.GetComponent<CPGrabber>();
    }


    public static void Clear(List<CPGrabber> grabbers)
    {
        Transform p = null;
        for (int i = 0; i < grabbers.Count; i++)
        {
            if (grabbers[i])
            {
                p = p ?? grabbers[i].transform.parent;
                CPUtility.Destroy(grabbers[i].gameObject, true);
            }
        }
        if (p && p.name == "Projecter Cameras" && p.childCount == 0)
            CPUtility.Destroy(p.gameObject, true);
        grabbers.Clear();
    }
}
