Shader "UI/SpritePureOutline"
{
    Properties
    {
        _MainTex           ("Sprite", 2D) = "white" {}
        _Color             ("Tint", Color) = (1,1,1,1)

        _OutlineColor      ("Outline Color", Color) = (1,1,1,1)
        _OutlineThickness  ("Outline Thickness (px)", Float) = 0.0
        _AlphaCutoff       ("Alpha Cutoff", Range(0,1)) = 0.1

        _StencilComp       ("Stencil Comparison", Float) = 8
        _Stencil           ("Stencil ID", Float) = 0
        _StencilOp         ("Stencil Operation", Float) = 0
        _StencilWriteMask  ("Stencil Write Mask", Float) = 255
        _StencilReadMask   ("Stencil Read Mask", Float) = 255
        _ColorMask         ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags{
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
        }

        Stencil {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "UI_SpritePureOutline"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            sampler2D _MainTex;
            float4    _MainTex_ST;
            fixed4    _Color;

            fixed4 _OutlineColor;
            float  _OutlineThickness; // in px (texture space)
            float  _AlphaCutoff;

            float4 _ClipRect;
            float4 _MainTex_TexelSize; // x=1/width, y=1/height

            struct appdata_t {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color    : COLOR;
            };

            struct v2f {
                float4 vertex        : SV_POSITION;
                float2 uv            : TEXCOORD0;
                float4 color         : COLOR;
                float4 worldPosition : TEXCOORD1;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // ���� ����
                fixed4 c0 = tex2D(_MainTex, i.uv) * i.color;

                // **����**: �β��� 0�̰ų� �ƿ����� ���İ� 0�̸� ������ ������ ���
                if (_OutlineThickness <= 0.0001 || _OutlineColor.a <= 0.0001)
                {
                    fixed4 c = c0;
                    #ifdef UNITY_UI_CLIP_RECT
                        c.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                    #endif
                    #ifdef UNITY_UI_ALPHACLIP
                        clip(c.a - 0.001);
                    #endif
                    return c;
                }

                half a0 = c0.a;

                // �ƿ����� ���ø� ������(�ؽ�ó �ȼ� ����)
                float2 px = _MainTex_TexelSize.xy * max(_OutlineThickness, 0.0);

                // 8���� �̿� ���� �ִ밪
                half aN = 0.0;
                aN = max(aN, tex2D(_MainTex, i.uv + float2( px.x,  0   )).a);
                aN = max(aN, tex2D(_MainTex, i.uv + float2(-px.x,  0   )).a);
                aN = max(aN, tex2D(_MainTex, i.uv + float2( 0,     px.y)).a);
                aN = max(aN, tex2D(_MainTex, i.uv + float2( 0,    -px.y)).a);
                aN = max(aN, tex2D(_MainTex, i.uv + float2( px.x,  px.y)).a);
                aN = max(aN, tex2D(_MainTex, i.uv + float2(-px.x,  px.y)).a);
                aN = max(aN, tex2D(_MainTex, i.uv + float2( px.x, -px.y)).a);
                aN = max(aN, tex2D(_MainTex, i.uv + float2(-px.x, -px.y)).a);

                // "�ٱ� �׵θ�" ����ũ: �̿��� ������������ ���� �ȼ��� ����
                half outsideHasAlpha = step(_AlphaCutoff, aN);
                half isTransparent   = 1.0 - step(_AlphaCutoff, a0);
                half outlineMask     = outsideHasAlpha * isTransparent;

                // �� ȥ��: ���δ� ����, �ٱ� �������� ���� �ƿ����� ����
                float3 rgb = lerp(c0.rgb, _OutlineColor.rgb, outlineMask);
                float  a   = saturate(a0 + outlineMask * _OutlineColor.a * (1.0 - a0));

                fixed4 c = fixed4(rgb, a);

                // UI Ŭ����/����ŷ
                #ifdef UNITY_UI_CLIP_RECT
                    c.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                #endif
                #ifdef UNITY_UI_ALPHACLIP
                    clip(c.a - 0.001);
                #endif

                return c;
            }
            ENDCG
        }
    }

    Fallback "UI/Default"
}
