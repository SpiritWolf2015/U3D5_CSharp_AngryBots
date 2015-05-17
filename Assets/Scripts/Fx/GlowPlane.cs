using UnityEngine;
using System.Collections;

//2013年7月27日17:46:42，郭志程

public class GlowPlane : MonoBehaviour {

    Transform playerTransform;
    private Vector3 pos;
    private Vector3 scale;
    float minGlow = 0.2f;
    float maxGlow = 0.5f;
    Color glowColor = Color.white;

    private Material mat;

    void Start (){
	    if (!playerTransform)
		    playerTransform = GameObject.FindWithTag ("Player").transform;	
	    pos = transform.position;
	    scale = transform.localScale;
	    mat = GetComponent<Renderer>().material;
	    enabled = false;
    }

    void OnDrawGizmos (){	    
        Gizmos.color = new Color(glowColor.r, glowColor.g, glowColor.b, maxGlow * 0.25f);
	    Gizmos.matrix = transform.localToWorldMatrix;
        Vector3 scale = 5.0f * Vector3.Scale(Vector3.one, new Vector3(1, 0, 1));     // Vector3.one表示的是(1,1,1)
	    Gizmos.DrawCube (Vector3.zero, scale);
	    Gizmos.matrix = Matrix4x4.identity;
    }

    void OnDrawGizmosSelected (){	    
        Gizmos.color = new Color(glowColor.r, glowColor.g, glowColor.b, maxGlow);
	    Gizmos.matrix = transform.localToWorldMatrix;
	    Vector3 scale = 5.0f * Vector3.Scale (Vector3.one, new Vector3(1,0,1));
	    Gizmos.DrawCube (Vector3.zero, scale);
	    Gizmos.matrix = Matrix4x4.identity;
    }

    void OnBecameVisible (){
	    enabled = true;	
    }

    void OnBecameInvisible (){
	    enabled = false;
    }

    void Update (){
	    Vector3 vec = (pos - playerTransform.position);
	    vec.y = 0.0f;
	    float distance= vec.magnitude;	
	    transform.localScale = Vector3.Lerp (Vector3.one * minGlow, scale, Mathf.Clamp01 (distance * 0.35f));
	    mat.SetColor ("_TintColor",  glowColor * Mathf.Clamp (distance * 0.1f, minGlow, maxGlow));	
    }

}