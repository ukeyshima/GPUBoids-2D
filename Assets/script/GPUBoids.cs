using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class GPUBoids : MonoBehaviour
{
    [System.Serializable]
    struct BoidData
    {
        public Vector3 Velocity;
        public Vector3 Position;
        public float Scale;
        public float Luminance;
    }

    struct InfluenceData{
        public Vector3 Force;
        public float Luminance;
    }

    const int SIMULATION_BLOCK_SIZE = 16;

    #region Boids Parameters
    [Range(16, 32768)]
    public int MaxObjectNum = 32768;
    public float CohesionNeighborhoodRadius = 2.0f;
    public float AlignmentNeighborhoodRadius = 2.0f;
    public float SeparateNeighborhoodRadius = 1.0f;
    public float MouseNeighborhoodRadius = 1.0f;

    public float MaxSpeed = 5.0f;
    public float MaxSteerForce = 0.5f;

    public float CohesionWeight = 1.0f;
    public float AlignmentWeight = 1.0f;
    public float SeparateWeight = 3.0f;
    public float MouseWeight = 1.0f;
    public float AvoidWallWeight = 10.0f;

    public Vector3 WallCenter = Vector3.zero;
    public Vector3 WallSize = new Vector3(52.0f, 52.0f, 52.0f);
    public MouseCoordinates mouseCoordinates;
    #endregion

    #region Built-in Resources
    public ComputeShader BoidsCS;
    #endregion

    #region Private Resources
    ComputeBuffer _boidinfluenceBuffer;
    ComputeBuffer _boidDataBuffer;
    #endregion

    #region Accessors
    public ComputeBuffer GetBoidDataBuffer()
    {
        return this._boidDataBuffer != null ? this._boidDataBuffer : null;
    }
    public int GetMaxObjectNum()
    {
        return this.MaxObjectNum;
    }
    public Vector3 GetSimulationAreaCenter()
    {
        return this.WallCenter;
    }
    public Vector3 GetSimulationAreaSize()
    {
        return this.WallSize;
    }
    #endregion

    #region MonoBehaviour Functions        
    void Start()
    {
        InitBuffer();
    }
    void Update()
    {
        Simulation();
    }
    void OnDestroy()
    {
        ReleaseBuffer();
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(WallCenter, WallSize);
    }
    #endregion

    #region Private Functions
    void InitBuffer()
    {
        _boidDataBuffer = new ComputeBuffer(MaxObjectNum, Marshal.SizeOf(typeof(BoidData)));
        _boidinfluenceBuffer = new ComputeBuffer(MaxObjectNum, Marshal.SizeOf(typeof(InfluenceData)));

        var influenceArr = new InfluenceData[MaxObjectNum];
        var boidDataArr = new BoidData[MaxObjectNum];
        for (var i = 0; i < MaxObjectNum; i++)
        {
            influenceArr[i].Force = Vector3.zero;
            influenceArr[i].Luminance = 0.5f;
            boidDataArr[i].Position = Random.insideUnitSphere * 20.0f;
            boidDataArr[i].Velocity = Random.insideUnitSphere * 20.0f;
            boidDataArr[i].Scale = Random.Range(0.8f, 2.0f);
            boidDataArr[i].Luminance = 0.5f;
        }
        _boidinfluenceBuffer.SetData(influenceArr);
        _boidDataBuffer.SetData(boidDataArr);
        influenceArr = null;
        boidDataArr = null;
    }
    void Simulation()
    {
        ComputeShader cs = BoidsCS;
        int id = -1;

        int threadGroupSize = Mathf.CeilToInt(MaxObjectNum / SIMULATION_BLOCK_SIZE);

        Vector2 MousePosition = mouseCoordinates.GetMousePosition();        

        id = cs.FindKernel("InfluenceCS");
        cs.SetInt("_MaxBoidObjectNum", MaxObjectNum);
        cs.SetFloat("_CohesionNeighborhoodRadius", CohesionNeighborhoodRadius);
        cs.SetFloat("_AlignmentNeighborhoodRadius", AlignmentNeighborhoodRadius);
        cs.SetFloat("_SeparateNeighborhoodRadius", SeparateNeighborhoodRadius);
        cs.SetFloat("_MouseNeighborhoodRadius", MouseNeighborhoodRadius);
        cs.SetFloat("_MaxSpeed", MaxSpeed);
        cs.SetFloat("_MaxSteerForce", MaxSteerForce);
        cs.SetFloat("_SeparateWeight", SeparateWeight);
        cs.SetFloat("_CohesionWeight", CohesionWeight);
        cs.SetFloat("_AlignmentWeight", AlignmentWeight);
        cs.SetFloat("_MouseWeight", MouseWeight);
        cs.SetVector("_WallCenter", WallCenter);
        cs.SetVector("_WallSize", WallSize);
        cs.SetFloat("_AvoidWallWeight", AvoidWallWeight);
        cs.SetVector("_MousePosition", MousePosition);
        cs.SetBuffer(id, "_BoidDataBufferRead", _boidDataBuffer);
        cs.SetBuffer(id, "_BoidInfluenceBufferWrite", _boidinfluenceBuffer);
        cs.Dispatch(id, threadGroupSize, 1, 1);

        id = cs.FindKernel("IntegrateCS");
        cs.SetFloat("_DeltaTime", Time.deltaTime);
        cs.SetBuffer(id, "_BoidInfluenceBufferRead", _boidinfluenceBuffer);
        cs.SetBuffer(id, "_BoidDataBufferWrite", _boidDataBuffer);
        cs.Dispatch(id, threadGroupSize, 1, 1);
    }
    void ReleaseBuffer()
    {
        if (_boidDataBuffer != null)
        {
            _boidDataBuffer.Release();
            _boidDataBuffer = null;
        }
        if (_boidinfluenceBuffer != null)
        {
            _boidinfluenceBuffer.Release();
            _boidinfluenceBuffer = null;
        }
    }
    #endregion
}
