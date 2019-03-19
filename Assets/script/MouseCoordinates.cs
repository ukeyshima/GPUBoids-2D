using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCoordinates : MonoBehaviour
{
    public Material mat;
    public GPUBoids gpuBoids;
    private Vector3 MousePosition;   
    private Vector3 WallSize;
    public Vector2 GetMousePosition(){
        return new Vector2(this.MousePosition.x,this.MousePosition.z);
    }
    public Vector2 GetConventionMousePosition(){
        return new Vector2(MousePosition.x/WallSize.x*4.0f+0.5f,MousePosition.z/WallSize.z*4.0f+0.5f);
    }
    void Start()
    {
        
    }
    void Update()
    {   
        WallSize=gpuBoids.WallSize;        
        MousePosition = Input.mousePosition;
        MousePosition = Camera.main.ScreenToWorldPoint(MousePosition);
        float sensitivity = 0.1f; 
        float mouse_move_x = Input.GetAxis("Mouse X") * sensitivity;
        float mouse_move_y = Input.GetAxis("Mouse Y") * sensitivity;
		mat.SetVector("_MousePosition", GetConventionMousePosition());
        mat.SetVector("_RawMousePosition", MousePosition);
        mat.SetVector("_MouseVelocity",new Vector2(mouse_move_x,mouse_move_y));
    }
}
