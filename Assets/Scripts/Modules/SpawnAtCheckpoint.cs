using UnityEngine;
using System.Collections;

public class SpawnAtCheckpoint : MonoBehaviour {

    public Transform checkpoint;

    void OnSignal (){
	    transform.position = checkpoint.position;
	    transform.rotation = checkpoint.rotation;
	
	    ResetHealthOnAll ();
    }

    static void ResetHealthOnAll (){
        Health[] healthObjects = FindObjectsOfType(typeof(Health)) as Health[];
	    foreach(Health health in healthObjects) {
		    health.dead = false;
		    health.health = health.maxHealth;
	    }
    }

}