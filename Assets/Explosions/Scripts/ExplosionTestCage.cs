using UnityEngine;
using System.Collections;

public class ExplosionTestCage : MonoBehaviour {

    public GameObject explPrefab;

    void OnTriggerEnter ( Collider other  ){
	    if(other.GetComponent<Collider>().tag == "Player") {
            GameObject go = Instantiate(explPrefab, transform.position, transform.rotation) as GameObject;	
	    }	
    }

}