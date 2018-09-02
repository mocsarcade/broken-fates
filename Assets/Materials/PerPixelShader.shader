Shader "Custom/PerPixelShader"
{
     Properties
     {
        _Diffuse ("Diffuse", Color) = (1,1,1,1)
        _MainTex("Texture",2D) = "white"{}
        _Cutoff("Alpha Cutoff",Range(0,1)) = 0.5
     }
     
     SubShader {
         Tags{"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
         //ZWrite Off
         Pass
         {
             Tags{"LightMode" = "ForwardBase"}
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
             #pragma multi_compile_fwdbase
             #pragma target 3.0
             #include "UnityCG.cginc"
             #include "Lighting.cginc"
             //fixed4 _Diffuse;


             struct appdata
             {
                 float4 vertex : POSITION;
                 float3 normal : NORMAL;
                 float2 uv : TEXCOORD0;
             };
             struct v2f
             {
                 float4 vertex : SV_POSITION;
                 float3 worldNormal : TEXCOORD0;
                 float3 worldPos : TEXCOORD1;
                 float2 uv : TEXCOORD2;
             };
             sampler2D _MainTex;
             float4 _MainTex_ST;
             fixed _Cutoff;

             v2f vert (appdata v)
             {
                 v2f o;
                 o.vertex = UnityObjectToClipPos(v.vertex);

                 o.worldNormal = UnityObjectToWorldNormal(v.normal);
                 o.worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;
                 o.uv = TRANSFORM_TEX(v.uv,_MainTex);
                 return o;
             }


             fixed4 frag (v2f i) : SV_Target
             {

                 fixed3 worldNormal = normalize(i.worldNormal);
                 fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                 fixed4 albedo = tex2D(_MainTex,i.uv);
                 fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo.rgb;
                 clip (albedo.a - _Cutoff);//透明度

                 fixed3 diffuse = _LightColor0.rgb * albedo * max(0,dot(worldNormal,worldLightDir));
                 //fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                 //fixed3 halfDir = normalize(worldLightDir+viewDir);
                 //fixed3 color = ambient + diffuse;
                 return fixed4(ambient+diffuse,1.0);
             }
             ENDCG
         }
         Pass
         {
             Tags{"LightMode" = "ForwardAdd"}
             BlendOp Max
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
             #pragma multi_compile_fwdadd
             #pragma target 3.0
             #include "UnityCG.cginc"
             #include "Lighting.cginc"
             #include "AutoLight.cginc"
             struct appdata
             {
                 float4 vertex : POSITION;
                 float3 normal : NORMAL;
                 float2 uv : TEXCOORD0;
             };
             struct v2f
             {
                 float4 vertex : SV_POSITION;
                 float3 worldNormal : TEXCOORD0;
                 float3 worldPos : TEXCOORD1;
                 float2 uv : TEXCOORD2;
             };
             sampler2D _MainTex;
             float4 _MainTex_ST;
             fixed _Cutoff;

             v2f vert (appdata v)
             {
                 v2f o;
                 o.vertex = UnityObjectToClipPos(v.vertex);

                 o.worldNormal = UnityObjectToWorldNormal(v.normal);
                 o.worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;
                 o.uv = TRANSFORM_TEX(v.uv,_MainTex);
                 return o;
             }


             fixed4 frag (v2f i) : SV_Target
             {

                 fixed3 worldNormal = normalize(i.worldNormal);


             #ifdef USING_DIRECTINAL_LIGHT
                 fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);

             #else
                 fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos.xyz);

             #endif
                 fixed4 albedo = tex2D(_MainTex,i.uv);
                 //fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo.rgb;
                 clip (albedo.a - _Cutoff);//透明度

                 fixed3 diffuse = _LightColor0.rgb * albedo * max(0,dot(worldNormal,worldLightDir));
                 //fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
                 //fixed3 halfDir = normalize(worldLightDir+viewDir);
                 //fixed3 specular = _LightColor0.rgb * pow(saturate(dot(worldNormal,halfDir)),255);
                 //fixed3 color = ambient + diffuse;
                 #ifdef USING_DIRECTIONAL_LIGHT
                     fixed atten = 1.0;
                 #else
                     #if defined (POINT)
                         float3 lightCoord = mul(unity_WorldToLight, float4(i.worldPos, 1)).xyz;
                         fixed atten = tex2D(_LightTexture0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;
                     #elif defined (SPOT)
                         float4 lightCoord = mul(unity_WorldToLight, float4(i.worldPos, 1));
                         fixed atten = (lightCoord.z > 0) * tex2D(_LightTexture0, lightCoord.xy / lightCoord.w + 0.5).w * tex2D(_LightTextureB0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;

                     #else
                         fixed atten = 1.0;
                     #endif
                 #endif
                 return fixed4(diffuse*atten,1.0);
             }
             ENDCG
         }
     }

     FallBack "Sprite/Diffuse"

}
