using UnityEngine;
using System.Collections;

public class Sea : MonoBehaviour {

    [SerializeField]
    public Shader _shader;
    public Material m_Material;
    void Start(){
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
        m_Material=new Material(_shader);   
    }
    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        Graphics.Blit(src, dest, m_Material);
    }
}