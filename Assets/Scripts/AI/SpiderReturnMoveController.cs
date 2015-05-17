using UnityEngine;
using System.Collections;

public class SpiderReturnMoveController : MonoBehaviour {

    // Public member data
    public MovementMotor motor;

    // Private memeber data
    private AI ai;

    private Transform character;
    private Vector3 spawnPos;
    public MonoBehaviour animationBehaviour;

    void Awake (){
	    character = motor.transform;
	    ai = transform.parent.GetComponentInChildren<AI> ();
	    spawnPos = character.position;
    }

    void Update (){
	    motor.movementDirection = spawnPos - character.position;
	    motor.movementDirection.y = 0;
	    if (motor.movementDirection.sqrMagnitude > 1)
		    motor.movementDirection = motor.movementDirection.normalized;
	
	    if (motor.movementDirection.sqrMagnitude < 0.01f) {
		    character.position = new Vector3 (spawnPos.x, character.position.y, spawnPos.z);
		    motor.GetComponent<Rigidbody>().velocity = Vector3.zero;
		    motor.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		    motor.movementDirection = Vector3.zero;
		    this.enabled = false;
		    animationBehaviour.enabled = false;
	    }
    }

}