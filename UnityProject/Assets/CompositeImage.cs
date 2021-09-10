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
        if(Input.GetKeyUp(KeyCode.S)) 
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
            File.WriteAllBytes(Application.dataPath + "/Screenshot/realColorImage"+ System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".png", bytes);
            string filename = Application.dataPath + "/Screenshot/handDepths" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".exr";
            //string filename = Application.dataPath + "/Screenshot/handDepths.exr";
            SaveRenderTextureRGBAEXR(handTex, filename);
            //Texture2D Image = new Texture2D(handTex.width, handTex.height, TextureFormat.RGBAFloat, false);
            //Graphics.CopyTexture(handTex, Image);
            //bytes = Image.EncodeToEXR();
            //File.WriteAllBytes(Application.dataPath + "/Screenshot/handDepths.exr", bytes);
            saveImages = false;
            //Destroy(Image);
        }

        RenderTexture.ReleaseTemporary(handTex);
    }

    void SaveRenderTextureRGBAEXR(RenderTexture texture, string filename)
    {
        RenderTexture.active = texture;
        Texture2D tex = new Texture2D(texture.width, texture.height, TextureFormat.RGBAFloat, false);
        tex.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        RenderTexture.active = null;
        byte[] bytes;
        bytes = tex.EncodeToEXR();
        //if (!File.Exists(filename))
        //{
        //    File.Create(filename).Dispose();
        //}
        System.IO.File.WriteAllBytes(filename, bytes);
        Destroy(tex);
    }
}
