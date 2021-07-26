using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VertData
{
    [System.Serializable]
    public class HandData
    {
        public Vector3 origin;
        public List<Vector3> joints;
        public float distX;
        public float distY;
        public float vert;
    }
    public HandData dataL;
    public HandData dataR;

    public int frameWidth;
    public int frameHeight;

    [System.Serializable]
    public class HandMesh
    {
        public List<Vector3> verts;
        public int[] faces;

    }
    public HandMesh left_hand_data;
    public HandMesh right_hand_data;


}

