using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTextureSetup : MonoBehaviour
{
    public Camera cameraA;
    public Camera cameraB;


    public Material cameraMatA;
    public Material cameraMatB;

    

    void Start()
    {
        int rtWidth = Screen.width / 2;
        int rtHeight = Screen.height / 2;

        if (cameraA.targetTexture)
            cameraA.targetTexture.Release();
        cameraA.targetTexture = new RenderTexture(rtWidth, rtHeight, 24);
        cameraMatA.mainTexture = cameraA.targetTexture;

        if (cameraB.targetTexture)
            cameraB.targetTexture.Release();
        cameraB.targetTexture = new RenderTexture(rtWidth, rtHeight, 24);
        cameraMatB.mainTexture = cameraB.targetTexture;
    }

    
}
