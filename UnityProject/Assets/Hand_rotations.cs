using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand_rotations
{
    public GameObject rigidHand;
    public List<Vector3> rotations;
    private string handPath = "hand test 1.fbx";

    // Start is called before the first frame update
    public Hand_rotations()
    {
        rigidHand = new GameObject("rigidHand");
        rigidHand = Resources.Load(handPath) as GameObject;
    }

    // Update is called once per frame
    public void Process(Data.HandData rodata)
    {
        //convert quaternion to euler angles
        //Quaternion[] rotationsQuat = new Quaternion[21];
        //for (int i = 0 i < 21; ++i)
        //    rotationsQuat[i] = new Quaternion(rodata.rotations[i].x, rodata.rotations[i].y, rodata.rotations[i].z, rodata.rotations[i].w);
        //rotations = rotationsQuat.eulerAngles;
        //Debug.Log(rigidHand);
        //Debug.Log(rodata.rotations[0]);

    }
}
