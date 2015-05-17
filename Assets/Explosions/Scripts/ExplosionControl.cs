using UnityEngine;
using System.Collections;

public class ExplosionControl : MonoBehaviour {

    public GameObject[] trails;
    public ParticleEmitter emitter;
    public LineRenderer[] lineRenderer;
    public GameObject lightDecal;

    public float autoDisableAfter = 2.0f;

    void Awake (){
	    for (int i = 0; i < lineRenderer.Length; i++) {
		    float lineWidth = Random.Range(0.25f,0.5f);

		    lineRenderer[i].SetWidth (lineWidth, lineWidth);
		    lineRenderer[i].SetPosition (0, Vector3.zero);

		    Vector3 dir = Random.onUnitSphere;
		    dir.y = Mathf.Abs (dir.y);

		    lineRenderer[i].SetPosition (1, dir * Random.Range (8.0f, 12.0f));
	    }
    }

    void OnEnable (){
	    lightDecal.transform.localScale = Vector3.one;

	    lightDecal.SetActive (true);

	    for (int i = 0; i < trails.Length; i++) {
		    trails[i].transform.localPosition = Vector3.zero;
		    trails[i].SetActive (true);
		    (trails[i].GetComponent<ExplosionTrail>() as ExplosionTrail).enabled = true;
	    }

	    for(int i = 0; i < lineRenderer.Length; i++) {
		    lineRenderer[i].transform.localPosition = Vector3.zero;
		    lineRenderer[i].gameObject.SetActive (true);
		    lineRenderer[i].enabled = true;
	    }

	    emitter.emit = true;
	    emitter.enabled = true;
	    emitter.gameObject.SetActive (true);

	    Invoke("DisableEmitter", emitter.maxEnergy);
	    Invoke("DisableStuff", autoDisableAfter);
    }

    void DisableEmitter (){
	    emitter.emit = false;
	    emitter.enabled = false;
    }

    void DisableStuff (){
	    gameObject.SetActive(false);
    }

}