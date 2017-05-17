using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
	GameObject go;
	// Use this for initialization
	void Start () {
		rt = CreatRenderTexture ();
		oldRT = CreatRenderTexture ();

		initTex = new Texture2D(rtSize, rtSize, TextureFormat.RGBAHalf, false);
		initTex.filterMode = FilterMode.Point;
		initTex.anisoLevel = 0;
		Debug.Log (initTex.mipmapCount);
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
				SetNormal (m,n,new Vector3(i,3f,33f));
			}
		}
		mesh.normals = normals;
		if(isVis)
		go.GetComponent<MeshFilter> ().mesh = mesh;
	}
	public void SetNormal(int x,int y,Vector3 v)
	{
		int index = x + y * rtSize;
		index *= 4;
		normals [index] = normals [index + 1] = normals [index + 2] = normals [index + 3] = v;
	}
	public void CreatMesh()
	{
		mesh = new Mesh();
		float dealt = 2f / rtSize;
		float d = 1f / (rtSize - 1);
		vertices = new Vector3[rtSize*rtSize*4];
		triangles = new int[rtSize*rtSize*6];
		normals = new Vector3[vertices.Length];
		int vCount = 0;
		int tCount = 0;
		for (int n = 0; n < rtSize; n++) {
			for (int m = 0; m < rtSize; m++) {
				vertices [vCount] = new Vector3 (-1f + m * dealt, -1f + n * dealt, 1f);
				vertices [vCount+1] = new Vector3 (-1f + m * dealt, -1f + (n+1) * dealt, 1f);
				vertices [vCount+2] = new Vector3 (-1f + (m+1) * dealt, -1f + (n+1) * dealt, 1f);
				vertices [vCount+3] = new Vector3 (-1f + (m+1) * dealt, -1f + n * dealt, 1f);

				triangles [tCount] = vCount;
				triangles [tCount+1] = vCount+1;
				triangles [tCount+2] = vCount+3;
				triangles [tCount+3] = vCount+3;
				triangles [tCount+4] = vCount+1;
				triangles [tCount+5] = vCount+2;
				vCount += 4;
				tCount += 6;

				//normals.Add (new Vector3(n*d,n*d,1f));//0 0.5
				//uv.Add (new Vector2(m*d,n*d));
			}
		}
		mesh.vertices = vertices;
		//mesh.colors = colors;
		//mesh.uv = uv;
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

		for (int n = 0; n < rtSize; n++) {
			for (int m = 0; m < rtSize; m++) {
				int i = Random.Range (0,100);
				SetNormal (m,n,new Vector3(i,3f,33f));
			}
		}
		mesh.normals = normals;



		RenderTexture.active = rt;
		if (mat == null) {
			mat = new Material (Shader.Find("GPUShader"));
		}
		mat.SetPass (0);
		Graphics.DrawMeshNow (mesh,Matrix4x4.identity);
		initTex.ReadPixels (new Rect(0f,0f,rt.width,rt.height),0,0);
		initTex.Apply ();
		rt = oldRT;
		oldRT = RenderTexture.active;
		RenderTexture.active = null;
	}
}
