using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracingCameraEntity : MonoBehaviour
{
    public GameObject targetObject;

    void Update()
    {
        Vector3 deltaPos = targetObject.transform.position - this.transform.position + Vector3.up;
        Vector3 position = deltaPos * 0.9f * Time.deltaTime;

        this.transform.position += new Vector3(0, position.y, position.z);
    }
}
