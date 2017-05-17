// Stylized Water Shader by Staggart Creations http://u3d.as/A2R
// Online documentation can be found at http://staggart.xyz

using UnityEngine;
using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class StylizedWater : MonoBehaviour
{

    #region Shader variables
    [Header("Color")]
    public Color waterColor;
    public Color fresnelColor;
    public Color rimColor;
    public float waveTint;

    [Header("Surface")]
    public float transparency;
    public Texture reflectionCubemap;
    public float glossiness;

    public float fresnel;
    public bool useIntersectionHighlight;
    public float surfaceHighlight;
    public float surfaceHighlightTiling;
    public float surfaceHighlightSize;
    public bool surfaceHighlightPanning;

    public float depth;
    public float depthDarkness;

    public float rimSize;
    public float rimFalloff;
    public float rimDistance;

    public bool worldSpaceTiling;
    public float tiling;
    public float rimTiling;

    public float refractionAmount;

    public float waveSpeed;
    public float waveStrength;

    public Texture customIntersection;
    public Texture customNormal;
    public Texture normals;
    public Texture shadermap;

    public float tessellation;
    #endregion

    #region  Substance variables
    [Header("Substance")]

#if UNITY_EDITOR
    public ProceduralMaterial substance;
#endif

    public float seed;
    public float normalStrength;

    public string[] intersectionStyleList = new string[] { "None", "Windwaker", "Foamy", "Foamy2", "Triangular", "Cells", "Perlin" };
    public int intersectionStyle;

    public string[] waveStyleList = new string[] { "Perlin", "Plasma", "Circular", "Stream", "Sharp", "Cells", "Cloudy" };
    public int waveStyle;
    public float waveSize;

    public string[] waveHeightmapList = new string[] { "Smooth", "Sharp", "Billowed" };
    public int waveHeightmap;

    public bool useCustomIntersection;
    public bool useCustomNormals;
    public bool useIntersectionAsHighlight;
    #endregion

    //Script vars
    public Material material;
    public bool isMobileAdvanced = false;
    public bool isMobileBasic = false;
    public string shaderName = null;

    //Toggle bools
    [Header("Toggles")]
    public bool showColors = true;
    public bool showSurface = true;
    public bool showIntersection;
    public bool showHighlights;
    public bool showDepth;
    public bool showWaves;

    public bool hasSubstanceParams;

    public void getProperties()
    {
        //Debug.Log("StylizedWater.cs: getProperties()");

        material = GetComponent<MeshRenderer>().sharedMaterial; //Requires typeof
        shaderName = material.shader.name;

#if UNITY_EDITOR
        if (!substance)
        {
            getSubstance(material);

        }

        getSubstanceProperties();
#endif
        getShaderProperties();
    }

#if UNITY_EDITOR
    private void getSubstanceProperties()
    {
        if (!substance)
        {
            return;
        }
        //Debug.Log("StylizedWater getSubstanceProperties()");

        seed = substance.GetProceduralFloat("$randomseed");

        normalStrength = substance.GetProceduralFloat("normalStrength");
        intersectionStyle = substance.GetProceduralEnum("intersectionStyle");
        waveStyle = substance.GetProceduralEnum("waveStyle");
        waveSize = substance.GetProceduralFloat("waveSize");
        waveHeightmap = substance.GetProceduralEnum("waveHeightmap");

        useCustomIntersection = substance.GetProceduralBoolean("useCustomIntersection");
        useCustomNormals = substance.GetProceduralBoolean("useCustomNormal");
        useIntersectionAsHighlight = substance.GetProceduralBoolean("useIntersectionAsHighlight");

        if (!substance.isProcessing) substance.RebuildTexturesImmediately(); //Force output generation to avoid blank results

        Texture[] proceduralTextures = substance.GetGeneratedTextures();
        //Debug.Log("GET " + substance.name + " map [0]: " + proceduralTextures[0]);
        shadermap = proceduralTextures[0];
        normals = proceduralTextures[1];

        //Debug.Log("Getting value " + intersectionStyleList[intersectionStyle] + " from " + substance.name);

        hasSubstanceParams = true;

    }
#endif

    private void getShaderProperties()
    {
        if (!material) return;

        isMobileBasic = (shaderName.Contains("Mobile Basic")) ? true : false;
        isMobileAdvanced = (shaderName.Contains("Mobile Advanced")) ? true : false;

        //Not basic, get these parameters
        if (!isMobileBasic)
        {
            fresnelColor = material.GetColor("_FresnelColor");
            fresnel = material.GetFloat("_Fresnelexponent");


            surfaceHighlight = material.GetFloat("_SurfaceHighlight");
            surfaceHighlightTiling = material.GetFloat("_SurfaceHightlighttiling");
            surfaceHighlightSize = material.GetFloat("_Surfacehightlightsize");

            depth = material.GetFloat("_Depth");
            depthDarkness = material.GetFloat("_Depthdarkness");

            rimDistance = material.GetFloat("_RimDistance");
        }

        //Excluded from both mobile versions
        if (!isMobileAdvanced && !isMobileBasic)
        {
            useIntersectionHighlight = (material.GetFloat("_UseIntersectionHighlight") == 1) ? true : false;
            surfaceHighlightPanning = (material.GetFloat("_HighlightPanning") == 1) ? true : false;
            refractionAmount = material.GetFloat("_RefractionAmount");
            waveStrength = material.GetFloat("_Wavesstrength");
            reflectionCubemap = material.GetTexture("_Reflection");
            waveTint = material.GetFloat("_Wavetint");
        }

        transparency = material.GetFloat("_Transparency");

        //Shared by all
        worldSpaceTiling = (material.GetFloat("_Worldspacetiling") == 1) ? true : false;

        waterColor = material.GetColor("_WaterColor");
        rimColor = material.GetColor("_RimColor");

        glossiness = material.GetFloat("_Glossiness");

        rimSize = material.GetFloat("_RimSize");
        rimFalloff = material.GetFloat("_Rimfalloff");

        tiling = material.GetFloat("_Tiling");
        rimTiling = material.GetFloat("_Rimtiling");

        waveSpeed = material.GetFloat("_Wavesspeed");

        //Tesselation shader only
        if (shaderName == "StylizedWater/Desktop (DX11 Tessellation)")
        {
            tessellation = material.GetFloat("_Tessellation");
        }
    }

#if UNITY_EDITOR
    private void getSubstance(Material mat)
    {
        if (mat == null)
        {
            Debug.LogError("StylizedWater: No material assigned to MeshRenderer component!");
            return;
        }

        if (mat.GetTexture("_Shadermap"))
        {
            string[] assets;
            string substanceName = mat.GetTexture("_Shadermap").name.Replace("_shadermap", ""); //eg StylizedWater_frozen_shadermap to StylizedWater_frozen

            assets = AssetDatabase.FindAssets("t:ProceduralMaterial " + substanceName);
            string assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);

            SubstanceImporter si = AssetImporter.GetAtPath(assetPath) as SubstanceImporter; //Substance .sbsar container
            ProceduralMaterial[] substanceContainer = si.GetMaterials();

            //Look for the substance instance matching the material name we're looking for
            foreach (ProceduralMaterial substanceInstance in substanceContainer)
            {
                if (substanceInstance.name == substanceName)
                {
                    substance = substanceInstance; //Gotcha
                }
            }

            //Debug.Log("Found substance using " + material.name + ". Result: " + substance.name);
        }

        else
        {
            Debug.LogError("StylizedWater: Shadermap is not assigned to the current material, cannot locate associated Substance without it.");
            return;
        }

    }
#endif
}//class end
//Easter egg, good job :)
