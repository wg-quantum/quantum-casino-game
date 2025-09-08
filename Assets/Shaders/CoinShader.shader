Shader "Custom/CoinShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MetalColor ("Metal Color", Color) = (1, 0.84, 0, 1) // 金色
        _MySpecColor ("Specular Color", Color) = (1,1,1,1)
        _Shininess ("Shininess", Range(0.03, 1)) = 0.3
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Float) = 0.005
    }

    SubShader
    {
        CGPROGRAM
        #pragma surface surf BlinnPhong
        sampler2D _MainTex;
        fixed4 _MetalColor;
        fixed4 _MySpecColor;
        half _Shininess;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = _MetalColor.rgb * tex.rgb;
            o.Specular = _MySpecColor.r;
            o.Gloss = _Shininess;
        }
        ENDCG

        // アウトラインパス
        Pass
        {
            Name "OUTLINE"
            Cull Back
            ZWrite On
            ZTest LEqual
            ColorMask RGB

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float _OutlineWidth;

            v2f vert (appdata v)
            {
                v2f o;
                float3 norm = normalize(v.normal);
                v.vertex.xyz += norm * _OutlineWidth;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 _OutlineColor;

            fixed4 frag (v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
