using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RigidHand : MonoBehaviour
{
    
    public GameObject rigidHand;
    public Vector3[] rotations;
    public GameObject wrist;
    public GameObject thumb1, thumb2, thumb3, thumb4;
    public GameObject index1, index2, index3, index4;
    public GameObject middle1, middle2, middle3, middle4;
    public GameObject ring1, ring2, ring3, ring4;
    public GameObject pinky1, pinky2, pinky3, pinky4;
    public Quaternion[] quatRotations;
    public Vector3[] adjRotations;
    public float temp;
    public Vector3 wristOffset;

    // Start is called before the first frame update
    void Start()
    {
        rotations = new Vector3[21];
        adjRotations = new Vector3[21];
        quatRotations = new Quaternion[21];
        rigidHand = this.gameObject;
        wrist = rigidHand.transform.GetChild(0).GetChild(0).gameObject;

        thumb1 = wrist.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).gameObject;
        thumb2 = thumb1.transform.GetChild(0).gameObject;
        thumb3 = thumb2.transform.GetChild(0).gameObject;
        thumb4 = thumb3.transform.GetChild(0).gameObject;

        index1 = wrist.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        index2 = index1.transform.GetChild(0).gameObject;
        index3 = index2.transform.GetChild(0).gameObject;
        index4 = index3.transform.GetChild(0).gameObject;

        middle1 = wrist.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
        middle2 = middle1.transform.GetChild(0).gameObject;
        middle3 = middle2.transform.GetChild(0).gameObject;
        middle4 = middle3.transform.GetChild(0).gameObject;

        ring1 = wrist.transform.GetChild(2).GetChild(0).GetChild(0).gameObject;
        ring2 = ring1.transform.GetChild(0).gameObject;
        ring3 = ring2.transform.GetChild(0).gameObject;
        ring4 = ring3.transform.GetChild(0).gameObject;

        pinky1 = wrist.transform.GetChild(3).GetChild(0).GetChild(0).gameObject;
        pinky2 = pinky1.transform.GetChild(0).gameObject;
        pinky3 = pinky2.transform.GetChild(0).gameObject;
        pinky4 = pinky3.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    public void Process(Data.HandData data)
    {
        for (int i = 0; i < 21; i++)
        {
            temp = data.rotations[i].x * data.rotations[i].x + data.rotations[i].y * data.rotations[i].y + data.rotations[i].z * data.rotations[i].z + data.rotations[i].w * data.rotations[i].w;
            if (temp != 1)
            {
                data.rotations[i] = data.rotations[i] / Mathf.Sqrt(temp);
            }
            quatRotations[i] = new Quaternion(data.rotations[i].x, data.rotations[i].y, data.rotations[i].z, data.rotations[i].w);
            rotations[i] = quatRotations[i].eulerAngles;
        }

        float scaleX = data.vert * data.distX + (1 - data.vert) * data.distY;
        float scaleY = data.vert * data.distY + (1 - data.vert) * data.distX;
        float dist = (scaleX + scaleY) / 2;
        var target = new Vector3(
            (+data.joints[0].x / scaleX + data.origin.x) * (Client.frustumWidth / 2) * (1),
            (-data.joints[0].y / scaleY + (data.origin.y - 0.5f)) * Client.frustumHeight,
            1f * data.joints[0].z + 1f * dist);
        var actualTarget = Vector3.Lerp(wrist.transform.position, target,
                Vector3.Distance(wrist.transform.position, target) * 5f);

        wrist.transform.position = actualTarget;

        for (int i = 0; i < 21; i++)
        {
            adjRotations[i] = new Vector3(-rotations[i].x, rotations[i].y, -rotations[i].z);
        }

        wrist.transform.rotation =  Quaternion.Euler(adjRotations[0]);
        thumb1.transform.rotation = Quaternion.Euler(adjRotations[1]);
        thumb2.transform.rotation = Quaternion.Euler(adjRotations[2]);
        thumb3.transform.rotation = Quaternion.Euler(adjRotations[3]);
        thumb4.transform.rotation = Quaternion.Euler(adjRotations[4]);
        index1.transform.rotation = Quaternion.Euler(adjRotations[5]);
        index2.transform.rotation = Quaternion.Euler(adjRotations[6]);
        index3.transform.rotation = Quaternion.Euler(adjRotations[7]);
        index4.transform.rotation = Quaternion.Euler(adjRotations[8]);
        middle1.transform.rotation = Quaternion.Euler(adjRotations[9]);
        middle2.transform.rotation = Quaternion.Euler(adjRotations[10]);
        middle3.transform.rotation = Quaternion.Euler(adjRotations[11]);
        middle4.transform.rotation = Quaternion.Euler(adjRotations[12]);
        ring1.transform.rotation = Quaternion.Euler(adjRotations[13]);
        ring2.transform.rotation = Quaternion.Euler(adjRotations[14]);
        ring3.transform.rotation = Quaternion.Euler(adjRotations[15]);
        ring4.transform.rotation = Quaternion.Euler(adjRotations[16]);
        pinky1.transform.rotation = Quaternion.Euler(adjRotations[17]);
        pinky2.transform.rotation = Quaternion.Euler(adjRotations[18]);
        pinky3.transform.rotation = Quaternion.Euler(adjRotations[19]);
        pinky4.transform.rotation = Quaternion.Euler(adjRotations[20]);

        wrist.transform.rotation *= Quaternion.Euler(wristOffset);

        //Debug.Log(rigidHand.transform.GetChild(0).GetChild(0));
        //Debug.Log(data.dataL.rotations[20]);
        //Debug.Log(rotations[1]);
    }
}
