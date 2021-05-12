using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerControl : MonoBehaviour
{
    const float MOVE_SPEED = 2f;
    const float ROTATE_SPEED = 0.5f;
    const float ATTACK_DISTANCE = 0.5f;
    GameObject m_DetectedObject;

    [SerializeField] GameObject m_JointBody;
    ConfigurableJoint m_JointForObject;

    [SerializeField] LineRenderer m_Cable;

    [SerializeField] GameObject m_Hook;
    [SerializeField] GameObject m_Jib;
    [SerializeField] GameObject m_Towermast;
    [SerializeField] GameObject m_Trolley;

    void UpdateCable()
    {
        m_Cable.enabled = m_JointForObject != null;
        if (m_Cable.enabled)
        {
            m_Cable.SetPosition(0, m_Hook.transform.position - new Vector3(0, 0.3f, 0));

            var connectedBodyTransform = m_JointForObject.connectedBody.transform;
            m_Cable.SetPosition(1, connectedBodyTransform.TransformPoint(m_JointForObject.connectedAnchor));
        }
    }

    void DetectObjects()
    {
        Ray ray = new Ray (m_Hook.transform.position, Vector3.down);
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

    void Update()
    {
        #region Key Control

        if (Input.GetKey(KeyCode.W))
        {
            m_Jib.transform.Translate(0, 0, MOVE_SPEED * Time.deltaTime);
            m_Trolley.transform.Translate(0, 0, MOVE_SPEED * Time.deltaTime);
            m_Hook.transform.Translate(0, 0, MOVE_SPEED * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            m_Jib.transform.Translate(0, 0, -MOVE_SPEED * Time.deltaTime);
            m_Trolley.transform.Translate(0, 0, -MOVE_SPEED * Time.deltaTime);
            m_Hook.transform.Translate(0, 0, -MOVE_SPEED * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            m_Trolley.transform.Translate(0, MOVE_SPEED * Time.deltaTime, 0);
            m_Hook.transform.Translate(0, MOVE_SPEED * Time.deltaTime, 0);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            m_Trolley.transform.Translate(0, -MOVE_SPEED * Time.deltaTime, 0);
            m_Hook.transform.Translate(0, -MOVE_SPEED * Time.deltaTime, 0);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            m_Hook.transform.Translate(0, 0, MOVE_SPEED * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            m_Hook.transform.Translate(0, 0, -MOVE_SPEED * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            m_Jib.transform.Rotate(0f, 0f, ROTATE_SPEED);
            //m_Trolley.transform.Rotate(0f, 0f, ROTATE_SPEED);
            //m_Hook.transform.Rotate(0f, 0f, ROTATE_SPEED);
            m_Trolley.transform.RotateAround(Vector3.zero, Vector3.up, ROTATE_SPEED);
            m_Hook.transform.RotateAround(Vector3.zero, Vector3.up, ROTATE_SPEED);
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            m_Jib.transform.Rotate(0f, 0f, -ROTATE_SPEED);
            //m_Trolley.transform.Rotate(0f, 0f, -ROTATE_SPEED);
            //m_Hook.transform.Rotate(0f, 0f, -ROTATE_SPEED);
            m_Trolley.transform.RotateAround(Vector3.zero, Vector3.up, -ROTATE_SPEED);
            m_Hook.transform.RotateAround(Vector3.zero, Vector3.up, -ROTATE_SPEED);
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
