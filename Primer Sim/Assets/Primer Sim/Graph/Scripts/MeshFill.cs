using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshFill : MonoBehaviour
{
    Mesh mesh;
    public GameObject meshPrefab;

    public void CreateShape(Vector2 dotA, Vector2 dotB)
    {

        Vector3[] dotsToVerts = new Vector3[]
{
            new Vector3(dotA.x,0,0),
            new Vector3(dotA.x,dotA.y,0),
            new Vector3(dotB.x,0,0),
            new Vector3(dotB.x,dotB.y,0)
};

        GameObject meshObject = Instantiate(meshPrefab, transform);
        mesh = new Mesh();
        meshObject.GetComponent<MeshFilter>().mesh = mesh;

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
