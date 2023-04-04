Shader "UI/BlendUI" 
{
    Properties
    {
        _MainTex1("Texture 1", 2D) = "white" {}
        _MainTex2("Texture 2", 2D) = "white" {}
        _BlendMap("Blend Map", 2D) = "white" {}
        _SizeX("Array Size X", Range(1, 10)) = 1
        _SizeY("Array Size Y", Range(1, 10)) = 1
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }

        Pass {
            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
#pragma exclude_renderers d3d11 gles
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex1;
            sampler2D _MainTex2;
            sampler2D _BlendMap;
            float _SizeX;
            float _SizeY;
            float _BlendArray[];

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

            v2f vert(appdata v) 
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target 
            {
                float4 blendValue = tex2D(_BlendMap, i.uv);
                float2 arrayIndex = float2(i.uv.x * _SizeX, i.uv.y * _SizeY);
                float2 arrayIndexFloored = floor(arrayIndex);
                float4 tex1Value = tex2D(_MainTex1, arrayIndexFloored / float2(_SizeX, _SizeY));
                float4 tex2Value = tex2D(_MainTex2, arrayIndexFloored / float2(_SizeX, _SizeY));
                fixed4 blendedColor = lerp(tex1Value, tex2Value, blendValue.r);
                return blendedColor;
            }
            ENDCG
        }
    }

        FallBack "UI/Default"
}