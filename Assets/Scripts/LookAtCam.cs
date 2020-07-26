using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCam : MonoBehaviour
{   
   
    // Update is called once per frame
    void Update()
    {
       

      //  transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

        Vector3 targetPosition  = new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z);
        transform.LookAt(targetPosition);
    }
}
