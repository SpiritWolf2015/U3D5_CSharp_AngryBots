using UnityEngine;
using System.Collections;

public class SetCheckpoint : MonoBehaviour {

    public Transform spawnTransform;

    void OnTriggerEnter ( Collider other  ){
	    SpawnAtCheckpoint checkpointKeeper = other.GetComponent<SpawnAtCheckpoint> () as SpawnAtCheckpoint;
	    checkpointKeeper.checkpoint = spawnTransform;
    }

}