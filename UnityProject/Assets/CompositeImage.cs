using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class CompositeImage : MonoBehaviour
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

    public Camera handCamera;
    public VertClient vertClient;
    public float colorSigma = 2;
    public float spatialSigma = 2;
    public int size = 2;
    private bool saveImages = false;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
        handCamera.depthTextureMode = DepthTextureMode.Depth;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S)) 
        {
            saveImages = true;
        }
        
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
        materialOverlayImages.SetVector("_FrameSize", new Vector2(handCamera.pixelWidth, handCamera.pixelHeight));
        materialOverlayImages.SetFloat("_ColorSigma", colorSigma);
        materialOverlayImages.SetFloat("_SpatialSigma", spatialSigma);
        materialOverlayImages.SetInt("_Size", size);
        Graphics.Blit(vertClient.receiveTexture, destination, materialOverlayImages);

        if(saveImages)
        {
            byte[] bytes = vertClient.receiveTexture.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/realColorImage.png", bytes);
            Texture2D Image = new Texture2D(handTex.width, handTex.height, TextureFormat.RGBAFloat, false);
            Graphics.CopyTexture(handTex, Image);
            bytes = Image.EncodeToEXR();
            File.WriteAllBytes(Application.dataPath + "/handDepths.exr", bytes);
            saveImages = false;
            Destroy(Image);
        }

        RenderTexture.ReleaseTemporary(handTex);
    }
}
