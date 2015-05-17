using UnityEngine;
using System.Collections;

//2013年7月29日10:38:12，郭志程

/// <summary>
///  Mech大机器人的移动业务逻辑
/// </summary>

[RequireComponent(typeof(Rigidbody))]
public class MechMovementMotor : MovementMotor {
	
	public float walkingSpeed = 3.0f;
	public float turningSpeed = 100.0f;
    public float aimingSpeed = 150.0f;      //瞄准
	
	public Transform head;
	
	//private Vector3 wallNormal = Vector3.zero;
	private Vector3 wallHit;
	private bool  facingInRightDirection = false;
	private Quaternion headRotation = Quaternion.identity;

    void Awake() {
        wallHit = Vector3.zero; facingInRightDirection = false; headRotation = Quaternion.identity;
    }

    // 1.大机器怪施加旋转力，使怪朝向玩家。2.施加力使大机器人往前移动。所以业务逻辑写在FixedUpdate中，并且这个脚本需要Rigidbody组件
	void FixedUpdate () {
		Vector3 adjustedMovementDirection = movementDirection;

        /*
        // If the movement direction points into a wall as defined by the wall normal,
        // then change the movement direction to be perpendicular to the wall,
        // so the character "glides" along the wall.
        if (Vector3.Dot (movementDirection, wallNormal) < 0) {
            // Keep the vector length prior to adjustment
            float vectorLength = movementDirection.magnitude;
            // Project the movement vector onto the plane defined by the wall normal
            adjustedMovementDirection =
                movementDirection - Vector3.Project (movementDirection, wallNormal) * 0.9f;
            // Apply the original length of the vector
            adjustedMovementDirection = adjustedMovementDirection.normalized * vectorLength;
        }         
		
		Debug.DrawRay(transform.position, adjustedMovementDirection, Color.yellow);
		Debug.DrawRay(transform.position, movementDirection, Color.green);
		Debug.DrawRay(transform.position, wallNormal, Color.red);*/

        // 角色旋转至目标朝向
		float rotationAngle;
		if (adjustedMovementDirection != Vector3.zero)
			rotationAngle = AngleAroundAxis (transform.forward, adjustedMovementDirection, Vector3.up) * 0.3f;
		else
			rotationAngle = 0;
		Vector3 targetAngularVelocity = Vector3.up * Mathf.Clamp (rotationAngle, -turningSpeed * Mathf.Deg2Rad, turningSpeed * Mathf.Deg2Rad);
		GetComponent<Rigidbody>().angularVelocity = Vector3.MoveTowards (GetComponent<Rigidbody>().angularVelocity, targetAngularVelocity, Time.deltaTime * turningSpeed * Mathf.Deg2Rad * 3);      //施加旋转的力
		
		/*
		if ((transform.position - wallHit).magnitude > 2) {
			wallNormal = Vector3.zero;
		}*/
		
		float angle = Vector3.Angle (transform.forward, adjustedMovementDirection);
		if (facingInRightDirection && angle > 25)
			facingInRightDirection = false;
		if (!facingInRightDirection && angle < 5)
			facingInRightDirection = true;
		
		// 操纵角色移动
		Vector3 targetVelocity;
		if (facingInRightDirection)
			targetVelocity = transform.forward * walkingSpeed + GetComponent<Rigidbody>().velocity.y * Vector3.up;
		else
			targetVelocity = GetComponent<Rigidbody>().velocity.y * Vector3.up;

        GetComponent<Rigidbody>().velocity = Vector3.MoveTowards(GetComponent<Rigidbody>().velocity, targetVelocity, Time.deltaTime * walkingSpeed * 3);       // Vector3.MoveTowards : u3d api, 移向,当前的地点移向目标。
		//transform.position += targetVelocity * Time.deltaTime * walkingSpeed * 3;
	}
	
	void LateUpdate (){
		// 头看的目标
		if (facingDirection != Vector3.zero) {
			Quaternion targetRotation = Quaternion.LookRotation (facingDirection);
			headRotation = Quaternion.RotateTowards (
				headRotation,
				targetRotation,
				aimingSpeed * Time.deltaTime
			);
			head.rotation = headRotation * Quaternion.Inverse (transform.rotation) * head.rotation;
		}
	}
	
	/*
	void OnCollisionStay ( Collision collisionInfo  ){
		if (collisionInfo.gameObject.tag == "Player")
			return;
		
		// Record the first wall normal
		foreach(ContactPoint contact in collisionInfo.contacts) {
			// Discard normals that are not mostly horizontal
			if (Mathf.Abs(contact.normal.y) < 0.7f) {
				wallNormal = contact.normal;
				wallNormal.y = 0;
				wallHit = transform.position;
				break;
			}
		}
		
		// Only keep the horizontal components
		wallNormal.y = 0;
	}
	*/
	
	// 方向A与方向B绕哪个轴间的角度值
	public static float AngleAroundAxis ( Vector3 dirA ,   Vector3 dirB ,   Vector3 axis  ){
	    // Project A and B onto the plane orthogonal target axis
	    dirA = dirA - Vector3.Project (dirA, axis);
	    dirB = dirB - Vector3.Project (dirB, axis);
	   
	    // Find (positive) angle between A and B
	    float angle = Vector3.Angle (dirA, dirB);
	   
	    // Return angle multiplied with 1 or -1
	    return angle * (Vector3.Dot (axis, Vector3.Cross (dirA, dirB)) < 0 ? -1 : 1);
	}
	
}