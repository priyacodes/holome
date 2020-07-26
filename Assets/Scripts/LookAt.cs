using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    public float offset = 0f;


    void Start()
    {

    }

    void Update()
    {
        var lookPos = mainCamera.transform.position - transform.position;
        lookPos.y = 0;
        lookPos = Quaternion.AngleAxis(offset, Vector3.up) * lookPos;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 50f);
    }
}
