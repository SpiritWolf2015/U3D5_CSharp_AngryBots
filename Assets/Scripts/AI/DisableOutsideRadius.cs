using UnityEngine;
using System.Collections;

[RequireComponent (typeof (SphereCollider))]

public class DisableOutsideRadius : MonoBehaviour {

    private GameObject target;
    private SphereCollider sphereCollider;
    private float activeRadius;

    void Awake (){
	    target = transform.parent.gameObject;
	    sphereCollider = GetComponent<SphereCollider> ();
	    activeRadius = sphereCollider.radius;

	    Disable ();
    }

    void Disable() {
        transform.parent = target.transform.parent;
        target.transform.parent = transform;
        target.SetActive(false);
        sphereCollider.radius = activeRadius;
    }

    void OnTriggerEnter ( Collider other  ){
	    if (other.tag == "Player" && target.transform.parent == transform) {
		    Enable ();
	    }
    }

    void OnTriggerExit ( Collider other  ){
	    if (other.tag == "Player") {
		    Disable ();
	    }
    }

    void Enable (){
	    target.transform.parent = transform.parent;
	    target.SetActive (true);
	    transform.parent = target.transform;
	    sphereCollider.radius = activeRadius * 1.1f;
    }

}