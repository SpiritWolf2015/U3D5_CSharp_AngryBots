using UnityEngine;
using System.Collections;

public class CopyRotation : MonoBehaviour {

    public Transform sourceRotation;
    public Vector3 addLocalRotation;

    void LateUpdate () {
	    transform.rotation = sourceRotation.rotation;
	    transform.localRotation = transform.localRotation * Quaternion.Euler(addLocalRotation);
    }

}