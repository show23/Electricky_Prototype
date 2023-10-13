Shader "HDRP/UnlitEN"
{
    Properties
    {
        _Color("Color", Color) = (1.0, 0.0, 0.0, 0.5)
    }

        SubShader
    {
        Tags
        {
            "RenderPipeline" = "HDRP"
            "Queue" = "Transparent"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/Common.hlsl"

            struct Attributes
            {
                float4 vertex : POSITION;
            };

            struct Varyings
            {
                float4 position : TEXCOORD0;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.position = UnityObjectToClipPos(input.vertex);
                return output;
            }

            half4 frag(Varyings i) : SV_Target
            {
                return _Color;
            }
            ENDHLSL
        }
    }
}
