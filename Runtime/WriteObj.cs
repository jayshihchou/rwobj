using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WriteObj
{
	static void WriteVertices(System.Text.StringBuilder sb, Vector3[] verts, Color32[] colors = null)
	{
		if (colors == null || verts.Length != colors.Length)
		{
			for (int i = 0, max = verts.Length; i < max; ++i)
			{
				sb.AppendFormat("v {0} {1} {2}\n", verts[i].x, verts[i].y, verts[i].z);
			}
		}
		else
		{
			for (int i = 0, max = verts.Length; i < max; ++i)
			{
				// convert to float
				Color c = colors[i];
				sb.AppendFormat("v {0} {1} {2} {3} {4} {5}\n", verts[i].x, verts[i].y, verts[i].z, c.r, c.g, c.b);
			}
		}
	}

	static void WriteTangent(System.Text.StringBuilder sb, Vector4[] tangent)
	{
		for (int i = 0, max = tangent.Length; i < max; ++i)
		{
			sb.AppendFormat("ta {0} {1} {2} {3}\n", tangent[i].x, tangent[i].y, tangent[i].z, tangent[i].w);
		}
	}

	static void WriteNormal(System.Text.StringBuilder sb, Vector3[] normal)
	{
		for (int i = 0, max = normal.Length; i < max; ++i)
		{
			sb.AppendFormat("vn {0} {1} {2}\n", normal[i].x, normal[i].y, normal[i].z);
		}
	}

	static void WriteUV(System.Text.StringBuilder sb, Vector2[] uv)
	{
		for (int i = 0, max = uv.Length; i < max; ++i)
		{
			sb.AppendFormat("vt {0} {1}\n", uv[i].x, uv[i].y);
		}
	}

	public enum FaceType
	{
		None = 0, Normal = 2, UV = 4
	}

	static bool Contains(int flags, int flag)
	{
		return (flags & flag) != 0;
	}

	static int Add(int flags, int flag)
	{
		return (flags | flag);
	}

	static int Remove(int flags, int flag)
	{
		return (flags & (~flag));
	}

	static void WriteTriangles(System.Text.StringBuilder sb, int[] triangle, int type)
	{
		string formatter;
		if (Contains(type, (int)FaceType.Normal) && Contains(type, (int)FaceType.UV))
		{
			formatter = "f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n";
		}
		else if (Contains(type, (int)FaceType.Normal))
		{
			formatter = "f {0}//{0} {1}//{1} {2}//{2}\n";
		}
		else if (Contains(type, (int)FaceType.UV))
		{
			formatter = "f {0}/{0}/ {1}/{1}/ {2}/{2}/\n";
		}
		else
		{
			formatter = "f {0} {1} {2}\n";
		}

		for (int i = 0, max = triangle.Length; i < max; i += 3)
		{
			sb.AppendFormat(formatter, triangle[i] + 1, triangle[i + 1] + 1, triangle[i + 2] + 1);
		}
	}

	static void WriteTriangles(System.Text.StringBuilder sb, int[] triangle, int[] uvTri, int[] normTri)
	{
        if (uvTri == null || uvTri.Length == 0)
        {
            uvTri = triangle;
        }
		if (normTri == null || normTri.Length == 0)
        {
            normTri = triangle;
        }

        string formatter = "f {0}/{1}/{2} {3}/{4}/{5} {6}/{7}/{8}\n";

		for (int i = 0, max = triangle.Length; i < max; i += 3)
		{
			sb.AppendFormat(
				formatter,
				triangle[i] + 1, uvTri[i] + 1, normTri[i] + 1, 
				triangle[i + 1] + 1, uvTri[i + 1] + 1, normTri[i + 1] + 1,
				triangle[i + 2] + 1, uvTri[i + 2] + 1, normTri[i + 2] + 1
			);
		}
	}

	public static void Write(string file, Vector3[] verts)
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		WriteVertices(sb, verts);
		System.IO.File.WriteAllText(file, sb.ToString());
	}

	public static void Write(string file, Vector3[] verts, int[] triangles)
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		WriteVertices(sb, verts);
		int triType = 0;
		WriteTriangles(sb, triangles, triType);
		System.IO.File.WriteAllText(file, sb.ToString());
	}

	public static void Write(string file, Vector3[] verts, Color32[] colors)
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		WriteVertices(sb, verts, colors);
		System.IO.File.WriteAllText(file, sb.ToString());
	}

	public static void Write(string file, Vector3[] verts, Color32[] colors, int[] triangles)
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		WriteVertices(sb, verts, colors);
		int triType = 0;
		WriteTriangles(sb, triangles, triType);
		System.IO.File.WriteAllText(file, sb.ToString());
	}

	public static void Write(string file, Vector3[] verts, Vector2[] uvs, int[] triangles)
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		WriteVertices(sb, verts);
		int triType = 1;
		triType = Add(triType, (int)FaceType.UV);
		WriteUV(sb, uvs);
		WriteTriangles(sb, triangles, triType);

		System.IO.File.WriteAllText(file, sb.ToString());
	}

	public static void Write(string file, Vector3[] verts, Vector2[] uvs, Vector3[] normals, int[] triangles)
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		WriteVertices(sb, verts);
		int triType = 1;
		triType = Add(triType, (int)FaceType.UV);
		WriteUV(sb, uvs);
		triType = Add(triType, (int)FaceType.Normal);
		WriteNormal(sb, normals);

		WriteTriangles(sb, triangles, triType);

		System.IO.File.WriteAllText(file, sb.ToString());
	}

	public static void Write(string file, Vector3[] verts, Vector3[] normals, int[] triangles, Color32[] colors)
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		WriteVertices(sb, verts, colors);
		int triType = 1;
		triType = Add(triType, (int)FaceType.Normal);
		WriteNormal(sb, normals);

		WriteTriangles(sb, triangles, triType);

		System.IO.File.WriteAllText(file, sb.ToString());
	}

	public static void Write(string file, Vector3[] verts, Vector2[] uvs, Vector3[] normals, int[] triangles, Color32[] colors)
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		WriteVertices(sb, verts, colors);
		int triType = 1;
		triType = Add(triType, (int)FaceType.UV);
		WriteUV(sb, uvs);
		if (normals != null && normals.Length > 0)
		{
			triType = Add(triType, (int)FaceType.Normal);
			WriteNormal(sb, normals);
		}

		WriteTriangles(sb, triangles, triType);

		System.IO.File.WriteAllText(file, sb.ToString());
	}

	public static void Write(string file, Dictionary<string, object> mesh)
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		Vector3[] verts = null;
		Color32[] colors = null;
		if (mesh.ContainsKey("verts"))
	        verts = (Vector3[])mesh["verts"];
		if (mesh.ContainsKey("colors"))
			colors = (Color32[])mesh["colors"];
        WriteVertices(sb, verts, colors);

        if (mesh.ContainsKey("uvs"))
        {
			var uvs = (Vector2[])mesh["uvs"];
        	WriteUV(sb, uvs);
        }

		if (mesh.ContainsKey("normal"))
		{
            var normals = (Vector3[])mesh["normal"];
            WriteNormal(sb, normals);
		}

        int[] triangles = null;
        if (mesh.ContainsKey("faces"))
        {
            triangles = (int[])mesh["faces"];
        }

        if (triangles != null && triangles.Length > 0)
        {
            int[] uvTris = null;
            if (mesh.ContainsKey("uv_faces"))
            {
				uvTris = (int[])mesh["uv_faces"];
            }
            int[] normalTris = null;
            if (mesh.ContainsKey("norm_faces"))
            {
				normalTris = (int[])mesh["norm_faces"];
            }
            WriteTriangles(sb, triangles, uvTris, normalTris);
        }

        System.IO.File.WriteAllText(file, sb.ToString());
	}

	public static void Write(string file, Mesh mesh, bool withTangent = false)
	{
		var verts = mesh.vertices;
		var triangles = mesh.triangles;
		var uv = mesh.uv;
		var normals = mesh.normals;
		var colors = mesh.colors32;
		if (colors != null && colors.Length == 0)
			colors = null;
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		WriteVertices(sb, verts, colors);

		int triType = 1;
		if (uv != null && uv.Length > 0)
		{
			triType = Add(triType, (int)FaceType.UV);
			WriteUV(sb, uv);
		}
		if (normals != null && normals.Length > 0)
		{
			triType = Add(triType, (int)FaceType.Normal);
			WriteNormal(sb, normals);
		}

		if (withTangent)
		{
			var tangent = mesh.tangents;
			WriteTangent(sb, tangent);
		}

		WriteTriangles(sb, triangles, triType);

		System.IO.File.WriteAllText(file, sb.ToString());
	}

	public class BlendShapeData
	{
		public float[] deltaVerts;
		public float[] deltaNorms;
		public BlendShapeData(int vc, Vector3[] dv, Vector3[] dn)
		{
			deltaVerts = new float[vc * 3];
			if (dn != null)
				deltaNorms = new float[vc * 3];
			for (int i = 0; i < vc; ++i)
			{
				for (int j = 0; j < 3; ++j)
				{
					deltaVerts[i * 3 + j] = dv[i][j];
				}
				if (dn != null)
				{
					for (int j = 0; j < 3; ++j)
					{
						deltaNorms[i * 3 + j] = dn[i][j];
					}
				}
			}
		}
	}
#if UNITY_EDITOR
	[UnityEditor.MenuItem("Custom/Export Obj/Export Selected (to world space)")]
	static void WriteSelectedObject()
	{
		WriteObject(false, true, false, false);
	}

	[UnityEditor.MenuItem("Custom/Export SkinnedMeshRenderer/Export Selected (with blend shape) (to world space)")]
	static void WriteSelectedObject2()
	{
		WriteObject(true, true, false, false);
	}

	[UnityEditor.MenuItem("Custom/Export Obj/Export Selected")]
	static void WriteSelectedObject3()
	{
		WriteObject(false, false, true, false);
	}
	[UnityEditor.MenuItem("Custom/Export SkinnedMeshRenderer/Export Selected (center to rootBone)")]
	static void WriteSelectedObject3_1()
	{
		WriteObject(false, false, true, true);
	}

	[UnityEditor.MenuItem("Custom/Export SkinnedMeshRenderer/Export Selected (with blend shape)")]
	static void WriteSelectedObject4()
	{
		WriteObject(true, false, false, false);
	}

	static void WriteObject(bool bakeSMR, bool doTrans, bool withTangent, bool smrToRootBone)
	{
		var go = UnityEditor.Selection.activeGameObject;
		if (go)
		{
			var trans = go.transform;
			Transform root = null;
			Mesh mesh = null;
			var mf = go.GetComponent<MeshFilter>();

			if (mf)
			{
				mesh = Object.Instantiate(mf.sharedMesh);
			}
			else
			{
				var smr = go.GetComponent<SkinnedMeshRenderer>();
				if (smr)
				{
					if (bakeSMR)
					{
						mesh = new Mesh();
						smr.BakeMesh(mesh);
					}
					else
						mesh = Object.Instantiate(smr.sharedMesh);
					if (smrToRootBone && smr.rootBone)
						root = smr.rootBone;
				}
			}

			if (mesh != null && trans != null)
			{
				if (doTrans)
				{
					var verts = mesh.vertices;
					for (int i = verts.Length - 1; i >= 0; --i)
					{
						verts[i] = trans.TransformPoint(verts[i]);
					}
					mesh.vertices = verts;
				}
				if (smrToRootBone && root)
				{
					var verts = mesh.vertices;
					for (int i = verts.Length - 1; i >= 0; --i)
					{
						verts[i] = root.InverseTransformPoint(trans.TransformPoint(verts[i]));
					}
					mesh.vertices = verts;
					mesh.RecalculateNormals();
				}
				var m = Object.Instantiate(mesh);
				if (withTangent)
					m.RecalculateTangents();
				if (m.normals.Length == 0)
					m.RecalculateNormals();
				Write("./obj.obj", m, withTangent);
				Debug.Log("Done export to ./obj.obj");
			}
		}
	}

	[UnityEditor.MenuItem("Custom/Export Obj/Export BlendShape")]
	static void WriteBlendShape()
	{
		var go = UnityEditor.Selection.activeGameObject;
		if (go)
		{
			var trans = go.transform;
			var smr = go.GetComponent<SkinnedMeshRenderer>();
			if (smr)
			{
				var mesh = smr.sharedMesh;

				int vc = mesh.vertexCount;
				Vector3[] dv = new Vector3[vc];
				Vector3[] dn = new Vector3[vc];
				Vector3[] dt = new Vector3[vc];
				BlendShapeData[] datas = new BlendShapeData[mesh.blendShapeCount];
				for (int i = 0; i < mesh.blendShapeCount; ++i)
				{
					mesh.GetBlendShapeFrameVertices(i, 0, dv, dn, dt);
					datas[i] = new BlendShapeData(vc, dv, dn);
				}
				using (var br = new System.IO.BinaryWriter(System.IO.File.Open("bs.bin", System.IO.FileMode.Create)))
				{
					br.Write(mesh.blendShapeCount);
					br.Write(vc);
					//Debug.Log("ovc:" + vc);
					for (int i = 0, max = mesh.blendShapeCount; i < max; ++i)
					{
						var dverts = datas[i].deltaVerts;
						//Debug.Log(dverts.Length);
						for (int j = 0; j < vc * 3; ++j)
						{
							br.Write(dverts[j]);
						}
					}
				}

				Debug.Log("Done");
			}
		}
	}
#endif
}