Shader "Custom/Triplanar"
{
    Properties
    {
        _Color("Tint", Color) = (0, 0, 0, 1)

        _TopTex("Top texture", 2D) = "white" {}
        _SideTex("Side texture", 2D) = "white" {}

        _MainTex("Main texture", 2D) = "white" {}

        _TextureScale("Texture Scale", float) = 1.0
        _TriplanarBlendSharpness("Blend sharpness", float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue" = "Geometry"}
        LOD 200

        /*Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 normal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v) {
                v2f o;
                //calculate the position in clip space to render the object
                o.vertex = UnityObjectToClipPos(v.vertex);
                //calculate world position of vertex
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = worldPos.xyz;
                //calculate world normal
                float3 worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
                o.normal = normalize(worldNormal);
                
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
            // apply fog
            UNITY_APPLY_FOG(i.fogCoord, col);
            return col;
        }
        ENDCG
        }*/
            
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _TopTex;
        sampler2D _SideTex;
        float _TextureScale;
        float _TriplanarBlendSharpness;




        
        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float2 xUV = IN.worldPos.yz / _TextureScale;
            float2 yUV = IN.worldPos.xz / _TextureScale;
            float2 zUV = IN.worldPos.xy / _TextureScale;

            float3 xDiffuse = tex2D(_SideTex, xUV);
            float3 yDiffuse = tex2D(_TopTex, yUV);
            float3 zDiffuse = tex2D(_SideTex, zUV);

            float3 blendWeights = pow(abs(IN.worldNormal), _TriplanarBlendSharpness);
            blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z);



            o.Albedo = xDiffuse * blendWeights.x + yDiffuse * blendWeights.y + zDiffuse * blendWeights.z;
        }
        
        ENDCG
            
    }
    FallBack "Diffuse"
}
