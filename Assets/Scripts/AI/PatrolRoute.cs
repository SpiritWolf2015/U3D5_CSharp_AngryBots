using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//2013年7月29日10:43:47，郭志程

/// <summary>
/// 巡逻路线类
/// </summary>

[RequireComponent (typeof (Collider))]
public class PatrolRoute : MonoBehaviour {

    public bool  pingPong = false;
    public List<PatrolPoint> patrolPoints = new List<PatrolPoint>();

    private List<GameObject> activePatrollers = null;      //激活巡逻者

    void Awake() {
        activePatrollers = new List<GameObject>();
    }

    public void Register(GameObject go){
	    activePatrollers.Add (go);
    }

    public void UnRegister ( GameObject go  ){
	    activePatrollers.Remove (go);
    }

    void OnTriggerEnter ( Collider other  ){
	    if (activePatrollers.Contains (other.gameObject)) {
		    AI ai = other.gameObject.GetComponentInChildren<AI> ();
		    if (ai)
			    ai.OnEnterInterestArea ();
	    }
    }

    void OnTriggerExit ( Collider other  ){
	    if (activePatrollers.Contains (other.gameObject)) {
		    AI ai = other.gameObject.GetComponentInChildren<AI> ();
		    if (ai)
			    ai.OnExitInterestArea ();
	    }
    }

    public int GetClosestPatrolPoint ( Vector3 pos  ){
	    if (patrolPoints.Count == 0)
		    return 0;
	
	    float shortestDist = Mathf.Infinity;
	    int shortestIndex = 0;
	    for (int i = 0; i < patrolPoints.Count; i++) {
		    patrolPoints[i].position = patrolPoints[i].transform.position;
		    float dist = (patrolPoints[i].position - pos).sqrMagnitude;
		    if (dist < shortestDist) {
			    shortestDist = dist;
			    shortestIndex = i;
		    }
	    }
	
	    // 如果朝最近的巡逻点走使我们走错方向，选择下一个点替代	    
	    if (!pingPong || shortestIndex < patrolPoints.Count - 1) {
		    int nextIndex = (shortestIndex + 1) % patrolPoints.Count;
		    float angle = Vector3.Angle (
			    patrolPoints[nextIndex].position - patrolPoints[shortestIndex].position,
			    patrolPoints[shortestIndex].position - pos
		    );
		    if (angle > 120)
			    shortestIndex = nextIndex;
	    }
	
	    return shortestIndex;
    }

    void OnDrawGizmos (){
	    if (patrolPoints.Count == 0)
		    return;
	
	    Gizmos.color = new Color (0.5f, 0.5f, 1.0f);
	
	    Vector3 lastPoint = patrolPoints[0].transform.position;
	    int loopCount= patrolPoints.Count;
	    if (pingPong)
		    loopCount--;
	    for (int i = 0; i < loopCount; i++) {
		    if (!patrolPoints[i])
			    break;
		    Vector3 newPoint= patrolPoints[(i + 1) % patrolPoints.Count].transform.position;
		    Gizmos.DrawLine (lastPoint, newPoint);
		    lastPoint = newPoint;
	    }
    }

    int GetIndexOfPatrolPoint ( PatrolPoint point  ) {
	    for (int i = 0; i < patrolPoints.Count; i++) {
		    if (patrolPoints[i] == point)
			    return i;
	    }
	    return -1;
    }

    GameObject InsertPatrolPointAt ( int index  ){
        GameObject go = new GameObject("PatrolPoint", typeof(PatrolPoint));
	    go.transform.parent = transform;
	    int count = patrolPoints.Count;
	
	    if (count == 0) {
		    go.transform.localPosition = Vector3.zero;
		    patrolPoints.Add(go.GetComponent<PatrolPoint>());
	    } else {
		    if (!pingPong || (index > 0 && index < count) || count < 2) {
			    index = index % count;
			    int prevIndex = index - 1;
			    if (prevIndex < 0)
				    prevIndex += count;

                go.transform.position = (patrolPoints[prevIndex].transform.position + patrolPoints[index].transform.position) * 0.5f;
		    } else if (index == 0) {
                go.transform.position = (patrolPoints[0].transform.position * 2 - patrolPoints[1].transform.position);
		    } else {
                go.transform.position = (patrolPoints[count - 1].transform.position * 2 - patrolPoints[count - 2].transform.position);
		    }
		    patrolPoints.Insert(index, go.GetComponent<PatrolPoint>());
	    }
	
	    return go;
    }

    void RemovePatrolPointAt ( int index  ) {
	    GameObject go = patrolPoints[index].gameObject;
	    patrolPoints.RemoveAt (index);
	    DestroyImmediate (go);
    }

}