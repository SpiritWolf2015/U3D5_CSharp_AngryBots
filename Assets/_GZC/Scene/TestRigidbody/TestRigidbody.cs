using UnityEngine;
using System.Collections;

public class TestRigidbody : MonoBehaviour {

    private Rigidbody m_Rigidbody;
	
	void Start () {
        m_Rigidbody = GetComponent<Rigidbody>();
	}	
	
	void OnGUI () {
        if(Input.GetKey(KeyCode.Q)) {
            m_Rigidbody.angularVelocity = Vector3.up;
        }
        if (Input.GetKey(KeyCode.W))  {
            m_Rigidbody.angularVelocity = Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            m_Rigidbody.angularVelocity = -Vector3.forward;
        }
        if (Input.GetKey(KeyCode.D))  {
            m_Rigidbody.angularVelocity = Vector3.right;
        }
        if (Input.GetKey(KeyCode.A))
        {
            m_Rigidbody.angularVelocity = -Vector3.right;
        }
	}

}
