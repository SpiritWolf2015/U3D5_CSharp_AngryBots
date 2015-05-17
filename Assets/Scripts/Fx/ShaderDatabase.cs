using UnityEngine;
using System.Collections;

// ShaderDatabase
// knows and eventually "cooks" shaders in the beginning of the game (see CookShaders),
// also knows some tricks to hide the frame buffer with white and/or black planes
// to hide loading artefacts or shader cooking process

[RequireComponent (typeof (Camera))]
public class ShaderDatabase : MonoBehaviour {

    public Shader[] shaders;
    public bool  cookShadersOnMobiles = true;
    public Material cookShadersCover;
    private GameObject cookShadersObject;

    void Awake (){	
    #if UNITY_IPHONE || UNITY_ANDROID
	    Screen.sleepTimeout = 0.0ff;

	    if (!cookShadersOnMobiles)
		    return;
		
	    if (!cookShadersCover.HasProperty ("_TintColor"))
		    Debug.LogWarning ("Dualstick: the CookShadersCover material needs a _TintColor property to properly hide the cooking process", transform);
	
	    CreateCameraCoverPlane ();
	    cookShadersCover.SetColor ("_TintColor", Color (0.0f,0.0f,0.0f,1.0f));
    #endif
    }

    GameObject CreateCameraCoverPlane (){
	    cookShadersObject = GameObject.CreatePrimitive (PrimitiveType.Cube);
	    cookShadersObject.GetComponent<Renderer>().material = cookShadersCover;	
	    cookShadersObject.transform.parent = transform;
	
        cookShadersObject.transform.localPosition = new Vector3(cookShadersObject.transform.localPosition.x, cookShadersObject.transform.localPosition.y, cookShadersObject.transform.localPosition.z + 1.55f);

	    cookShadersObject.transform.localRotation = Quaternion.identity;	
        Quaternion quaternion = Quaternion.identity;
        quaternion.eulerAngles = new Vector3(cookShadersObject.transform.localEulerAngles.x, cookShadersObject.transform.localEulerAngles.y,cookShadersObject.transform.localEulerAngles.z+180);
        cookShadersObject.transform.localRotation=quaternion;
	    
        cookShadersObject.transform.localScale=new Vector3(cookShadersObject.transform.localScale.x*1.6f,cookShadersObject.transform.localScale.y*1.5f,cookShadersObject.transform.localScale.z*1.5f);
	
	    return cookShadersObject;		
    }

    IEnumerator WhiteOut (){
	    CreateCameraCoverPlane ();
	    Material mat  = cookShadersObject.GetComponent<Renderer>().sharedMaterial;
	    mat.SetColor ("_TintColor", new Color (1.0f, 1.0f, 1.0f, 0.0f));	
	
	    yield return 0;

        Color c = new Color(1.0f, 1.0f, 1.0f, 0.0f);
	    while (c.a < 1.0f) {
		    c.a += Time.deltaTime * 0.25f;
		    mat.SetColor ("_TintColor", c);
		    yield return 0;
	    }
			
	    DestroyCameraCoverPlane ();
    }

    IEnumerator WhiteIn (){	
	    CreateCameraCoverPlane ();
	    Material mat  = cookShadersObject.GetComponent<Renderer>().sharedMaterial;
	    mat.SetColor ("_TintColor",new Color (1.0f, 1.0f, 1.0f, 1.0f));	
	
	    yield return 0;
	
	    Color c =new Color (1.0f, 1.0f, 1.0f, 1.0f);
	    while (c.a > 0.0f) {
		    c.a -= Time.deltaTime * 0.25f;
		    mat.SetColor ("_TintColor", c);
		    yield return 0;
	    }
			
	    DestroyCameraCoverPlane ();
    }

    void DestroyCameraCoverPlane (){
	    if (cookShadersObject)
		    DestroyImmediate (cookShadersObject);	
	    cookShadersObject = null;
    }

    void Start (){	
    #if UNITY_IPHONE || UNITY_ANDROID	
	    if (cookShadersOnMobiles)
		    yield CookShaders ();	
    #endif
    }

    // this function is cooking all shaders to be used in the game. 
    // it's good practice to draw all of them in order to avoid
    // triggering in game shader compilations which might cause evil
    // frame time spikes

    // currently only enabled for mobile (iOS and Android) platforms

    IEnumerator CookShaders() {
	    if (shaders.Length>0) {
		    Material m = new Material (shaders[0]);
		    GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
	
		    cube.transform.parent = transform;
            cube.transform.localPosition = new Vector3(cube.transform.localPosition.x, cube.transform.localPosition.y, cube.transform.localPosition.z+4.0f);
			
		    yield return 0;	
		
		    foreach(Shader s in shaders) {
			    if (s) {
				    m.shader = s;
				    cube.GetComponent<Renderer>().material = m;
			    }
			    yield return 0;
		    }
					 
		    Destroy (m);
		    Destroy (cube);
		
		    yield return 0;
		    Color c = Color.black;
		    c.a = 1.0f;
		    while (c.a>0.0f) {
			    c.a -= Time.deltaTime*0.5f;
			    cookShadersCover.SetColor ("_TintColor", c);
			    yield return 0;
		    }
	    }

	    DestroyCameraCoverPlane ();
    }

}