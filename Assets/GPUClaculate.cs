using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[ExecuteInEditMode]
public class GPUClaculate : MonoBehaviour {
	public bool isVis;
	private RenderTexture rt;
	private RenderTexture oldRT;
	private Texture2D initTex;
	public int rtSize;
	public Material mat;
	public Mesh mesh;
	Vector3[] vertices;
	Vector3[] normals;
	Color[] colors;
	Vector2[] uv;
	Vector4[] tangents;
	int[] triangles;
	public GameObject go;
	// Use this for initialization
	void Start () {
		rt = CreatRenderTexture ();
		oldRT = CreatRenderTexture ();

		initTex = new Texture2D(rtSize, rtSize, TextureFormat.RGBAHalf, false);
		initTex.filterMode = FilterMode.Point;
		initTex.anisoLevel = 0;
		Color[] colors = initTex.GetPixels();
		int numColor = colors.Length;
		for (int i = 0; i < numColor; ++i)
		{
			colors[i] = new Color(0, 0, 0);
		}
		initTex.SetPixels(colors);
		initTex.Apply();

		Graphics.Blit(initTex,rt);
		Graphics.Blit(initTex,oldRT);

		CreatMesh ();
		float d = 1f / (rtSize - 1);
		for (int n = 0; n < rtSize; n++) {
			for (int m = 0; m < rtSize; m++) {
				int i = Random.Range (0,100);
				SetNormal (m,n,new Vector3(i,3f,100f));
			}
		}
		mesh.normals = normals;
		if(isVis)
		go.GetComponent<MeshFilter> ().mesh = mesh;

		for (int n = 0; n < rtSize; n++) {
			for (int m = 0; m < rtSize; m++) {
				int i = Random.Range (0,100);
				SetNormal (m,n,new Vector3(i,Random.Range (0,100),20));
			}
		}
		mesh.normals = normals;
	}
	public void SetNormal(int x,int y,Vector3 v)
	{
		int index = x + y * rtSize;
		normals [index] = v;
	}
	public void CreatMesh()
	{
		if (go != null)
			GameObject.DestroyImmediate (go);
		mesh = new Mesh();
		float dealt = 2f / rtSize;
		float d = 1f / (rtSize - 1);
		vertices = new Vector3[rtSize*rtSize];
		triangles = new int[(rtSize-1)*(rtSize-1)*2*6];
		normals = new Vector3[vertices.Length];
		uv = new Vector2[rtSize*rtSize];

		float uvD = 1f/(rtSize-1);
		for (int n = 0; n < rtSize; n++) {
			for (int m = 0; m < rtSize; m++) {
				vertices [m+n*rtSize] = new Vector3 (-1f + m * dealt+dealt*0.5f, -1f + n * dealt+dealt*0.5f, 1f);
				uv [m + n * rtSize] = new Vector2(m*uvD,n*uvD);
			}
		}
		int count = 0;
		for (int n = 0; n < rtSize-1; n++) {
			for (int m = 0; m < rtSize-1; m++) {
				triangles [count] = m + n * rtSize;
				triangles [count+1] = m + (n+1) * rtSize;
				triangles [count+2] = (m+1) + (n+1) * rtSize;

				triangles [count+3] = m + n * rtSize;
				triangles [count+4] = (m+1) + (n+1) * rtSize;
				triangles [count+5] = (m+1) + n * rtSize;
				count += 6;
			}
		}
		mesh.vertices = vertices;
		//mesh.colors = colors;
		mesh.uv = uv;
		//mesh.tangents = tangents;
		mesh.triangles = triangles;
		mesh.normals = normals;
		mesh.bounds = new Bounds(Vector3.zero, new Vector3(100, 100, 100));
		if (isVis) {
			go = new GameObject ();
			go.AddComponent<MeshFilter> ().mesh = mesh;
			go.AddComponent<MeshRenderer> ().material = mat;
		}
	}
	RenderTexture CreatRenderTexture()
	{
		
		RenderTexture rt = new RenderTexture(rtSize, rtSize, 0, RenderTextureFormat.ARGBHalf);
		rt.useMipMap = false;
		rt.generateMips = false;
		rt.filterMode = FilterMode.Point;
		rt.anisoLevel = 0;
		rt.wrapMode = TextureWrapMode.Clamp;
		rt.Create();
		return rt;
	}
	void OnPreRender()
	{
		RenderTexture.active = rt;
		if (mat == null) {
			mat = new Material (Shader.Find("GPUShader"));
		}
		mat.SetTexture ("_MainTex",oldRT);
		mat.SetPass (0);
		Graphics.DrawMeshNow (mesh,Matrix4x4.identity);
		initTex.ReadPixels (new Rect(0f,0f,rt.width,rt.height),0,0);
		initTex.Apply ();
		rt = oldRT;
		oldRT = RenderTexture.active;
		RenderTexture.active = null;
	}
}
