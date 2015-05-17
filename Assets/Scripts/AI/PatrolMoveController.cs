using UnityEngine;
using System.Collections;

// 2013年7月29日10:49:50，郭志程
// 绑定在大机器人下AIPatrol子物体的脚本，模拟AI巡逻
// 简单的理解，将所有的巡逻点放到arrayList中，然后机器人每次都找到最近的巡逻点，然后走到了之后再寻找下一个最近的巡逻点，再往那走。以此模拟一个巡逻的效果

/// <summary>
/// AI巡逻移动控制器
/// </summary>

public class PatrolMoveController : MonoBehaviour {

        #region 公共变量
            public MovementMotor motor;

            public PatrolRoute patrolRoute;             //巡逻路线物体
            public float patrolPointRadius = 0.5f;
        #endregion

        #region 私有变量
            private Transform character;
            private int nextPatrolPoint = 0;
            private int patrolDirection = 1; 
        #endregion

    void Start () {
        character = motor.transform;        // 得到机器人跟物体的transform
	    patrolRoute.Register (transform.parent.gameObject);         //  注册，将机器人根物体加入到激活的巡逻者列表中
        nextPatrolPoint = 0; patrolDirection = 1;
    }

    void OnEnable () {
	    nextPatrolPoint = patrolRoute.GetClosestPatrolPoint (transform.position);       //得到最近的巡逻点
    }

    void OnDestroy () {
	    patrolRoute.UnRegister (transform.parent.gameObject);
    }

    void Update () {
	    // 如果没有巡逻点则结束Update函数
	    if (patrolRoute == null || patrolRoute.patrolPoints.Count == 0)
		    return;
	
	    // 找到去下一个巡逻点的方向向量
	    Vector3 targetVector = patrolRoute.patrolPoints[nextPatrolPoint].position - character.position;
	    targetVector.y = 0;
	
	    // 如果达到巡逻点，选择下一个
	    if (targetVector.sqrMagnitude < patrolPointRadius * patrolPointRadius) {
		    nextPatrolPoint += patrolDirection;
		    if (nextPatrolPoint < 0) {
			    nextPatrolPoint = 1;
			    patrolDirection = 1;
		    }
		    if (nextPatrolPoint >= patrolRoute.patrolPoints.Count) {
			    if (patrolRoute.pingPong) {
				    patrolDirection = -1;
				    nextPatrolPoint = patrolRoute.patrolPoints.Count - 2;
			    }
			    else {
				    nextPatrolPoint = 0;
			    }
		    }
	    }
	
	    // 保证目标向量长度不超过1
	    if (targetVector.sqrMagnitude > 1)
		    targetVector.Normalize ();
	
	    // 设置移动的方向
	    motor.movementDirection = targetVector;
	    // 设置面朝移动的方向
	    motor.facingDirection = targetVector;
    }

}