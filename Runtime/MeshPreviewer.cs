using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshPreviewer : MonoBehaviour
{
    public bool disableByDefault;
    public Mesh defaultMesh;
    Mesh result;
    [Header("If PreviewMat is not setted, use Standard as default shader")]
    public bool loadDefaultShader = true;
    [SerializeField] Material previewMat;
    public Material PreviewMaterial
    {
        get
        {
            if (previewMat == null && loadDefaultShader)
            {
                previewMat = new Material(Shader.Find("Standard"));
            }
            return previewMat;
        }
        set
        {
            if (value != previewMat)
            {
                previewMat = value;
                var mr = gameObject.GetComponent<MeshRenderer>();
                if (mr) mr.sharedMaterial = previewMat;
            }
        }
    }

    public void MakePreview()
    {
        if (result == null) return;
        var mf = gameObject.GetComponent<MeshFilter>();
        if (mf == null)
        {
            mf = gameObject.AddComponent<MeshFilter>();
            mf.sharedMesh = result;
            var mr = gameObject.AddComponent<MeshRenderer>();
            mr.sharedMaterial = PreviewMaterial;
            mr.enabled = !disableByDefault;
        }
        mf.sharedMesh = result;
    }

    public void MakePreview(Mesh m)
    {
        result = m;
        MakePreview();
    }

    public void UpdateVertices(Vector3[] verts)
    {
        result.vertices = verts;
        result.RecalculateBounds();
        result.RecalculateNormals();
        result.RecalculateTangents();
        MakePreview();
    }

    public void MakePreview(Vector3[] verts, int[] triangles)
    {
        result = new Mesh();
        result.vertices = verts;
        result.triangles = triangles;
        result.RecalculateBounds();
        result.RecalculateNormals();
        result.RecalculateTangents();
        MakePreview();
    }

    public void MakePreview(Vector3[] verts, int[] triangles, Vector2[] uv)
    {
        result = new Mesh();
        result.vertices = verts;
        result.triangles = triangles;
        result.uv = uv;
        result.RecalculateBounds();
        result.RecalculateNormals();
        result.RecalculateTangents();
        MakePreview();
    }

    public void MakePreview(Vector3[] verts, int[] triangles, Color32[] colors)
    {
        result = new Mesh();
        result.vertices = verts;
        result.triangles = triangles;
        if (colors != null) result.colors32 = colors;
        result.RecalculateBounds();
        result.RecalculateNormals();
        result.RecalculateTangents();
        MakePreview();
    }

    public void MakePreview(Vector3[] verts, int[] triangles, Vector2[] uv, Color32[] colors)
    {
        result = new Mesh();
        result.vertices = verts;
        result.triangles = triangles;
        if (colors != null) result.colors32 = colors;
        result.uv = uv;
        result.RecalculateBounds();
        result.RecalculateNormals();
        result.RecalculateTangents();
        MakePreview();
    }

    [SerializeField] string inputFilePath = string.Empty;
    [ContextMenu("Read from file")]
    void ReadFromFile()
    {
        var vertices = ReadObj.Read(inputFilePath);
        var indices = ReadObj.ReadTriangles(inputFilePath);
        Debug.Log($"vertices.len: {vertices.Length}, indices.len: {indices.Length}");
        // var s = "";
        // for (int i = indices.Length - 1; i >= 0; --i)
        // {
        //     s += $"{indices[i]}, ";
        // }
        // Debug.Log(s);
        Vector2[] uv = null;
        try
        {
            uv = ReadObj.ReadUVsForUnity(inputFilePath, vertices, indices);
        }
        catch (System.Exception e) { Debug.Log($"read uv failed with error: {e}"); }
        result = new Mesh();
        result.vertices = vertices;
        result.triangles = indices;
        if (uv != null)
        {
            List<Vector2> uvs = new List<Vector2>(uv);
            // for (int i = 0; i < 10; ++i)
            // {
            // 	Debug.Log(uvs[i]);
            // }
            result.SetUVs(0, uvs);
        }
        result.RecalculateBounds();
        result.RecalculateNormals();
        result.RecalculateTangents();
        MakePreview();
    }

    [ContextMenu("Export Default Mesh")]
    void ExportDefaultMesh()
    {
        result = defaultMesh;
        WriteToFile();
    }

    [SerializeField] string outputFilePath = string.Empty;
    [ContextMenu("Write to file")]
    void WriteToFile()
    {
        if (result == null)
        {
            Debug.LogError("result is null");
            return;
        }
        if (!string.IsNullOrEmpty(outputFilePath))
        {
            WriteObj.Write(outputFilePath, result);
        }
        else
        {
            Debug.LogError("set outputFilePath first!");
        }
    }
#if UNITY_EDITOR
    public bool drawGizmos = false;
    public float drawSize = 0.005f;
    public bool drawIndex;
    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        Vector3 size = Vector3.one * drawSize;
        Gizmos.color = Color.red;
        if (result != null)
        {
            var verts = result.vertices;
            for (int i = verts.Length - 1; i >= 0; --i)
            {
                Gizmos.DrawCube(verts[i], size);
                if (drawIndex)
                {
                    UnityEditor.Handles.Label(verts[i], i.ToString());
                }
            }
        }
        Gizmos.color = Color.white;
    }
#endif
}