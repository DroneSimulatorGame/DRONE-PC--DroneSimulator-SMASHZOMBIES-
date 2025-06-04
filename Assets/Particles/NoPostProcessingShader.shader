Shader "Custom/NoPostProcessingShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Overlay" }
        LOD 100

        Pass
        {
            ZWrite Off
            Cull Off
            Lighting Off

            SetTexture[_MainTex] {
                combine primary * texture
            }
        }
    }
}
