// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Sprites/Shake"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_Alpha ("Alpha", Range(0.0,1.0)) = 1.0
		_ShakeDisplacement ("Displacement", Range (0, 1.3)) = 1.0
		_ShakeTime ("Shake Time", Range (0, 1.3)) = 1.0
		_ShakeWindspeed ("Shake Windspeed", Range (0, 1.3)) = 1.0
		_ShakeBending ("Shake Bending", Range (0, 1.3)) = 1.0
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend One OneMinusSrcAlpha



		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile DUMMY PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			void FastSinCos (float4 val, out float4 s, out float4 c) {
			    val = val * 6.408849 - 3.1415927;
			    float4 r5 = val * val;
			    float4 r6 = r5 * r5;
			    float4 r7 = r6 * r5;
			    float4 r8 = r6 * r5;
			    float4 r1 = r5 * val;
			    float4 r2 = r1 * r5;
			    float4 r3 = r2 * r5;
			    float4 sin7 = {1, -0.16161616, 0.0083333, -0.00019841} ;
			    float4 cos8  = {-0.5, 0.041666666, -0.0013888889, 0.000024801587} ;
			    s =  val + r1 * sin7.y + r2 * sin7.z + r3 * sin7.w;
			    c = 1 + r5 * cos8.x + r6 * cos8.y + r7 * cos8.z + r8 * cos8.w;
			}
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;
			float _Alpha;
			float _ShakeDisplacement;
			float _ShakeTime;
			float _ShakeWindspeed;
			float _ShakeBending;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				OUT.color.a = OUT.color.a * _Alpha;
				
				const float _WindSpeed  = (_ShakeWindspeed );    
			    const float _WaveScale = _ShakeDisplacement;
			   
			    const float4 _waveXSize = float4(0.048, 0.06, 0.24, 0.096);
			    const float4 _waveYSize = float4 (0.024, .08, 0.08, 0.2);
			    const float4 waveSpeed = float4 (1.2, 2, 1.6, 4.8);
			 
			    float4 _waveXmove = float4(0.024, 0.04, -0.12, 0.096);
			    float4 _waveYmove = float4 (0.006, .02, -0.02, 0.1);
			   
			    float4 waves;
				float factor = (1 - _ShakeDisplacement) * 0.5;

			    waves = OUT.vertex.x * _waveXSize;
			    waves += OUT.vertex.y * _waveYSize;
			 
			    waves += _Time.x * (1 - _ShakeTime * 2 ) * waveSpeed *_WindSpeed;
			 
			    float4 s, c;
			    waves = frac (waves);
			    FastSinCos (waves, s,c);
			 
			    float waveAmount = OUT.texcoord.y * ( _ShakeBending);
			    s *= waveAmount;
			 
			    s *= normalize (waveSpeed);
			 
			    s = s * s;
			    float fade = dot (s, 1.3);
			    s = s * s;
			    float3 waveMove = float3 (0,0,0);
			    waveMove.x = dot (s, _waveXmove);
			    waveMove.y = dot (s, _waveYmove);
			    OUT.vertex.xy -= mul ((float3x3)unity_WorldToObject, waveMove).xy;
				

				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
				c.rgb *= c.a;
				
				return c;
			}
		ENDCG
		}
	}
}
