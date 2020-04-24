using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshFill : MonoBehaviour
{
    public Material[] mat;
    Mesh mesh;
    public GameObject meshPrefab;

    public void CreateShape(Vector2 dotA, Vector2 dotB, int matIndex, float offset)
    {

        Vector3[] dotsToVerts = new Vector3[]
{
            new Vector3(dotA.x,0,offset),
            new Vector3(dotA.x,dotA.y,offset),
            new Vector3(dotB.x,0,offset),
            new Vector3(dotB.x,dotB.y,offset)
};

        GameObject meshObject = Instantiate(meshPrefab, transform);
        mesh = new Mesh();
        meshObject.GetComponent<MeshFilter>().mesh = mesh;
        meshObject.GetComponent<Renderer>().material = mat[matIndex];

        mesh.Clear();

        mesh.vertices = dotsToVerts;

        mesh.triangles = new int[]
        {
            0,1,2,1,3,2
        };

    }

    public void DestroyChildren()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
