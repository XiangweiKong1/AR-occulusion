using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VertRigidHand : MonoBehaviour
{
    
    private bool facesSet = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    public void Process(VertData.HandMesh data)
    {
        this.GetComponent<MeshFilter>().mesh.SetVertices(data.verts);
        if (!facesSet) // Only need to set the faces once.
        {
            this.GetComponent<MeshFilter>().mesh.SetTriangles(data.faces, 0);
            facesSet = true;
        }
    }
}
