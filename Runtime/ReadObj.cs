using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ReadObj
{
	public static Vector3 ParseLineToVec(string[] line)
	{
		return new Vector3(float.Parse(line[1]), float.Parse(line[2]), float.Parse(line[3]));
	}

	public static Vector3 ParseLineToVec_InvX(string[] line)
	{
		return new Vector3(-float.Parse(line[1]), float.Parse(line[2]), float.Parse(line[3]));
	}

	public static Vector2 ParseLineToUV(string[] line)
	{
		return new Vector2(float.Parse(line[1]), float.Parse(line[2]));
	}

	public static Color ParseLineToColor(string[] line)
	{
		return new Color(float.Parse(line[4]), float.Parse(line[5]), float.Parse(line[6]), 1f);
	}

	public static Color32 ParseLineToColor32(string[] line)
	{
		return new Color32((byte)float.Parse(line[4]), (byte)float.Parse(line[5]), (byte)float.Parse(line[6]), 255);
	}

	static string RemoveNonVerticesInfo(string line)
	{
		int index = line.IndexOf('/');
		if (index != -1)
		{
			return line.Substring(0, index);
		}
		return line;
	}

	static string GetFaceInfo(string line, int type)
	{
		var each = line.Split('/');
		if (each.Length <= type)
		{
			return each[0];
		}
		return each[type];
	}

	public static void ParseLineToInd(string[] line, int type, List<int> indices)
	{
		for (int i = 3, imax = line.Length; i < imax; ++i)
		{
			indices.Add(int.Parse(GetFaceInfo(line[1], type)) - 1);
			indices.Add(int.Parse(GetFaceInfo(line[i - 1], type)) - 1);
			indices.Add(int.Parse(GetFaceInfo(line[i], type)) - 1);
		}
	}

	public static void Read(string objFilePath, List<Vector3> verts, List<int> indices, List<Vector2> uvs = null, List<Vector3> normals = null, List<Color32> colors = null)
	{
		string line;
		System.IO.StreamReader file = new System.IO.StreamReader(objFilePath);
		while ((line = file.ReadLine()) != null)
		{
			line = line.Replace("  ", " ");
			var splitted = line.Split(' ');
			if (splitted[0] == "v")
			{
				verts.Add(ParseLineToVec(splitted));
				if (colors != null && splitted.Length > 4)
				{
					colors.Add(ParseLineToColor32(splitted));
				}
			}
			else if (uvs != null && splitted[0] == "vt")
			{
				uvs.Add(ParseLineToUV(splitted));
			}
			else if (normals != null && splitted[0] == "vn")
			{
				normals.Add(ParseLineToVec(splitted));
			}
			else if (splitted[0] == "f")
			{
				ParseLineToInd(splitted, 0, indices);
			}
		}
	}

	public static Vector3[] Read(string objFilePath)
	{
		List<Vector3> verts = new List<Vector3>();
		string line;
		System.IO.StreamReader file = new System.IO.StreamReader(objFilePath);
		while ((line = file.ReadLine()) != null)
		{
			line = line.Replace("  ", " ");
			var splitted = line.Split(' ');
			if (splitted[0] == "v")
			{
				verts.Add(ParseLineToVec(splitted));
			}
		}

		file.Close();

		return verts.ToArray();
	}

	public static int[] ReadIndices(string objFilePath, int type)
	{
		List<int> indices = new List<int>();
		string line;
		System.IO.StreamReader file = new System.IO.StreamReader(objFilePath);
		while ((line = file.ReadLine()) != null)
		{
			line = line.Replace("  ", " ");
			var splitted = line.Split(' ');
			if (splitted[0] == "f")
			{
				ParseLineToInd(splitted, type, indices);
			}
		}

		file.Close();

		return indices.ToArray();
	}

	public static Vector3[] Read_InvX(string objFilePath)
	{
		List<Vector3> verts = new List<Vector3>();
		string line;
		System.IO.StreamReader file = new System.IO.StreamReader(objFilePath);
		while ((line = file.ReadLine()) != null)
		{
			line = line.Replace("  ", " ");
			var splitted = line.Split(' ');
			if (splitted[0] == "v")
			{
				verts.Add(ParseLineToVec_InvX(splitted));
			}
		}
		file.Close();

		return verts.ToArray();
	}

	public static Color32[] ReadColors(string objFilePath)
	{
		List<Color32> colors = new List<Color32>();
		string line;
		System.IO.StreamReader file = new System.IO.StreamReader(objFilePath);
		while ((line = file.ReadLine()) != null)
		{
			line = line.Replace("  ", " ");
			var splitted = line.Split(' ');
			if (splitted[0] == "v" && splitted.Length > 4)
			{
				colors.Add(ParseLineToColor(splitted));
			}
		}
		file.Close();

		return colors.ToArray();
	}

	public static Color32[] ReadColor32s(string objFilePath)
	{
		List<Color32> colors = new List<Color32>();
		string line;
		System.IO.StreamReader file = new System.IO.StreamReader(objFilePath);
		while ((line = file.ReadLine()) != null)
		{
			line = line.Replace("  ", " ");
			var splitted = line.Split(' ');
			if (splitted[0] == "v" && splitted.Length > 4)
			{
				colors.Add(ParseLineToColor32(splitted));
			}
		}
		file.Close();

		return colors.ToArray();
	}

	public static Vector2[] ReadUVs(string objFilePath)
	{
		List<Vector2> uvs = new List<Vector2>();
		string line;
		System.IO.StreamReader file = new System.IO.StreamReader(objFilePath);

		while ((line = file.ReadLine()) != null)
		{
			line = line.Replace("  ", " ");
			var splitted = line.Split(' ');
			if (splitted[0] == "vt")
			{
				uvs.Add(ParseLineToUV(splitted));
			}
		}
		file.Close();

		return uvs.ToArray();
	}

	public static Vector2[] ReadUVsForUnity(string objFilePath)
	{
		var vertices = Read(objFilePath);
		var uv = ReadUVs(objFilePath);
		var uvmap = ReadTriangles(objFilePath, 1);
		var indices = ReadTriangles(objFilePath);
		var uvs = new Vector2[vertices.Length];
		for (int i = 0; i < indices.Length; ++i)
		{
			var ind = indices[i];
			uvs[ind] = uv[uvmap[i]];
		}
		return uvs;
	}

	public static Vector2[] ReadUVsForUnity(string objFilePath, Vector3[] vertices, int[] indices = null)
	{
		var uv = ReadUVs(objFilePath);
		var uvmap = ReadTriangles(objFilePath, 1);
		if (indices == null)
			indices = ReadTriangles(objFilePath);
		var uvs = new Vector2[vertices.Length];
		for (int i = 0; i < indices.Length; ++i)
		{
			var ind = indices[i];
			uvs[ind] = uv[uvmap[i]];
		}
		return uvs;
	}

	public static Vector3[] ReadNormals(string objFilePath)
	{
		List<Vector3> normals = new List<Vector3>();
		string line;
		System.IO.StreamReader file = new System.IO.StreamReader(objFilePath);
		while ((line = file.ReadLine()) != null)
		{
			line = line.Replace("  ", " ");
			var splitted = line.Split(' ');
			if (splitted[0] == "vn")
			{
				normals.Add(ParseLineToVec(splitted));
			}
		}
		file.Close();

		return normals.ToArray();
	}

	public static int[] ReadTriangles(string objFilePath, int type = 0)
	{
		List<int> indices = new List<int>();
		string line;
		System.IO.StreamReader file = new System.IO.StreamReader(objFilePath);
		while ((line = file.ReadLine()) != null)
		{
			line = line.Replace("  ", " ");
			var splitted = line.Split(' ');
			if (splitted[0] == "f")
			{
				ParseLineToInd(splitted, type, indices);
			}
		}
		file.Close();
		return indices.ToArray();
	}

	public static void Save(Vector3[] vertices, string objFilePath)
	{
		using (System.IO.StreamWriter file = new System.IO.StreamWriter(objFilePath))
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for (int i = 0, max = vertices.Length; i < max; ++i)
			{
				var p = vertices[i];
				sb.AppendFormat("v {0} {1} {2}\n", p.x, p.y, p.z);
			}
			file.Write(sb.ToString());
		}
	}
}