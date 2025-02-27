Shader "Custom/OutlineShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5

        _OutlineColor ("Outline Color", Color) = (1,0,0,1)
        _OutlineWidth ("Outline Width", Range(0,0.1)) = 0.005
        
        // _Glow 값이 0이면 기본, 1이면 빛나는 효과(외곽선 두께 확대)를 적용
        _Glow ("Glow", Range(0,1)) = 0.0
        _GlowFactor ("Glow Factor", Range(1,5)) = 2.0
    }

    SubShader
    {
        Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" }
        LOD 200

        // 1. Outline Pass: 외곽선 렌더링 (Cutout 적용)
        Pass {
            Name "Outline"
            Tags { "LightMode" = "Always" }
            Cull Front  // 앞면 컬링: 뒷면을 그려 외곽선 효과 생성
            ZWrite On
            Offset 10, 10

            CGPROGRAM
            #pragma vertex vertOutline
            #pragma fragment fragOutline
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Cutoff;

            fixed4 _OutlineColor;
            float _OutlineWidth;
            float _Glow;
            float _GlowFactor;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vertOutline(appdata v)
            {
                v2f o;
                // 월드 좌표로 변환
                float4 worldPos4 = mul(unity_ObjectToWorld, v.vertex);
                float3 worldPos = worldPos4.xyz;
                // 월드 스페이스에서의 정점 노멀
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                // 카메라 위치와 정점 사이의 방향 (카메라에서 정점까지)
                float3 dirToCam = normalize(worldPos - _WorldSpaceCameraPos);
                // 카메라에 가까운(보이는) 정점일수록 dot 값이 1에 가까워지므로,
                // 실루엣 부분는 1 - dot 값으로 계산하여 오프셋을 크게 한다.
                float edgeFactor = 1.0 - saturate(dot(worldNormal, dirToCam));
                edgeFactor = max(edgeFactor, 0.2); // 최소 0.2의 효과를 줘서 정면에서도 약간의 외곽선을 보이게 함.
                float effectiveWidth = max(_OutlineWidth * edgeFactor, _OutlineWidth * 0.5);
                if (_Glow > 0.5)
                {
                effectiveWidth *= _GlowFactor;
                }
                // 정점 오프셋을 카메라 방향으로 적용하면, 카메라에 담기는 실루엣 외곽에만 두꺼운 외곽선이 생긴다.
                worldPos += dirToCam * effectiveWidth;
                o.pos = UnityWorldToClipPos(float4(worldPos, 1.0));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 fragOutline(v2f i) : SV_Target
            {
                fixed4 texcol = tex2D(_MainTex, i.uv) * _Color;
                // 만약 클리핑을 적용하되, _Cutoff보다 약간 낮은 임계치를 쓴다면:
                clip(texcol.a - (_Cutoff - 0.1));
                return _OutlineColor;
            }
            ENDCG
        }

        // 2. Main Pass: 기본 오브젝트 렌더링 (Cutout 적용)
        Pass {
            Name "Main"
            Cull Back
            ZWrite On

            CGPROGRAM
            #pragma vertex vertMain
            #pragma fragment fragMain
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Cutoff;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vertMain(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 fragMain(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                clip(col.a - _Cutoff);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
