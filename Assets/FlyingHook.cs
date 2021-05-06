using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingHook : MonoBehaviour
{
    const float MOVE_SPEED = 2f;
    const float ATTACK_DISTANCE = 0.5f;
    GameObject m_DetectedObject;

    [SerializeField] GameObject m_JointBody;
    [SerializeField] ConfigurableJoint m_JointForObject;

    [SerializeField] LineRenderer m_Cable;

    void UpdateCable()
    {
        m_Cable.enabled = m_JointForObject.connectedBody != null && m_JointForObject.connectedBody != null;
        
        if (m_Cable.enabled)
        {
            m_Cable.SetPosition(0, this.transform.position);

            var connectedBodyTransform = m_JointForObject.connectedBody.transform;
            m_Cable.SetPosition(1, connectedBodyTransform.TransformPoint(m_JointForObject.connectedAnchor));
        }
    }

    void DetectObjects()
    {
        Ray ray = new Ray (this.transform.position, Vector3.down);
        RaycastHit hit; // A RaycastHit to store the raycast result

        if (Physics.Raycast(ray, out hit, ATTACK_DISTANCE))
        {
            if (m_DetectedObject == hit.collider.gameObject)
            {
                return;
            }

            RecoverDetectedObject();

            MeshRenderer renderer = hit.collider.GetComponent <MeshRenderer> ();

            if (renderer != null)
            {
                renderer.material.color = Color.yellow;
                m_DetectedObject = hit.collider.gameObject;
            }
        }
        else
        {
            RecoverDetectedObject();
        }
    }

    void RecoverDetectedObject()
    {
        if (m_DetectedObject != null)
        {
            m_DetectedObject.GetComponent <MeshRenderer> ().material.color = Color.white;
            m_DetectedObject = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        #region Key Control

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.Translate(0, 0, MOVE_SPEED * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.Translate(0, 0, -MOVE_SPEED * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            this.transform.Translate(MOVE_SPEED * Time.deltaTime, 0, 0);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            this.transform.Translate(-MOVE_SPEED * Time.deltaTime, 0, 0);
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            this.transform.Translate(0, MOVE_SPEED * Time.deltaTime, 0);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            this.transform.Translate(0, -MOVE_SPEED * Time.deltaTime, 0);
        }

        #endregion

        if (m_JointForObject == null)
        {
            DetectObjects();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AttachOrDetachObject();
        }

        UpdateCable();
    }

    void AttachOrDetachObject()
    {
        if (m_JointForObject == null)
        {
            if (m_DetectedObject != null)
            {
                var joint = m_JointBody.AddComponent <ConfigurableJoint> ();
                joint.xMotion = ConfigurableJointMotion.Limited;
                joint.yMotion = ConfigurableJointMotion.Limited;
                joint.zMotion = ConfigurableJointMotion.Limited;
                joint.angularXMotion = ConfigurableJointMotion.Free;
                joint.angularYMotion = ConfigurableJointMotion.Free;
                joint.angularZMotion = ConfigurableJointMotion.Free;

                var limit = joint.linearLimit;
                limit.limit = 0.4f;

                joint.linearLimit = limit;

                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = new Vector3 (0f, 0.5f, 0f);
                joint.anchor = new Vector3 (0f, 0f, 0f);

                joint.connectedBody = m_DetectedObject.GetComponent<Rigidbody> ();

                m_JointForObject = joint;

                m_DetectedObject.GetComponent <MeshRenderer> ().material.color = Color.red;
                m_DetectedObject = null;
            }
        }
        else
        {
            GameObject.Destroy(m_JointForObject);
            m_JointForObject = null;
        }
    }
}
