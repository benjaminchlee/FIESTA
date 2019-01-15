using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util;

public class ConvexMesh : MonoBehaviour {

    [SerializeField] [Tooltip("The color of the lasso range while incomplete.")]
    private Color incompleteColor = new Color(125, 125, 125, 25);
    [SerializeField] [Tooltip("The color of the lasso range when complete.")]
    private Color completeColor = new Color(255, 255, 0, 25);

    private GameObject screen;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        screen = GameObject.FindGameObjectWithTag("Screen");
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// Creates a convex mesh on the screen
    /// </summary>
    /// <param name="vertices">An array of vertices in local space</param>
    /// <param name="uv">An array of 2-dimensional vertices in local space</param>
    public void CreateConvexMesh(Vector3[] vertices)
    {
        Vector2[] uv = vertices.Select(p => (Vector2)p).ToArray();

        Triangulator t = new Triangulator(uv);

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = t.Triangulate();
    }

    public void SetIncomplete()
    {
        meshRenderer.material.SetColor("_TintColor", incompleteColor);
    }

    public void SetComplete()
    {
        meshRenderer.material.SetColor("_TintColor", completeColor);
    }

    public bool IsFacingUser()
    {
        Vector3[] vertices = meshFilter.mesh.vertices;
        int[] triangles = meshFilter.mesh.triangles;
        int count = triangles.Length;

        Vector3 a = vertices[triangles[count - 1]];
        Vector3 b = vertices[triangles[count - 2]];
        Vector3 c = vertices[triangles[count - 3]];

        Vector3 x = b - a;
        Vector3 y = c - a;
        Vector3 perpen = Vector3.Cross(x, y);
        Vector3 norm = Vector3.Normalize(perpen);
        Vector3 local = screen.transform.InverseTransformDirection(norm);

        if (local.z == 1)
            return false;
        else
            return true;
    }

    /// <summary>
    /// Destroys the convex mesh
    /// </summary>
    public void DestroyConvexMesh()
    {
        meshFilter.mesh = null;
    }
}
