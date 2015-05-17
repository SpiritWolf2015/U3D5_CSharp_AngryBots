using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyArea : MonoBehaviour {

    public List<GameObject> affected = new List<GameObject> ();

    void Start() {
        StartCoroutine(ActivateAffected(false));
    }

    void OnTriggerEnter ( Collider other  ) {
	    if (other.tag == "Player")
            StartCoroutine(ActivateAffected(true));
    }

    void OnTriggerExit ( Collider other  ) {
	    if (other.tag == "Player")
            StartCoroutine(ActivateAffected(false));
    }

    IEnumerator ActivateAffected ( bool state  ) {
	     foreach(GameObject go in affected) {
		    if (go == null)
			    continue;
		    go.SetActive (state);
		    yield return 0;
	    }
	    foreach(Transform tr in transform) {
		    tr.gameObject.SetActive (state);
		    yield return 0;
	    }
    }

}