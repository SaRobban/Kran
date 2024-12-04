Shader "Example/Tint Final Color" {
    Properties {
      _MainTex ("Texture", 2D) = "white" {}
      _ColorTint ("Tint", Color) = (1.0, 0.6, 0.6, 1.0)
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf Lambert finalcolor:mycolor

      struct Input {
          float2 uv_MainTex;
          float3 worldPos;// = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0)).xyz;
      };

      fixed4 _ColorTint;
      void mycolor (Input IN, SurfaceOutput o, inout fixed4 color)
      {
          float distX = IN.worldPos.x;
          float distY = IN.worldPos.z;
          float flatDistFromOrigo = distX * distX * 0.01f + distY * distY * 0.01f;
          flatDistFromOrigo -= 3;
          flatDistFromOrigo = clamp(flatDistFromOrigo, 0, 1);

          half4 bg;
          bg.r = 0.19f;
          bg.g = 0.20f;
          bg.b = 0.21f;
          bg.a = 1;

          half4 col = color  * _ColorTint;

          half4 c = lerp(col,bg, flatDistFromOrigo);

          color = c;
      }

      sampler2D _MainTex;
      void surf (Input IN, inout SurfaceOutput o) {



        

          half3 c = tex2D(_MainTex, IN.uv_MainTex).rgb;
           o.Albedo = c;
      }
      ENDCG
    } 
    Fallback "Diffuse"
  }