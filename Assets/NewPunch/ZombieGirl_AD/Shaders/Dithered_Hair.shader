Shader "SNP_Custom/Dithered_Hair"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_AmbientOcclusion("AmbientOcclusion", 2D) = "white" {}
		_MetallicSmoothness("MetallicSmoothness", 2D) = "white" {}
		[Normal]_Normal("Normal", 2D) = "bump" {}
		_AlphaClip("AlphaClip", Range( 0 , 1)) = 0.2
		_Desaturate("Desaturate", Range( -1 , 1)) = 0
		_DitherPower("DitherPower", Range( 1 , 10)) = 1
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_AlbedoColor("AlbedoColor", Color) = (1,1,1,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPosition;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float4 _AlbedoColor;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float _Desaturate;
		uniform sampler2D _MetallicSmoothness;
		uniform float4 _MetallicSmoothness_ST;
		uniform float _Smoothness;
		uniform sampler2D _AmbientOcclusion;
		uniform float4 _AmbientOcclusion_ST;
		uniform float _DitherPower;
		uniform float _AlphaClip;


		inline float Dither4x4Bayer( int x, int y )
		{
			const float dither[ 16 ] = {
				 1,  9,  3, 11,
				13,  5, 15,  7,
				 4, 12,  2, 10,
				16,  8, 14,  6 };
			int r = y * 4 + x;
			return dither[r] / 16; 
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			o.screenPosition = ase_screenPos;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode1 = tex2D( _Albedo, uv_Albedo );
			float3 desaturateInitialColor32 = ( _AlbedoColor * tex2DNode1 ).rgb;
			float desaturateDot32 = dot( desaturateInitialColor32, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar32 = lerp( desaturateInitialColor32, desaturateDot32.xxx, _Desaturate );
			o.Albedo = desaturateVar32;
			float2 uv_MetallicSmoothness = i.uv_texcoord * _MetallicSmoothness_ST.xy + _MetallicSmoothness_ST.zw;
			float4 tex2DNode4 = tex2D( _MetallicSmoothness, uv_MetallicSmoothness );
			o.Metallic = tex2DNode4.r;
			o.Smoothness = ( _Smoothness * tex2DNode4.a );
			float2 uv_AmbientOcclusion = i.uv_texcoord * _AmbientOcclusion_ST.xy + _AmbientOcclusion_ST.zw;
			o.Occlusion = tex2D( _AmbientOcclusion, uv_AmbientOcclusion ).r;
			o.Alpha = 1;
			float4 ase_screenPos = i.screenPosition;
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float4 ditherCustomScreenPos14 = ase_screenPosNorm;
			float2 clipScreen14 = ditherCustomScreenPos14.xy * _ScreenParams.xy;
			float dither14 = Dither4x4Bayer( fmod(clipScreen14.x, 4), fmod(clipScreen14.y, 4) );
			float blendOpSrc21 = ( tex2DNode1.a * ( dither14 * _DitherPower ) );
			float blendOpDest21 = tex2DNode1.a;
			clip( ( saturate( 	max( blendOpSrc21, blendOpDest21 ) )) - _AlphaClip );
		}

		ENDCG
	}
	Fallback "Diffuse"
	
}
