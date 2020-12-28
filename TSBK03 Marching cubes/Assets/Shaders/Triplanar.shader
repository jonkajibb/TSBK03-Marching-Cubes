Shader "Custom/Triplanar"
{
    Properties
    {
        _Color("Tint", Color) = (0, 0, 0, 1)

        _TopTex("Top texture", 2D) = "white" {}
        _SideTex("Side texture", 2D) = "white" {}

        _MainTex("Main texture", 2D) = "white" {}

        _TextureScale("Texture Scale", float) = 1.0
        _BlendSharpness("Blend sharpness", float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue" = "Geometry"}
        LOD 200
        /*
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //#pragma surface surf Standard fullforwardshadows

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 normal : NORMAL;
            };

            sampler2D _TopTex;
            sampler2D _SideTex;
            float _BlendSharpness;
            float _TextureScale;

            v2f vert(appdata v) {
                v2f o;

                //calculate the position in clip space to render the object
                o.position = UnityObjectToClipPos(v.vertex);
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
                float2 xUV = i.worldPos.yz / _TextureScale;
                float2 yUV = i.worldPos.xz / _TextureScale;
                float2 zUV = i.worldPos.xy / _TextureScale;

                fixed4 xDiffuse = tex2D(_SideTex, xUV);
                fixed4 yDiffuse = tex2D(_TopTex, yUV);
                fixed4 zDiffuse = tex2D(_SideTex, zUV);

                // Weights from world normals
                float3 blendWeights = pow(abs(i.normal), _BlendSharpness);
                blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z);

                xDiffuse *= blendWeights.x;
                yDiffuse *= blendWeights.y;
                zDiffuse *= blendWeights.z;

                fixed4 col = (xDiffuse + yDiffuse + zDiffuse);

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
        float _BlendSharpness;

        
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

            float3 blendWeights = pow(abs(IN.worldNormal), _BlendSharpness);
            blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z);



            o.Albedo = xDiffuse * blendWeights.x + yDiffuse * blendWeights.y + zDiffuse * blendWeights.z;
        }
        
        ENDCG
        
    }
    FallBack "Diffuse"
}
