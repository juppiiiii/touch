Shader "Custom/OutlineShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5

        _OutlineColor ("Outline Color", Color) = (1,0,0,1)
        _OutlineWidth ("Outline Width", Range(0,0.1)) = 0.005
        
        // _Glow ���� 0�̸� �⺻, 1�̸� ������ ȿ��(�ܰ��� �β� Ȯ��)�� ����
        _Glow ("Glow", Range(0,1)) = 0.0
        _GlowFactor ("Glow Factor", Range(1,5)) = 2.0
    }

    SubShader
    {
        Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" }
        LOD 200

        // 1. Outline Pass: �ܰ��� ������ (Cutout ����)
        Pass {
            Name "Outline"
            Tags { "LightMode" = "Always" }
            Cull Front  // �ո� �ø�: �޸��� �׷� �ܰ��� ȿ�� ����
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
                // ���� ��ǥ�� ��ȯ
                float4 worldPos4 = mul(unity_ObjectToWorld, v.vertex);
                float3 worldPos = worldPos4.xyz;
                // ���� �����̽������� ���� ���
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                // ī�޶� ��ġ�� ���� ������ ���� (ī�޶󿡼� ��������)
                float3 dirToCam = normalize(worldPos - _WorldSpaceCameraPos);
                // ī�޶� �����(���̴�) �����ϼ��� dot ���� 1�� ��������Ƿ�,
                // �Ƿ翧 �κд� 1 - dot ������ ����Ͽ� �������� ũ�� �Ѵ�.
                float edgeFactor = 1.0 - saturate(dot(worldNormal, dirToCam));
                edgeFactor = max(edgeFactor, 0.2); // �ּ� 0.2�� ȿ���� �༭ ���鿡���� �ణ�� �ܰ����� ���̰� ��.
                float effectiveWidth = max(_OutlineWidth * edgeFactor, _OutlineWidth * 0.5);
                if (_Glow > 0.5)
                {
                effectiveWidth *= _GlowFactor;
                }
                // ���� �������� ī�޶� �������� �����ϸ�, ī�޶� ���� �Ƿ翧 �ܰ����� �β��� �ܰ����� �����.
                worldPos += dirToCam * effectiveWidth;
                o.pos = UnityWorldToClipPos(float4(worldPos, 1.0));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 fragOutline(v2f i) : SV_Target
            {
                fixed4 texcol = tex2D(_MainTex, i.uv) * _Color;
                // ���� Ŭ������ �����ϵ�, _Cutoff���� �ణ ���� �Ӱ�ġ�� ���ٸ�:
                clip(texcol.a - (_Cutoff - 0.1));
                return _OutlineColor;
            }
            ENDCG
        }

        // 2. Main Pass: �⺻ ������Ʈ ������ (Cutout ����)
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
