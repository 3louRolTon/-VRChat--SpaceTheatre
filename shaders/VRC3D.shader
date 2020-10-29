// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "CKEP/VRC3D"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "white" {}
		[Toggle][Toggle]_OverUnder("Over-Under", Float) = 0
		[Toggle][Toggle]_SwapEyes("Swap Eyes", Float) = 0
		//[Toggle]_ApplyGamma("Apply Gamma", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityCG.cginc"
		#pragma target 3.5
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _ApplyGamma;
		uniform sampler2D _MainTex;
		uniform half _OverUnder;
		uniform float4 _MainTex_ST;
		uniform half _SwapEyes;


		int StereoEyeIndex(  )
		{
			return  unity_StereoEyeIndex;
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			int localStereoEyeIndex79 = StereoEyeIndex();
			float lerpResult64 = lerp( ( uv_MainTex.x * 0.5 ) , ( ( uv_MainTex.x * 0.5 ) + 0.5 ) , lerp((float)localStereoEyeIndex79,( -localStereoEyeIndex79 + 1.0 ),_SwapEyes));
			float2 appendResult71 = (float2(lerpResult64 , uv_MainTex.y));
			float lerpResult34 = lerp( ( 0.5 + ( uv_MainTex.y * 0.5 ) ) , ( uv_MainTex.y * 0.5 ) , lerp((float)localStereoEyeIndex79,( -localStereoEyeIndex79 + 1.0 ),_SwapEyes));
			float2 appendResult53 = (float2(uv_MainTex.x , lerpResult34));
			float4 tex2DNode8 = tex2D( _MainTex, lerp(appendResult71,appendResult53,_OverUnder) );
			float3 gammaToLinear82 = GammaToLinearSpace( tex2DNode8.rgb );
			o.Emission = lerp(tex2DNode8,float4( gammaToLinear82 , 0.0 ),_ApplyGamma).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
7;29;1586;1124;-617.1183;312.8015;1;True;True
Node;AmplifyShaderEditor.CustomExpressionNode;79;-1034.172,152.8626;Float;False;return  unity_StereoEyeIndex@;0;False;0;StereoEyeIndex;False;False;0;0;1;INT;0
Node;AmplifyShaderEditor.TexturePropertyNode;83;-1282.495,-11.4305;Float;True;Property;_MainTex;Main Tex;0;0;Create;True;0;0;False;0;None;75a8df9661921034f95af55c309698aa;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;85;-852.2112,-107.6333;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;38;-807.2781,94.10258;Float;False;Constant;_Float0;Float 0;1;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;50;-805.7516,220.4265;Float;False;1;0;INT;0;False;1;INT;0
Node;AmplifyShaderEditor.RangedFloatNode;105;-806.849,319.1114;Float;False;Constant;_Float1;Float 1;4;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;87;-506.7099,269.3082;Float;False;928.7204;444.6441;OU;5;34;53;103;99;102;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;86;-498.0164,-346.8578;Float;False;901.3223;452.0164;SBS;5;71;64;101;98;100;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;-438.4233,370.1544;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;104;-653.849,250.6748;Float;False;2;2;0;INT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;100;-462.7253,-93.81976;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;-380.2573,543.0626;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;52;-408.3782,138.6505;Half;False;Property;_SwapEyes;Swap Eyes;2;1;[Toggle];Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;98;-422.112,-251.3322;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;103;-249.4233,365.1544;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;101;-295.7253,-93.81976;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;64;-29.11566,-227.7092;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;34;-18.88602,387.9546;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;71;246.2248,-136.131;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;53;246.468,336.5813;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ToggleSwitchNode;68;531.7948,-15.79852;Half;False;Property;_OverUnder;Over-Under;1;1;[Toggle];Create;True;0;0;False;0;0;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;8;757.0021,-29.09985;Float;True;Property;_sampler;sampler;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;LockedToTexture2D;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GammaToLinearNode;82;1077.613,94.70169;Float;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ToggleSwitchNode;74;1303.184,26.5051;Float;False;Property;_ApplyGamma;Apply Gamma;3;0;Create;True;0;0;False;0;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1545.804,-14.60001;Float;False;True;3;Float;ASEMaterialInspector;0;0;Unlit;CKEP/VRC3D;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;85;2;83;0
WireConnection;50;0;79;0
WireConnection;102;0;85;2
WireConnection;102;1;38;0
WireConnection;104;0;50;0
WireConnection;104;1;105;0
WireConnection;100;0;85;1
WireConnection;100;1;38;0
WireConnection;99;0;85;2
WireConnection;99;1;38;0
WireConnection;52;0;79;0
WireConnection;52;1;104;0
WireConnection;98;0;85;1
WireConnection;98;1;38;0
WireConnection;103;0;38;0
WireConnection;103;1;102;0
WireConnection;101;0;100;0
WireConnection;101;1;38;0
WireConnection;64;0;98;0
WireConnection;64;1;101;0
WireConnection;64;2;52;0
WireConnection;34;0;103;0
WireConnection;34;1;99;0
WireConnection;34;2;52;0
WireConnection;71;0;64;0
WireConnection;71;1;85;2
WireConnection;53;0;85;1
WireConnection;53;1;34;0
WireConnection;68;0;71;0
WireConnection;68;1;53;0
WireConnection;8;0;83;0
WireConnection;8;1;68;0
WireConnection;82;0;8;0
WireConnection;74;0;8;0
WireConnection;74;1;82;0
WireConnection;0;2;74;0
ASEEND*/
//CHKSM=6CDC4F546D0BA5E79066D037A622603E371F8343