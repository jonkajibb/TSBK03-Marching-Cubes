Shader "Custom/Triplanar"
{
    Properties
    {
        //_Color ("Color", Color) = (1,1,1,1)
        //_MainTex ("Albedo (RGB)", 2D) = "white" {}
        //_Glossiness ("Smoothness", Range(0,1)) = 0.5
        //_Metallic ("Metallic", Range(0,1)) = 0.0

        _TopTex("Top texture", 2D) = "white" {}
        _SideTex("Side texture", 2D) = "white" {}

        _TextureScale("Texture Scale", float) = 1.0
        _TriplanarBlendSharpness("Blend sharpness", float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Lambert

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

        void surf(Input IN, inout SurfaceOutput o)
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
