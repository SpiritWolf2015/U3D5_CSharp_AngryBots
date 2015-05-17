using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent (typeof (Camera))]
[AddComponentMenu("Image Effects/Noise")]
public class ColoredNoise : MonoBehaviour {

    public float globalNoiseAmount = 0.075f;
    public float globalNoiseAmountOnDamage = -6.0f;

    public float localNoiseAmount = 0.0f;
    private Shader noiseShader;
    public Texture2D noiseTexture;
    private Material noise = null;

    void Start (){
	    FindShaders ();
	    CheckSupport ();
	    CreateMaterials ();	
    }

    void FindShaders (){	
	    if (!noiseShader)
		    noiseShader = Shader.Find("Hidden/ColoredNoise");
    }

    void CreateMaterials (){		
	    if (!noise) {
		    noise = new Material (noiseShader);	
		    noise.hideFlags = HideFlags.DontSave;
	    }           
    }

    bool Supported (){
	     return (SystemInfo.supportsImageEffects && SystemInfo.supportsRenderTextures && noiseShader.isSupported);
    }

    bool CheckSupport (){
	     if (!Supported ()) {
		    enabled = false;
		    return false;
	    }	
	    return true;
    }

    void OnDisable (){
	    if (noise) {
		    DestroyImmediate (noise);
		    noise = null;	
	    }
    }

    void OnRenderImage ( RenderTexture source ,   RenderTexture destination  ){		
    #if UNITY_EDITOR
	    FindShaders ();
	    CheckSupport ();
	    CreateMaterials ();	
    #endif
						
	    noise.SetFloat ("_NoiseAmount", globalNoiseAmount + localNoiseAmount * Mathf.Sign (globalNoiseAmount));
	    noise.SetTexture ("_NoiseTex", noiseTexture);	

	    DrawNoiseQuadGrid (source, destination, noise, noiseTexture, 0);
    }

    // Helper to draw a screenspace grid of quads with random texture coordinates

    static void DrawNoiseQuadGrid ( RenderTexture source ,   RenderTexture dest ,   Material fxMaterial ,   Texture2D noise ,   int passNr  ){
	    RenderTexture.active = dest;
	
	    #if UNITY_XBOX360
	        float tileSize = 128.0ff;
	    #else
	        float tileSize = 64.0f;
	    #endif
	
	    float subDs = (1.0f * source.width) / tileSize;
       
	    fxMaterial.SetTexture ("_MainTex", source);	        
                
	    GL.PushMatrix ();
	    GL.LoadOrtho ();	
		
	    float aspectCorrection = (1.0f * source.width) / (1.0f * source.height);
	    float stepSizeX = 1.0f / subDs;
	    float stepSizeY = stepSizeX * aspectCorrection; 
   	    float texTile = tileSize / (noise.width * 1.0f);
    	    	
	    fxMaterial.SetPass (passNr);	
	
        GL.Begin (GL.QUADS);
    
   	    for (float x1 = 0.0f; x1 < 1.0f; x1 += stepSizeX) {
   		    for (float y1 = 0.0f; y1 < 1.0f; y1 += stepSizeY) { 
   			
   			    float tcXStart = Random.Range (-1.0f, 1.0f);
   			    float tcYStart = Random.Range (-1.0f, 1.0f);
   			    float texTileMod = Mathf.Sign (Random.Range (-1.0f, 1.0f));
						
		        GL.MultiTexCoord2 (0, tcXStart, tcYStart); 
		        GL.Vertex3 (x1, y1, 0.1f);
		        GL.MultiTexCoord2 (0, tcXStart + texTile * texTileMod, tcYStart); 
		        GL.Vertex3 (x1 + stepSizeX, y1, 0.1f);
		        GL.MultiTexCoord2 (0, tcXStart + texTile * texTileMod, tcYStart + texTile * texTileMod); 
		        GL.Vertex3 (x1 + stepSizeX, y1 + stepSizeY, 0.1f);
		        GL.MultiTexCoord2 (0, tcXStart, tcYStart + texTile * texTileMod); 
		        GL.Vertex3 (x1, y1 + stepSizeY, 0.1f);
   		    }
   	    }
    	
	    GL.End ();
        GL.PopMatrix ();
    }

}