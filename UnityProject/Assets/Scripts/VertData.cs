using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VertData
{
    [System.Serializable]
    public class HandData 
    {
        public List<Vector3> verts;
        public int[] faces;

    };


    public HandData left_hand_data;
    public int frameWidth;
    public int frameHeight;
}