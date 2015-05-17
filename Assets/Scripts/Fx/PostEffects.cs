using UnityEngine;
using System.Collections;

public class PostEffects {
			
	public static Material CheckShaderAndCreateMaterial ( Shader s ,   Material m2Create  ){
		if (m2Create && m2Create.shader == s) 
			return m2Create;
			
		if (!s) { 
			Debug.LogWarning("PostEffects: missing shader for " + m2Create.ToString ());
			return null;
		}
		
		if(!s.isSupported) {
			Debug.LogWarning ("The shader " + s.ToString () + " is not supported");
			return null;
		}
		else {
			m2Create = new Material (s);	
			m2Create.hideFlags = HideFlags.DontSave;		
			return m2Create;
		}
	}
		
	public static bool CheckSupport ( bool needDepth  ){		
		 if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures) {
			Debug.Log ("Disabling image effect as this platform doesn't support any");
			return false;
		}	
		
		if(needDepth && !SystemInfo.SupportsRenderTextureFormat (RenderTextureFormat.Depth)) {
			Debug.Log ("Disabling image effect as depth textures are not supported on this platform.");
			return false;
		}
		
		return true;
	}

}