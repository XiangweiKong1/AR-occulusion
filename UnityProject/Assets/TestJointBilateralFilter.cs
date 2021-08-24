using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestJointBilateralFilter : MonoBehaviour
{
    #region shaders
    private Shader m_shaderOverlayImages;
    public Shader shaderOverlayImages
    {
        get
        {
            if (m_shaderOverlayImages == null)
                m_shaderOverlayImages = Shader.Find("Hidden/Filter");


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

    public float colorSigma = 2;
    public float spatialSigma = 2;
    public int size = 2;

    public Texture2D realColorTex;
    public Texture2D realDepthTex;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        materialOverlayImages.SetTexture("_VirtualTex", source);
        materialOverlayImages.SetTexture("_HandsTex", realDepthTex);
        materialOverlayImages.SetVector("_FrameSize", new Vector2(this.GetComponent<Camera>().pixelWidth, this.GetComponent<Camera>().pixelHeight));
        materialOverlayImages.SetFloat("_ColorSigma", colorSigma);
        materialOverlayImages.SetFloat("_SpatialSigma", spatialSigma);
        materialOverlayImages.SetInt("_Size", size);
        Graphics.Blit(realColorTex, destination, materialOverlayImages);
    }
}
