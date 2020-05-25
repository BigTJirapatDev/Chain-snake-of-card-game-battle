using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Target;
    // Start is called before the first frame update
    void Start()
    {
        Target = GameObject.Find("Avatar").transform; //find avatar to camera follow avater
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(Target.position.x, Target.position.y, -1);
    }
}
