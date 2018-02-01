// Simplified Skybox shader. Differences from regular Skybox one:
// - no tint color

Shader "Mobile/Skybox-DayNight" {
Properties {
	_Color ("Color Tint", COLOR) = (1, 1, 1, 1)
	_FrontTex ("Front (+Z)", 2D) = "white" {}
	_BackTex ("Back (-Z)", 2D) = "white" {}
	_LeftTex ("Left (+X)", 2D) = "white" {}
	_RightTex ("Right (-X)", 2D) = "white" {}
	_UpTex ("Up (+Y)", 2D) = "white" {}
	_DownTex ("Down (-Y)", 2D) = "white" {}
}

SubShader {
	Tags { "Queue"="Background" "RenderType"="Background" }
	Cull Off ZWrite Off Fog { Mode Off }
	Pass {
		SetTexture [_FrontTex] {
			ConstantColor [_Color]
			combine texture * constant
		}
	}
	Pass {
		SetTexture [_BackTex] {
			ConstantColor [_Color]
			combine texture * constant
		}
	}
	Pass {
		SetTexture [_LeftTex] {
			ConstantColor [_Color]
			combine texture * constant
		}
	}
	Pass {
		SetTexture [_RightTex] {
			ConstantColor [_Color]
			combine texture * constant
		}
	}
	Pass {
		SetTexture [_UpTex] {
			ConstantColor [_Color]
			combine texture * constant
		}
	}
	Pass {
		SetTexture [_DownTex]  {
			ConstantColor [_Color]
			combine texture * constant
		}
	}
}
}
