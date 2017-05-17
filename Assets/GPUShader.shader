Shader "GPUShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 color:TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			v2f vert(appdata v)
			{
					v2f o;
			        // o.vertex 直接决定了将当前 fragment 输出到 RenderTexture 上的哪个像素中
			        // o.vertex.xy 经过投影变换后的值都是在 -1 到 1 之间
			        //             我们需要知道当前应该输出到 [-1,1] 之间的哪个值上，这就需要在上文中创建 Mesh 时填充顶点数据时指定好，这里直接读取即可
			        // o.vertex.z 这个值我们其实用不到，但是不能随便设置，因为 OpenGL 是 [-1,1]，而 DirectX 是 [0,1]，
			        //            超出这个范围会被裁切掉，所以要同时兼顾到，设置为 0
			        // o.vertex.w 这是用来做齐次坐标变换的，将顶点转换到 Canonical View Volume。简单来说最终的会将 o.vertex.xy 除以 w，来转换到齐次裁剪空间坐标系，
			        //            但是我们不希望进行这个操作，以免破坏了精心计算的 o.vertex.xy，所以设置为 1
			        o.vertex.xy = v.vertex.xy;
			        o.vertex.z=0;
			        o.vertex.w=1;
			        // 这是用来解决平台差异的
			        // 因为 OpenGL 的纹理坐标 (0,0) 点在左下角，而 DirectX 的纹理坐标 (0,0) 点在左上角
			        #if UNITY_UV_STARTS_AT_TOP
			        float scale = -1.0;
			        #else
			        float scale = 1.0;
			        #endif
			        o.vertex.y *= scale;
			        int ma;
			        int i;
			        for(int m=0;m<500;m++)
			        {
			        	i = ((v.normal.x+m)*v.normal.y)%(v.normal.z);
			        	ma+=i;
			        }
			        float f = float(i)/100;
			        o.color = float4(f,f,f,1);
			        return o;
			}
			 
			// 注意这里的返回值类型，因为用它表示三维空间中的坐标，所以使用 float
			// 同样 v2f 结构中 color 的类型也要注意
			float4 frag(v2f i) : SV_Target
			{
			        return i.color;
			}
			ENDCG
		}
	}
}
