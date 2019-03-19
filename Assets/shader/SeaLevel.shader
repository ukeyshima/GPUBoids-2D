Shader "Unlit/SeaLevel"
{
    Properties{}
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha 
        LOD 100

        Pass{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag alpha:fade                  

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;                
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float2 _MousePosition;
            float2 _MouseVelocity;
            float2 _RawMousePosition;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // o.vertex.z+=0.1/abs(sqrt(dot(_RawMousePosition-v.vertex.xy,_RawMousePosition-v.vertex.xy))-0.1*fmod(_Time.y*10.0,30.0));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);                
                return o;
            }

            float random1(float3 p){
                return frac(sin(dot(p.xyz,float3(12.9898,46.2346,78.233)))*43758.5453123)*2.0-1.0;
            }
            float random2(float3 p){
                return frac(sin(dot(p.xyz,float3(73.6134,21.6712,51.5781)))*51941.3781931)*2.0-1.0;
            }
            float random3(float3 p){
                return frac(sin(dot(p.xyz,float3(39.1831,85.3813,16.2981)))*39183.4971731)*2.0-1.0;
            }
            float perlinNoise(float3 p){
                float3 i1=floor(p);    
                float3 i2=i1+float3(1.0,0.0,0.0);
                float3 i3=i1+float3(0.0,1.0,0.0);
                float3 i4=i1+float3(1.0,1.0,0.0);
                float3 i5=i1+float3(0.0,0.0,1.0);
                float3 i6=i1+float3(1.0,0.0,1.0);
                float3 i7=i1+float3(0.0,1.0,1.0);
                float3 i8=i1+float3(1.0,1.0,1.0);
                float3 f1=float3(random1(i1),random2(i1),random3(i1));
                float3 f2=float3(random1(i2),random2(i2),random3(i2));
                float3 f3=float3(random1(i3),random2(i3),random3(i3));
                float3 f4=float3(random1(i4),random2(i4),random3(i4));
                float3 f5=float3(random1(i5),random2(i5),random3(i5));
                float3 f6=float3(random1(i6),random2(i6),random3(i6));
                float3 f7=float3(random1(i7),random2(i7),random3(i7));
                float3 f8=float3(random1(i8),random2(i8),random3(i8));
                float3 k1=p-i1;
                float3 k2=p-i2;
                float3 k3=p-i3;
                float3 k4=p-i4;
                float3 k5=p-i5;
                float3 k6=p-i6;
                float3 k7=p-i7;
                float3 k8=p-i8;
                float3 j=frac(p);
                j=j*j*(3.0-2.0*j);
              	return lerp(lerp(lerp(dot(f1,k1),dot(f2,k2),j.x),lerp(dot(f3,k3),dot(f4,k4),j.x),j.y),lerp(lerp(dot(f5,k5),dot(f6,k6),j.x),lerp(dot(f7,k7),dot(f8,k8),j.x),j.y),j.z)*0.95+0.05;
            }   

            float2 random4( float2 p ) {
                return frac(sin(float2(dot(p,float2(127.1,311.7)),dot(p,float2(269.5,183.3))))*43758.5453);
            }

            float boronoi(float3 p){
                float2 i = floor(p);
                float2 j = frac(p);
                float m_dist = 1.;
                for (float y= -1; y <= 1; y++) {
                    for (float x= -1; x <= 1; x++) {            
                    float2 neighbor = float2(x,y);            
                    float2 poi = random4(i+neighbor);			
                    poi = 0.5 + 0.5*sin(_Time.y + 6.2831*poi);			
                    float2 diff = neighbor +poi- j;            
                    float dist = pow(length(diff),1.8);
                    m_dist = min(m_dist, dist);
                    }
                }
                return m_dist*2.0;
            }

            fixed4 frag (v2f i) : SV_Target{                
                float2 boronoiUV=i.uv;
                boronoiUV.xy+=perlinNoise(float3(i.uv*15.0,_Time.y/2.0))/10.0;
                float boronoiColor=boronoi(float3(boronoiUV.x*5.0,boronoiUV.y*5.0,_Time.y/2.0))/2.0;
                float3 color=(float3)boronoiColor*float3(1.0,1.0,1.5);                
                return fixed4(color,0.6);
            }
            ENDCG
        }
    }
}
