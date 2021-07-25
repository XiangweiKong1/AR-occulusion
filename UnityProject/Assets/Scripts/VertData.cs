using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VertData
{
    [System.Serializable]
    public class HandMesh
    {
        public List<Vector3> verts;
        public int[] faces;

    }
    
   
    public HandMesh left_hand_data;
    public int frameWidth;
    public int frameHeight;

    public class HandData
    {
        public Vector3 origin;
        public List<Vector3> joints;
        public float distX;
        public float distY;
        public float vert;
    }
    public HandData dataL;
}

