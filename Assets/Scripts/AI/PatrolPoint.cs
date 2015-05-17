using UnityEngine;
using System.Collections;

public class PatrolPoint : MonoBehaviour {

    public Vector3 position;

    void Awake () {
	    position = this.transform.position;
    }

}