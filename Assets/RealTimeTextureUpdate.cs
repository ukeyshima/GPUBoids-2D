using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealTimeTextureUpdate : MonoBehaviour
{
    public Material HeightMapMaterial;    
    public MouseCoordinates mouseCoordinates;
    public RenderTexture HeightMapTexture; 
    public Material NormalMapMaterial;    
    void Start(){
        
    }

    void Update()
    {
        Vector2 MousePosition=mouseCoordinates.GetConventionMousePosition();
        HeightMapMaterial.SetVector("_MousePosition",MousePosition);                                
        HeightMapMaterial.SetFloat("_ShouldRippleRendering",0);        

        if(Input.GetMouseButtonDown(0)){
            HeightMapMaterial.SetFloat("_ShouldRippleRendering",1);
        }        
                
        NormalMapMaterial.SetTexture("_HeightMap",HeightMapTexture);
    }
}
