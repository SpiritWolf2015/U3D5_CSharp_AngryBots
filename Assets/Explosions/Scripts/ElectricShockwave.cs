using UnityEngine;
using System.Collections;

public class ElectricShockwave : MonoBehaviour {

    public float autoDisableAfter = 2.0f;

    void OnEnable (){
	    DeactivateCoroutine (autoDisableAfter);
    }
    IEnumerator DeactivateCoroutine ( float t  ){
	    yield return new WaitForSeconds(t);

	    gameObject.SetActive (false);
    }

}