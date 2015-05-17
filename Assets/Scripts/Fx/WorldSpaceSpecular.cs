using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class WorldSpaceSpecular : MonoBehaviour {

    public Transform casterA; 
    public Color colorA = Color.white;
    public Transform casterB; 
    public Color colorB = Color.white;
    public Transform casterC; 
    public Color colorC = Color.white;

    void Update (){
	    if (casterA)
		    Shader.SetGlobalVector ("SPEC_LIGHT_DIR_0", casterA.forward);
	    if (casterB)
		    Shader.SetGlobalVector ("SPEC_LIGHT_DIR_1", casterB.forward);
	    if (casterC)
		    Shader.SetGlobalVector ("SPEC_LIGHT_DIR_2", casterC.forward);
	
	    Shader.SetGlobalVector ("SPEC_LIGHT_COLOR_0", colorA);
	    Shader.SetGlobalVector ("SPEC_LIGHT_COLOR_1", colorB);
	    Shader.SetGlobalVector ("SPEC_LIGHT_COLOR_2", colorC);
    }

}