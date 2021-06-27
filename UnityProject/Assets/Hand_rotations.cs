using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand_rotations
{
    public GameObject rigidHand;
    private string handPath = "hand test 1";

    // Start is called before the first frame update
    public Hand_rotations()
    {
        rigidHand = new GameObject("rigidHand");
        rigidHand = Resources.Load(handPath) as GameObject;
    }

    // Update is called once per frame
    public void Process(Data.HandData rodata)
    {
        Debug.Log(rigidHand);
        Debug.Log(rodata.rotations[0]);

    }
}
