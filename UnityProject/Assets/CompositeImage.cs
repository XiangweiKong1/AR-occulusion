using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CompositeImage : MonoBehaviour
{
    #region shaders
    private Shader m_shaderOverlayImages;
    public Shader shaderOverlayImages
    {
        get
        {
            if (m_shaderOverlayImages == null)
                m_shaderOverlayImages = Shader.Find("Hidden/OverlayImages");


            return m_shaderOverlayImages;
        }
    }

    private Material m_MaterialOverlayImages;
    public Material materialOverlayImages
    {
        get
        {
            if (m_MaterialOverlayImages == null)
            {
                if (shaderOverlayImages == null || shaderOverlayImages.isSupported == false)
                    return null;

                m_MaterialOverlayImages = new Material(shaderOverlayImages);
            }

            return m_MaterialOverlayImages;
        }
    }
    #endregion

    public Camera handCamera;
    public VertClient vertClient;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
        handCamera.depthTextureMode = DepthTextureMode.Depth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        materialOverlayImages.SetTexture("_VirtualTex", source);
        var handTex = RenderTexture.GetTemporary(
            handCamera.pixelWidth, handCamera.pixelHeight, 24, 
            RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        handCamera.targetTexture = handTex;
        handCamera.Render();
        materialOverlayImages.SetTexture("_HandsTex", handTex);

        Graphics.Blit(vertClient.receiveTexture, destination, materialOverlayImages);

        RenderTexture.ReleaseTemporary(handTex);
    }
}