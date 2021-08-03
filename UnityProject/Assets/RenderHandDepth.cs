using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderHandDepth : MonoBehaviour
{

    #region shaders
    private Shader m_shaderRenderDepth;
    public Shader shaderRenderDepth
    {
        get
        {
            if (m_shaderRenderDepth == null)
                m_shaderRenderDepth = Shader.Find("Hidden/RenderDepth");


            return m_shaderRenderDepth;
        }
    }

    private Material m_MaterialRenderDepth;
    public Material materialRenderDepth
    {
        get
        {
            if (m_MaterialRenderDepth == null)
            {
                if (shaderRenderDepth == null || shaderRenderDepth.isSupported == false)
                    return null;

                m_MaterialRenderDepth = new Material(shaderRenderDepth);
            }

            return m_MaterialRenderDepth;
        }
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, materialRenderDepth);
    }
}
