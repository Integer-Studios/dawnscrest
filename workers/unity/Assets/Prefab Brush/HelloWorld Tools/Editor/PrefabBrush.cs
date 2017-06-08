
/*
 * 		Prefab Brush+ 
 * 		Version 1.2.4
 *		Author: Archie Andrews
 *		www.HelloWorldStudios.co.uk
 *
 *		Feel free to edit/change anything in this script for your own personal use. <3
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[ExecuteInEditMode]  
public class PrefabBrush : EditorWindow {
	
	//Toggle bools
	private bool placmentBrush = false;
	private bool removalBrush = false;
	private bool aboutTab = true;
	private bool settingsTab = false;
	
	//Foldout bools
	private bool showBrushSettings = true;
	private bool showObjectSettings = true;
	private bool showBrushSaves = true;
	
	//Placment Brush vars
	private int objectCount = 0;
	private List<GameObject> selectedObject = new List<GameObject>();
	private float brushSize = 50;
	private float maxBrushSize = 250;
	private float opacity = 100;
	private LayerMask layers;
	private string tag = "Untagged";
    private GameObject missingObject;

    //Bools for enabling each setting.
    private bool rotateToSurface = false;
	private bool randomScale = false;
	private bool randomRotation = false;
	private bool setParentToBrushBase = false;
	private bool setParentToObject = false;
	private bool checkLayer = true;
	private bool checkTag = false;
	private bool checkSlope = false;
	private bool checkTagRemove = false;
	private bool checkLayerRemove = false;
	
	private Vector3 offsetPrefabsOrigin = Vector3.zero;
    private Vector3 offsetPrefabRotation = Vector3.zero;
	private Vector3 rotToSurfaceVector = new Vector3(0,1,0);
	
	private GameObject parent;
	
	private int removeObjectSize = 1;
	private int addObjectSize = 1;
	
	private float minXRotation = 0;
	private float maxXRotation = 360;
	private float minYRotation = 0;
	private float maxYRotation = 360;
	private float minZRotation = 0;
	private float maxZRotation = 360;
	
	private float maxSlope = 90;
	private float minSlope = 0;
	
	private float rotationSet = 0;
	
	private float minScale = 1;
	private float maxScale = 10;
	
	private float opacityValue;
	
	//Removal brush vars
	private LayerMask removeLayerToBrushOn;
	private string removeTagToBrushOn = "Untagged";
	
	private float removeBrushSize = 50;
	private float removeMaxBrushSize = 100;
	
	private Vector2 scrollPos;
	private float angle = 0;
	private int inc = 0;
	private int maxRemoveUndo = 2;
	
	//Brush save variables
	private string saveName = "Brush";
	private List<bool> checkDelete = new List<bool> ();
	private List<string> saveList = new List<string> ();
	private int savesCount = 0;

	//Settings variables
	private Color placeBrush = new Color(0,1,0,0.1f);
	private Color removeBrush = new Color(1,0,0,0.1f);
    private Color backGround = Color.white;
    private Color backGroundTwo = Color.white;
    private Color selectedTab = Color.green;
	private Color deleteButton = Color.red;
    private Color textColor = Color.black;

	//Defualt settings
	private Color placeBrushDefault = new Color(0,1,0,0.1f);
	private Color removeBrushDefault = new Color(1,0,0,0.1f);
    private Color backGroundDefault = Color.white;
    private Color backGroundTwoDefault = Color.white;
    private Color selectedTabDefault = Color.green;
	private Color deleteButtonDefault = Color.red;

	private float maxBrushSizeDefualt = 250;
	private float removeMaxBrushSizeDefualt = 100;

	private int maxRemoveUndoDefualt = 2;

	//On Off Settings
	private bool isOn = true;
	private Texture2D onButton;
	private Texture2D offButton;
    private Texture2D buttonIcon;
    private Texture2D icon;
    private GUIStyle buttonSkin;
	private string onState = "on";

    //Styles
    private GUIStyle style;
    private GUIStyle styleBold;
    private GUIStyle styleFold;

    //Display the window.
    [MenuItem ("Window/Prefab Brush+")]
	public static void  ShowWindow () 
	{
		EditorWindow.GetWindow(typeof(PrefabBrush));
	}

    // Window has been selected
    void OnFocus()
    {
        // Remove delegate listener if it has previously been assigned.
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;

        // Add (or re-add) the delegate.
        SceneView.onSceneGUIDelegate += this.OnSceneGUI;
    }

    void Awake()
    {
        if (!System.IO.File.Exists(Application.persistentDataPath + "/PrefabPlusMeta.dat"))
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            System.IO.FileStream file = System.IO.File.Create(Application.persistentDataPath + "/PrefabPlusMeta.dat");
            bf.Serialize(file, new PrefabBrushSaveMeta());
            file.Close();
        }

        //Load Settings for settings tab when the user opens the tool.
        onButton = Resources.Load("Button_On") as Texture2D;
        offButton = Resources.Load("Button_Off") as Texture2D;
        buttonIcon = Resources.Load("Button_On") as Texture2D;
        icon = Resources.Load("Icon") as Texture2D;
        missingObject = Resources.Load("Missing Object") as GameObject;
        PrefabBrushSaveMeta meta = LoadBrushMeta();

        savesCount = meta.SaveCount;

        saveList.Clear();
        checkDelete.Clear();
        for (int i = 0; i < savesCount; i++)
        {
            saveList.Add(meta.SaveName[i]);
            checkDelete.Add(false);
        }

        //We check if there is a key for these values so that we don't set them to 0 (null) if there is no key.
        if (EditorPrefs.HasKey("HWmaxBrushSize") && EditorPrefs.HasKey("HWmaxBrushSizeRemove") && EditorPrefs.HasKey("HWmaxRemoveUndo"))
        {
            maxBrushSize = EditorPrefs.GetFloat("HWmaxBrushSize");
            removeMaxBrushSize = EditorPrefs.GetFloat("HWmaxBrushSizeRemove");
            maxRemoveUndo = EditorPrefs.GetInt("HWmaxRemoveUndo");
        }

        //Just check if the first key exists.
        if (EditorPrefs.HasKey("placeBrushR"))
        {
            placeBrush = new Vector4
            (EditorPrefs.GetFloat(name + "placeBrushR"),
             EditorPrefs.GetFloat(name + "placeBrushG"),
             EditorPrefs.GetFloat(name + "placeBrushB"),
             EditorPrefs.GetFloat(name + "placeBrushA"));

            removeBrush = new Vector4
                (EditorPrefs.GetFloat(name + "removeBrushR"),
                 EditorPrefs.GetFloat(name + "removeBrushG"),
                 EditorPrefs.GetFloat(name + "removeBrushB"),
                 EditorPrefs.GetFloat(name + "removeBrushA"));

            backGround = new Vector4
                (EditorPrefs.GetFloat(name + "backGroundR"),
                 EditorPrefs.GetFloat(name + "backGroundG"),
                 EditorPrefs.GetFloat(name + "backGroundB"),
                 EditorPrefs.GetFloat(name + "backGroundA"));

            backGroundTwo = new Vector4
                (EditorPrefs.GetFloat(name + "backGroundTwoR"),
                 EditorPrefs.GetFloat(name + "backGroundTwoG"),
                 EditorPrefs.GetFloat(name + "backGroundTwoB"),
                 EditorPrefs.GetFloat(name + "backGroundTwoA"));

            selectedTab = new Vector4
                (EditorPrefs.GetFloat(name + "selectedTabR"),
                 EditorPrefs.GetFloat(name + "selectedTabG"),
                 EditorPrefs.GetFloat(name + "selectedTabB"),
                 EditorPrefs.GetFloat(name + "selectedTabA"));

            deleteButton = new Vector4
                (EditorPrefs.GetFloat(name + "deleteButtonR"),
                 EditorPrefs.GetFloat(name + "deleteButtonG"),
                 EditorPrefs.GetFloat(name + "deleteButtonB"),
                 EditorPrefs.GetFloat(name + "deleteButtonA"));

            textColor = new Vector4
                (EditorPrefs.GetFloat(name + "TextColorR"),
                 EditorPrefs.GetFloat(name + "TextColorG"),
                 EditorPrefs.GetFloat(name + "TextColorB"),
                 EditorPrefs.GetFloat(name + "TextColorA"));
        }
    }

    public void SaveBrushMeta(PrefabBrushSaveMeta save)
    {
        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        System.IO.FileStream file = System.IO.File.Create(Application.persistentDataPath + "/PrefabPlusMeta.dat");
        bf.Serialize(file, save);
        file.Close();
    }

    public PrefabBrushSaveMeta LoadBrushMeta()
    {
        if (System.IO.File.Exists(Application.persistentDataPath + "/PrefabPlusMeta.dat"))
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            System.IO.FileStream file = System.IO.File.Open(Application.persistentDataPath + "/PrefabPlusMeta.dat", System.IO.FileMode.Open);
            PrefabBrushSaveMeta bSave = (PrefabBrushSaveMeta)bf.Deserialize(file);
            file.Close();
            return bSave;
        }
        else {
            Debug.Log(string.Format("File doesn't exist at path: {0}{1}", Application.persistentDataPath, "/PrefabPlusMeta.dat"));
            return null;
        }
    }

    void OnGUI()
	{
        style = EditorStyles.label;
        styleBold = EditorStyles.boldLabel;
        styleFold = EditorStyles.foldout;
        styleBold.normal.textColor = textColor;
        style.normal.textColor = textColor;
        styleFold.normal.textColor = textColor;
        styleFold.onFocused.textColor = textColor;
        styleFold.onActive.textColor = textColor;

        GUI.color = backGround;
        EditorGUILayout.BeginVertical("box");
        //Set the title for the tab.
        EditorGUILayout.BeginHorizontal ();
        GUI.color = Color.white;
        GUI.Label(new Rect(5, 6, 48, 48), icon);
        GUILayout.Label ("             Prefab Brush+", styleBold);
        GUI.color = backGround;	
		if (aboutTab)
        {
			GUI.color = selectedTab;
		} else {
			//Need to set it back to backGround Colour as the rest of the GUI is drawn with this colour.
			GUI.color = backGround;
		}
        GUI.color = backGroundTwo;
        if (GUILayout.Button ("About"))
        {
			placmentBrush = false;
			removalBrush = false;
			aboutTab = true;
			settingsTab = false;
		}
		GUI.color = backGround;
	
		EditorGUILayout.EndHorizontal ();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        //Begin the container box for the tool bar at the top of the window.
        GUILayout.BeginHorizontal ("box");
		
		//Set the button color so user know which tools is selected.
		if (placmentBrush)
        {
			//Set the defualt GUI colour to green. Unless changed, any GUI past this point will be green.
			GUI.color = selectedTab;
		} else {
			//GUI doesn't allow users to change the colour of single items so the colour must be reset for the remained of the items.
			GUI.color = backGround;
		}

        //If the GUI button has been pressed set the related bool to true. This is used to hide and show certain parts of the GUI and functionality.
        GUI.color = backGroundTwo;
        if (GUILayout.Button ("Placment Brush"))
        {
			placmentBrush = true;
			removalBrush = false;
			aboutTab = false;
			settingsTab = false;
		}

        //Set the button color so user know which tools is selected.
        if (removalBrush) {
			GUI.color = selectedTab;
		} else {
			GUI.color = backGround;
		}

        GUI.color = backGroundTwo;
        if (GUILayout.Button ("Removal Brush"))
        {
			placmentBrush = false;
			removalBrush = true;
			aboutTab = false;
			settingsTab = false;
		}
        GUI.color = backGround;

        if (settingsTab) {
			GUI.color = selectedTab;
		} else {
			//Need to set it back to white as the rest of the GUI is drawn with this colour.
			GUI.color = backGround;
		}
        GUI.color = backGroundTwo;
        if (GUILayout.Button ("Settings"))
        {
			placmentBrush = false;
			removalBrush = false;
			aboutTab = false;
			settingsTab = true;
		}
		GUI.color = backGround;
		//End the container box for the tool bar at the top of the window.
		GUILayout.EndHorizontal ();

		//-------------------------------------Button GUI End---------------------------------------------
		
		EditorGUILayout.BeginVertical ();
		scrollPos = EditorGUILayout.BeginScrollView (scrollPos, GUILayout.Height (position.height - 90));
		
		//If the placment brush is selected then display the GUI bellow.
		if (placmentBrush) 
		{
			EditorGUILayout.BeginVertical("box");
			EditorGUILayout.BeginHorizontal();
			GUI.color = Color.white;
			if (GUI.Button (new Rect (5, 6, 60, 30), buttonIcon, GUI.skin.label)) 
			{
				isOn = !isOn;
				
				if (isOn) {
					buttonIcon = onButton;
					onState = "on";
				} else {
					buttonIcon = offButton;
					onState = "off";
				}
			}
			GUI.color = backGround;
			EditorGUILayout.Space();
			GUILayout.Label(onState);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();

			Repaint();

			if (isOn)
			{
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				EditorGUILayout.Space ();
                GUI.color = Color.white;
                showBrushSettings = EditorGUILayout.Foldout (showBrushSettings, "Brush Settings", styleFold);
                		
				if (showBrushSettings) {
                    GUI.color = backGround;
                    EditorGUILayout.BeginVertical ("box");
                    GUI.color = Color.white;
                    GUILayout.Label ("GameObject", style);
                    GUI.color = backGround;

                    //Make sure that there is at least one object in the list.
                    if (objectCount <= 0) {
						objectCount++;
						selectedObject.Add (null);
					}

                    //Replace 'none object' with emptey one in order to stop old object references being picked up where no object exists in the list.
                    for (int i = 0; i < selectedObject.Count; i++)
                    {
                        if (selectedObject[i] == null)
                        {
                            selectedObject[i] = missingObject;
                        }
                    }

                    EditorGUILayout.BeginHorizontal ("box");
                    //Button that adds X ammount of objects to the list and increases the objectCount value by X.
                    GUI.color = backGroundTwo;
                    if (GUILayout.Button ("Add X Object(s)", GUILayout.Width (position.width / 2))) {
						for (int i = 0; i < addObjectSize; i++) {
							objectCount++;
							selectedObject.Add (missingObject);
						}
					}
                    addObjectSize = EditorGUILayout.IntField (addObjectSize);
					EditorGUILayout.EndHorizontal ();

                    //Draws an ObjectFeild for each object there is in the list.
                    for (int i = 0; i < objectCount; i++) {
						selectedObject [i] = EditorGUILayout.ObjectField (selectedObject [i], typeof(Object), true) as GameObject;
					}
                    GUI.color = backGround;
                    //Remove the the defined number of objects in the list and remove the same ammount from objectCount on button press.
                    EditorGUILayout.BeginHorizontal ("box");
                    GUI.color = backGroundTwo;
                    if (GUILayout.Button ("Remove X Object(s)", GUILayout.Width (position.width / 2))) {
						for (int i = 0; i < removeObjectSize; i++) {
							if (objectCount > 1) {
								selectedObject.RemoveAt (objectCount - 1);
								objectCount--;
							} else {
								Debug.Log ("Can not remove last object.");
							}
						}
					}
						
					removeObjectSize = EditorGUILayout.IntField (removeObjectSize);
					EditorGUILayout.EndHorizontal ();
						
					EditorGUILayout.Space ();
                    //Define radius of the brush.
                    GUI.color = Color.white;
                    GUILayout.Label ("Brush Size: " + brushSize.ToString (), style);
                    GUI.color = backGroundTwo;
                    brushSize = GUILayout.HorizontalSlider (brushSize, 1, maxBrushSize);
						
					EditorGUILayout.Space ();
                    //Define how dense the objects are brushed.
                    GUI.color = Color.white;
                    GUILayout.Label ("Opacity: " + opacity.ToString (), style);
                    GUI.color = backGroundTwo;
                    opacity = GUILayout.HorizontalSlider (opacity, 1, 100);
						
					//Calculate the Opacity value 
					opacityValue = brushSize * (opacity / 1000);
						
					EditorGUILayout.Space ();

                    //Define the layer that the brush will brush objects on.
                    GUI.color = Color.white;
                    GUILayout.Label ("Layer To Brush", style);
                    GUI.color = backGroundTwo;
                    EditorGUILayout.BeginHorizontal ("box");
					checkLayer = EditorGUILayout.Toggle (checkLayer);
                    GUI.color = Color.white;
                    GUILayout.Label (checkLayer.ToString (), style);
                    GUI.color = backGroundTwo;
                    EditorGUILayout.EndHorizontal ();
						
					if (checkLayer) {
						layers = EditorGUILayout.LayerField (layers);
					}
						
					EditorGUILayout.Space ();
						
					EditorGUILayout.Space ();
                    //Define the object tag that the brush will brush objects on.
                    GUI.color = Color.white;
                    GUILayout.Label ("Tag To Brush", style);
                    GUI.color = backGroundTwo;
                    EditorGUILayout.BeginHorizontal ("box");
					checkTag = EditorGUILayout.Toggle (checkTag);
                    GUI.color = Color.white;
                    GUILayout.Label (checkTag.ToString (), style);
					EditorGUILayout.EndHorizontal ();
						
					if (checkTag) {
						tag = EditorGUILayout.TextField (tag);
					}
						
					EditorGUILayout.Space ();

                    GUI.color = Color.white;
                    GUILayout.Label ("Slope Angle To Brush", style);
                    GUI.color = backGroundTwo;
                    EditorGUILayout.BeginVertical ("box");
                    EditorGUILayout.BeginHorizontal ();
					checkSlope = EditorGUILayout.Toggle (checkSlope);
                    GUI.color = Color.white;
                    GUILayout.Label (checkSlope.ToString (), style);
					EditorGUILayout.EndHorizontal ();
					EditorGUILayout.EndVertical ();
						
					if (checkSlope) {
                        GUI.color = Color.white;
                        GUILayout.Label ("Minimum Slope Angle: " + minSlope + "| Maximum Slope Angle: " + maxSlope, style);
                        GUI.color = backGroundTwo;
                        EditorGUILayout.MinMaxSlider (ref minSlope, ref maxSlope, 0, 90);
					}
						
					EditorGUILayout.EndVertical ();
				}
					
				EditorGUILayout.Space ();
                GUI.color = Color.white;
                showObjectSettings = EditorGUILayout.Foldout (showObjectSettings, "Object Settings", styleFold);
					
				if (showObjectSettings) {
                    GUI.color = backGround;
                    EditorGUILayout.BeginVertical ("box");
                    GUI.color = Color.white;
                    GUILayout.Label("Offset Center Of Prefab", style);
                    GUI.color = backGroundTwo;
                    EditorGUILayout.BeginVertical("box");
                    offsetPrefabsOrigin = EditorGUILayout.Vector3Field("", offsetPrefabsOrigin);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();

                    GUI.color = Color.white;
                    GUILayout.Label("Offset Rotation Of Prefab", style);
                    GUI.color = backGroundTwo;
                    EditorGUILayout.BeginVertical("box");
                    offsetPrefabRotation = EditorGUILayout.Vector3Field("", offsetPrefabRotation);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();

                    GUI.color = Color.white;
                    GUILayout.Label ("Custom Parent of Brushed Objects", style);
                    GUI.color = backGroundTwo;
                    EditorGUILayout.BeginHorizontal ("box");
					setParentToObject = EditorGUILayout.Toggle (setParentToObject);
                    GUI.color = Color.white;
                    GUILayout.Label (setParentToObject.ToString (), style);
					EditorGUILayout.EndHorizontal ();
					if (setParentToObject) {
                        GUI.color = backGroundTwo;
                        parent = EditorGUILayout.ObjectField (parent, typeof(GameObject), true) as GameObject;
						setParentToBrushBase = false;
					}

                    GUI.color = Color.white;
                    GUILayout.Label ("Brushed Objects Become Child of Surface", style);
                    GUI.color = backGroundTwo;
                    EditorGUILayout.BeginHorizontal ("box");
					setParentToBrushBase = EditorGUILayout.Toggle (setParentToBrushBase);
                    GUI.color = Color.white;
                    GUILayout.Label (setParentToBrushBase.ToString (), style);
					EditorGUILayout.EndHorizontal ();
					if (setParentToBrushBase) {
						setParentToObject = false;
					}
						
					GUILayout.Label ("Rotate GameObjects to Match Surface", style);
                    GUI.color = backGroundTwo;
                    EditorGUILayout.BeginHorizontal ("box");
					rotateToSurface = EditorGUILayout.Toggle (rotateToSurface);
                    GUI.color = Color.white;
                    GUILayout.Label (rotateToSurface.ToString (), styleBold);
					if (rotateToSurface) {
                        GUI.color = backGroundTwo;
                        rotToSurfaceVector = EditorGUILayout.Vector3Field ("", rotToSurfaceVector);
					}
					EditorGUILayout.EndHorizontal ();
                    GUI.color = Color.white;
                    if (rotateToSurface) {
                        EditorGUILayout.HelpBox ("If your GameObjects aren't facing up switch around the X,Y,Z values.", MessageType.Info);
					}

                    GUILayout.Label ("Customize Rotation", style);
                    GUI.color = backGroundTwo;
                    EditorGUILayout.BeginVertical ("box");
					EditorGUILayout.BeginHorizontal ();
                    randomRotation = EditorGUILayout.Toggle (randomRotation);
                    GUI.color = Color.white;
                    GUILayout.Label (randomRotation.ToString (), style);
					EditorGUILayout.EndHorizontal ();
					if (randomRotation) {
                        GUI.color = Color.white;
                        GUILayout.Label ("X Minimum Rotation: " + minXRotation + "| X Maximum Rotation: " + maxXRotation, style);
                        GUI.color = backGroundTwo;
                        EditorGUILayout.MinMaxSlider (ref minXRotation, ref maxXRotation, 0, 360);
                        GUI.color = Color.white;
                        GUILayout.Label ("Y Minimum Rotation: " + minYRotation + "| Y Maximum Rotation: " + maxYRotation, style);
                        GUI.color = backGroundTwo;
                        EditorGUILayout.MinMaxSlider (ref minYRotation, ref maxYRotation, 0, 360);
                        GUI.color = Color.white;
                        GUILayout.Label ("Z Minimum Rotation: " + minZRotation + "| Z Maximum Rotation: " + maxZRotation, style);
                        GUI.color = backGroundTwo;
                        EditorGUILayout.MinMaxSlider (ref minZRotation, ref maxZRotation, 0, 360);
                        GUI.color = Color.white;
                        GUILayout.Label ("0-360", style);
							
						EditorGUILayout.BeginHorizontal ();
                        GUI.color = backGroundTwo;
                        if (GUILayout.Button ("Set all to: ")) {
							minXRotation = rotationSet;
							minYRotation = rotationSet;
							minZRotation = rotationSet;
							maxXRotation = rotationSet;
							maxYRotation = rotationSet;
							maxZRotation = rotationSet;
						}
							
						rotationSet = EditorGUILayout.FloatField (rotationSet);
							
						EditorGUILayout.EndVertical ();
					}
					EditorGUILayout.EndHorizontal ();

                    GUI.color = Color.white;
                    GUILayout.Label ("Customize Scale", style);
                    GUI.color = backGroundTwo;
                    EditorGUILayout.BeginHorizontal ("box");
					randomScale = EditorGUILayout.Toggle (randomScale);
                    GUI.color = Color.white;
                    GUILayout.Label (randomScale.ToString (), style);
					EditorGUILayout.EndHorizontal ();
						
					EditorGUILayout.Space ();
					if (randomScale) {
                        GUI.color = Color.white;
                        minScale = EditorGUILayout.FloatField ("Minimum Scale", minScale, style);
						maxScale = EditorGUILayout.FloatField ("Maximum Scale", maxScale, style);
					}
					EditorGUILayout.EndVertical ();
				}
					
				EditorGUILayout.Space ();
                GUI.color = Color.white;
                showBrushSaves = EditorGUILayout.Foldout (showBrushSaves, "Brush Saves", styleFold);
					
				if (showBrushSaves) {
                    GUI.color = backGroundTwo;
                    EditorGUILayout.BeginVertical ("box");
						
					EditorGUILayout.BeginHorizontal ("box");
					if (GUILayout.Button ("Save Brush")) {
						SaveBrush (saveName);
					}
                    GUI.color = Color.white;
                    saveName = EditorGUILayout.TextField (saveName);
					EditorGUILayout.EndHorizontal ();
						
					for (int i = 0; i < saveList.Count; i++) {
                        GUI.color = backGroundTwo;
                        if (checkDelete [i] == false) {
                            EditorGUILayout.BeginHorizontal ("box");
							if (GUILayout.Button ("Load " + saveList [i])) {
								LoadBrush (saveList [i]);
							}
								
							GUI.color = deleteButton;
							if (GUILayout.Button ("Delete " + saveList [i])) {
								checkDelete [i] = true;
							}
							GUI.color = backGround;
							EditorGUILayout.EndHorizontal ();
						} else {
                            GUI.color = Color.white;
                            GUILayout.Label ("Are you sure you want to delete : " + saveList [i] + " ?");
							EditorGUILayout.BeginHorizontal ();

                            GUI.color = backGroundTwo;
                            if (GUILayout.Button ("Yes")) {						
								EditorPrefs.DeleteKey (saveList [i]);
								saveList.Remove (saveList [i]);
								checkDelete.Remove (checkDelete [i]);
							}
							if (GUILayout.Button ("No")) {
								checkDelete [i] = false;
							}
								
							EditorGUILayout.EndHorizontal ();
						}
					}
					EditorGUILayout.EndVertical ();
				}
			}
		}
		
		//If the removal brush is selected then display the GUI bellow.
		if(removalBrush)
		{
            GUI.color = Color.white;
			GUILayout.Label("Brush Settings", styleBold);
            GUI.color = backGroundTwo;
            EditorGUILayout.BeginVertical("box");

            GUI.color = Color.white;
            GUILayout.Label("Brush Size", style);
            GUI.color = backGroundTwo;
            removeBrushSize = EditorGUILayout.Slider(removeBrushSize, 1, removeMaxBrushSize);
			
			EditorGUILayout.EndVertical();

            GUI.color = Color.white;
            GUILayout.Label("Object Settings", styleBold);
            GUI.color = backGroundTwo;
            EditorGUILayout.BeginVertical("box");
            GUI.color = Color.white;
            GUILayout.Label("Required Tag", style);
            GUI.color = backGroundTwo;
            EditorGUILayout.BeginHorizontal("box");
			checkTagRemove = EditorGUILayout.Toggle(checkTagRemove);
            GUI.color = Color.white;
            GUILayout.Label(checkTagRemove.ToString(), style);
			EditorGUILayout.EndHorizontal();
			
			if(checkTagRemove)
			{
                GUI.color = backGroundTwo;
                removeTagToBrushOn = EditorGUILayout.TextField(removeTagToBrushOn);
			}

            GUI.color = Color.white;
            GUILayout.Label("Required Layer", style);
            GUI.color = backGroundTwo;
            EditorGUILayout.BeginHorizontal("box");
			checkLayerRemove = EditorGUILayout.Toggle(checkLayerRemove);
            GUI.color = Color.white;
            GUILayout.Label(checkLayerRemove.ToString(), style);
			EditorGUILayout.EndHorizontal();
			
			if(checkLayerRemove)
			{
                GUI.color = backGroundTwo;
                removeLayerToBrushOn = EditorGUILayout.LayerField(removeLayerToBrushOn);
			}
			
			EditorGUILayout.EndVertical();
		}

		//If the settings tab is selected then display the settings
		if(settingsTab)
		{
            GUI.color = Color.white;
            GUILayout.Label ("Brush Settings", style);
            GUI.color = backGroundTwo;
            EditorGUILayout.BeginVertical("box");
            GUI.color = Color.white;
            GUILayout.Label ("Max Placment Brush Size", style);
            GUI.color = backGroundTwo;
            maxBrushSize = EditorGUILayout.FloatField(maxBrushSize);
			EditorGUILayout.Space();
            GUI.color = Color.white;
            GUILayout.Label("Max Remove Brush Size", style);
            GUI.color = backGroundTwo;
            removeMaxBrushSize = EditorGUILayout.FloatField(removeMaxBrushSize);
			EditorGUILayout.Space();
            GUI.color = Color.white;
            GUILayout.Label ("Remove Brush Undo Grouping Size", style);
            GUI.color = backGroundTwo;
            maxRemoveUndo = EditorGUILayout.IntField(maxRemoveUndo);
            GUI.color = Color.white;
            GUILayout.Label("WARNING: increase this number at your own risk, increasing this number past 15 may cause crashes.", EditorStyles.helpBox);
			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();

			GUILayout.Label ("Color Settings", style);
            GUI.color = backGroundTwo;
            EditorGUILayout.BeginVertical("box");
            GUI.color = Color.white;
            GUILayout.Label("Background Color", style);
            backGround = EditorGUILayout.ColorField(backGround);
            EditorGUILayout.Space();

            GUILayout.Label("Background Color 2", style);
            backGroundTwo = EditorGUILayout.ColorField(backGroundTwo);
            EditorGUILayout.Space();

            GUILayout.Label("Text Color", style);
            textColor = EditorGUILayout.ColorField(textColor);
            EditorGUILayout.Space();

            GUILayout.Label("Placement Brush Color", style);
			placeBrush = EditorGUILayout.ColorField(placeBrush);
			EditorGUILayout.Space();

			GUILayout.Label("Remove Brush Color", style);
			removeBrush = EditorGUILayout.ColorField(removeBrush);
			EditorGUILayout.Space();

			GUILayout.Label("Selected Tab Color", style);
			selectedTab = EditorGUILayout.ColorField(selectedTab);
			EditorGUILayout.Space();

			GUILayout.Label("Delete Save Color", style);
			deleteButton = EditorGUILayout.ColorField(deleteButton);
			EditorGUILayout.Space();
			EditorGUILayout.EndVertical();
            GUI.color = backGroundTwo;
            if (GUILayout.Button("Reset to Defualt"))
			{
                backGround = backGroundDefault;
                backGroundTwo = backGroundDefault;
                placeBrush = placeBrushDefault;
				removeBrush = removeBrushDefault;
				selectedTab = selectedTabDefault;
				deleteButton = deleteButtonDefault;
                textColor = Color.black;

				maxBrushSize = maxBrushSizeDefualt;
				removeMaxBrushSize = removeMaxBrushSizeDefualt;

				maxRemoveUndo = maxRemoveUndoDefualt;
			}
		}

        //If the idle tab is selected then display the GUI bellow.
        if (aboutTab)
		{
			//Make sure that the colour for the GUI is set back to the defualt.
			GUI.color = backGround;
			GUILayout.Label("About the Prefab Brush+", styleBold);
			EditorGUILayout.HelpBox("Created by HelloWorld Studios - www.HelloWorldStudios.co.uk", MessageType.Info);
			EditorGUILayout.HelpBox("Feel free to contact support@helloworldstudios.co.uk with support inquiries.", MessageType.Info);
		}

        //End the scroll window.
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical ();
        EditorGUILayout.EndVertical();
    }

    void OnSceneGUI(SceneView sceneView)
    {
        //Hide gizmos when brushing
        if ((placmentBrush && isOn) || removalBrush)
        {
            Tools.hidden = true;
        }
        else
        {
            Tools.hidden = false;
        }


        if (placmentBrush && isOn)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            Ray drawPointRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit drawPointHit;

            if (Physics.Raycast(drawPointRay, out drawPointHit))
            {
                Handles.color = placeBrush;
                Handles.DrawSolidDisc(drawPointHit.point, Vector3.up, brushSize / 2);
                SceneView.RepaintAll();
            }

            //If the placment brush is selected and the mouse is being dragged across the scene view.
            if (Event.current.type == EventType.MouseDrag && Event.current.button == 0 && placmentBrush || Event.current.type == EventType.MouseDown && Event.current.button == 0 && placmentBrush)
            {

                if (selectedObject == null)
                {
                    Debug.LogError("There is no object selected in the Level tool window, please drag a prefab into the area to use the placment brush.");
                }
                else {

                    //Run the placment multiple times per frame to provide a opacity.
                    for (int i = 0; i < opacityValue; i++)
                    {

                        //Calculate the radius of the brush size.
                        float newBrushSize = brushSize / 2;

                        //Create a raycast that will come from the top of the world down onto a random point within the brush size raduis calculated above.
                        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                        RaycastHit hit;

                        if (Physics.Raycast(new Vector3(ray.origin.x + Random.insideUnitSphere.x * newBrushSize, ray.origin.y, ray.origin.z + Random.insideUnitSphere.z * newBrushSize), ray.direction, out hit))
                        {
                            if (checkSlope)
                            {
                                angle = (Vector3.Angle(hit.normal, Vector3.up));
                            }
                            else
                            {
                                minSlope = 0;
                                maxSlope = 90;
                            }

                            //If the layer or tag doesn't need to be checked then they can be set the the surface object's tag and layer.
                            if (checkLayer == false)
                            {
                                layers = hit.collider.gameObject.layer;
                            }

                            if (checkTag == false)
                            {
                                tag = hit.collider.gameObject.tag;
                            }

                            //Brushing begins here////////////////////////////////////////////////////////////////////////////////////
                            //////////////////////////////////////////////////////////////////////////////////////////////////////////

                            //If the object hit meets the defined reqirments.
                            if (hit.collider.gameObject.tag == tag && hit.collider.gameObject.layer == layers && hit.collider.gameObject != selectedObject[0] && angle >= minSlope && angle <= maxSlope)
                            {
                                //Create a GameObject var to store the spawned GameObject. This will then be used to set rotation and scale and save them for the undo proccess.
                                GameObject clone = null;
                                int t = Random.Range(0, objectCount);

                                for (int j = 0; j < objectCount; j++)
                                {
                                    if (selectedObject[t] != missingObject as GameObject)
                                    {
                                        break;
                                    }
                                    t = Random.Range(0, objectCount);
                                }

                                if (selectedObject[t] != missingObject)
                                {
                                    clone = PrefabUtility.InstantiatePrefab(selectedObject[t]) as GameObject;
                                }

                                if (clone != null)
                                {
                                    clone.transform.position = new Vector3(hit.point.x + offsetPrefabsOrigin.x, hit.point.y + offsetPrefabsOrigin.y, hit.point.z + offsetPrefabsOrigin.z);
                                    clone.transform.eulerAngles += offsetPrefabRotation;

                                    if (setParentToObject)
                                    {
                                        if (parent != null)
                                        {
                                            clone.transform.parent = parent.transform;
                                        }
                                        else {
                                            Debug.LogWarning("Prefab Brush is trying to set the objects parent to null. Please check that you have defined a gameobject in the Prefab Brush window.");
                                        }
                                    }

                                    if (setParentToBrushBase)
                                    {
                                        clone.transform.parent = hit.collider.gameObject.transform;
                                    }

                                    //If rotate to surface has been selected then set the spawn objects rotation to the bases normal.
                                    if (rotateToSurface)
                                    {
                                        clone.transform.rotation = Quaternion.FromToRotation(rotToSurfaceVector, hit.normal);
                                    }

                                    //If random rotation has been selected then apply a random rotation define by the values in the window.
                                    if (randomRotation)
                                    {
                                        //Set the roation to the random offset plus the original rotation.
                                        clone.transform.rotation *= Quaternion.Euler(new Vector3(Random.Range(minXRotation, maxXRotation), Random.Range(minYRotation, maxYRotation), Random.Range(minZRotation, maxZRotation)));

                                    }

                                    //If random scale has been selected then apply a new scale transform to each object based on a random range.
                                    if (randomScale)
                                    {
                                        //Create a random number between the min and max scale values.
                                        float rnd = Random.Range(minScale, maxScale);
                                        //Set the objects scale to the random number. (Based on imported scale)
                                        clone.transform.localScale = new Vector3(rnd, rnd, rnd);
                                    }

                                    //Store the undo variables.
                                    Undo.RegisterCreatedObjectUndo(clone, "brush stroke: " + clone.name);
                                }
                            }
                        }
                    }
                }
            }
        }
        //End of 'if placement brush'.

        if (removalBrush)
        {

            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            Ray drawPointRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit drawPointHit;

            if (Physics.Raycast(drawPointRay, out drawPointHit))
            {
                Handles.color = removeBrush;
                Handles.DrawSolidDisc(drawPointHit.point, Vector3.up, removeBrushSize / 2);
                SceneView.RepaintAll();
            }

            //If the placment brush is selected and the mouse is being dragged across the scene view.
            if (Event.current.type == EventType.MouseDrag && Event.current.button == 0 && removalBrush || Event.current.type == EventType.MouseDown && Event.current.button == 0 && removalBrush)
            {
                float newRemoveBrushSize = removeBrushSize / 2;

                //Create a raycast that will come from the top of the world down onto a random point within the brush size raduis calculated above.
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Collider[] hitColliders = Physics.OverlapSphere(hit.point, newRemoveBrushSize);
                    for (int i = 0; i < hitColliders.Length; i++)
                    {
                        if (hitColliders[i] != null)
                        {

                            if (checkTagRemove == false)
                            {
                                removeTagToBrushOn = hitColliders[i].gameObject.tag;
                            }

                            if (checkLayerRemove == false)
                            {
                                removeLayerToBrushOn = hitColliders[i].gameObject.layer;
                            }

                            if (hitColliders[i].gameObject.tag == removeTagToBrushOn && hitColliders[i].gameObject.layer == removeLayerToBrushOn)
                            {
                                Undo.DestroyObjectImmediate(hitColliders[i].gameObject);
                                inc++;
                                if (inc >= maxRemoveUndo)
                                {
                                    Undo.IncrementCurrentGroup();
                                    inc = 0;
                                }
                            }
                        }
                    }
                }
            }//End of mouse drag if statment.
        }
    }

    //Saves the saved brush class to a standalone file.
    public void SaveBrush(PrefabBrushSave save, string brushName)
    {
        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        System.IO.FileStream file = System.IO.File.Create(Application.persistentDataPath + "/" + brushName + ".dat");
        bf.Serialize(file, save);
        file.Close();
    }

    void SaveBrush(string name)
    {
        bool overwrite = false;

        for (int i = 0; i < saveList.Count; i++)
        {
            if (name == saveList[i])
            {
                Debug.Log("Prefab Brush+: " + name + " has been overwritten.");
                overwrite = true;
                break;
            }
        }

        if (overwrite == false)
        {
            saveList.Add(name);
            checkDelete.Add(false);
        }

        PrefabBrushSave newSave = new PrefabBrushSave();

        newSave.ShowBrushSettings = showBrushSettings;
        newSave.ShowObjectSettings = showObjectSettings;
        newSave.ObjectCount = objectCount;
        newSave.SelectedObject = new string[objectCount];
        for (int i = 0; i < selectedObject.Count; i++)
        {
            if (selectedObject[i] != null)
            {
                newSave.SelectedObject[i] = AssetDatabase.GetAssetPath(selectedObject[i]);
            }
        }

        newSave.BrushSize = brushSize;
        newSave.MaxBrushSize = maxBrushSize;
        newSave.Opacity = opacity;
        newSave.Layers = layers.value;
        newSave.Tag = tag;

        newSave.RotateToSurface = rotateToSurface;
        newSave.RandomScale = randomScale;
        newSave.RandomRotation = randomRotation;
        newSave.SetParentToBrushBase = setParentToBrushBase;
        newSave.SetParentToObject = setParentToObject;
        newSave.CheckLayer = checkLayer;
        newSave.CheckTag = checkTag;
        newSave.CheckSlope = checkSlope;
        newSave.CheckTagRemove = checkTagRemove;
        newSave.CheckLayerRemove = checkLayerRemove;

        newSave.OffsetPrefabRotationX = offsetPrefabsOrigin.x;
        newSave.OffsetPrefabRotationY = offsetPrefabsOrigin.y;
        newSave.OffsetPrefabRotationZ = offsetPrefabsOrigin.z;

        newSave.OffsetPrefabRotationX = offsetPrefabRotation.x;
        newSave.OffsetPrefabRotationY = offsetPrefabRotation.y;
        newSave.OffsetPrefabRotationZ = offsetPrefabRotation.z;

        newSave.RotToSurfaceVectorX = rotToSurfaceVector.x;
        newSave.RotToSurfaceVectorY = rotToSurfaceVector.y;
        newSave.RotToSurfaceVectorZ = rotToSurfaceVector.z;

        if (parent != null)
        {
            newSave.Parent = parent.GetInstanceID();
        }

        newSave.RemoveObjectSize = removeObjectSize;
        newSave.AddObjectSize = addObjectSize;

        newSave.MinXRotation = minXRotation;
        newSave.MaxXRotation = maxXRotation;
        newSave.MinYRotation = minYRotation;
        newSave.MaxYRotation = maxYRotation;
        newSave.MinZRotation = minZRotation;
        newSave.MaxZRotation = maxZRotation;

        newSave.MaxSlope = maxSlope;
        newSave.MinSlope = minSlope;

        newSave.RotationSet = rotationSet;

        newSave.MinScale = minScale;
        newSave.MaxScale = maxScale;

        newSave.OpacityValue = opacityValue;

        SaveBrush(newSave, name);
    }

    //Loads the saved brush class from a standalone file
    public PrefabBrushSave LoadNewBrush(string brushName)
    {
        if (System.IO.File.Exists(Application.persistentDataPath + "/" + brushName + ".dat"))
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            System.IO.FileStream file = System.IO.File.Open(Application.persistentDataPath + "/" + brushName + ".dat", System.IO.FileMode.Open);
            PrefabBrushSave bSave = (PrefabBrushSave)bf.Deserialize(file);
            file.Close();
            return bSave;
        }
        else {
            Debug.Log(string.Format("File doesn't exist at path: {0}{1}", Application.persistentDataPath, "/" + brushName + ".dat"));
            return null;
        }
    }

    void LoadBrush(string name)
	{
        PrefabBrushSave newSave = LoadNewBrush(name);

        showBrushSettings = newSave.ShowBrushSettings;
        showObjectSettings = newSave.ShowObjectSettings;
		objectCount = newSave.ObjectCount;
		
		selectedObject.Clear();
        for (int i = 0; i < objectCount; i++)
        {
            selectedObject.Add(null);
            selectedObject[i] = AssetDatabase.LoadAssetAtPath(newSave.SelectedObject[i].ToString(), typeof(GameObject)) as GameObject;
        }
		
		maxBrushSize = newSave.MaxBrushSize;
		brushSize = newSave.BrushSize;
		opacity = newSave.Opacity;
		layers.value = newSave.Layers;
		tag = newSave.Tag;
		
		rotateToSurface = newSave.RotateToSurface;
		randomScale = newSave.RandomScale;
		randomRotation = newSave.RandomRotation;
		setParentToBrushBase = newSave.SetParentToBrushBase;
		setParentToObject = newSave.SetParentToObject;
        checkLayer = newSave.CheckLayer;
		checkTag = newSave.CheckTag;
		checkSlope = newSave.CheckSlope;
		checkTagRemove = newSave.CheckTagRemove;
		checkLayerRemove = newSave.CheckLayerRemove;

        offsetPrefabsOrigin.x = newSave.OffsetPrefabsOriginX;
        offsetPrefabsOrigin.y = newSave.OffsetPrefabsOriginY;
        offsetPrefabsOrigin.z = newSave.OffsetPrefabsOriginZ;

        offsetPrefabRotation.x = newSave.OffsetPrefabRotationX;
        offsetPrefabRotation.y = newSave.OffsetPrefabRotationY;
        offsetPrefabRotation.z = newSave.OffsetPrefabRotationZ;

        rotToSurfaceVector.x = newSave.RotToSurfaceVectorX;
        rotToSurfaceVector.y = newSave.RotToSurfaceVectorY;
        rotToSurfaceVector.z = newSave.RotToSurfaceVectorZ;

        parent = EditorUtility.InstanceIDToObject(newSave.Parent) as GameObject;

        removeObjectSize = newSave.RemoveObjectSize;
		addObjectSize = newSave.AddObjectSize;
		
		minXRotation = newSave.MinXRotation;
		maxXRotation = newSave.MaxXRotation;
		minYRotation = newSave.MinYRotation;
		maxYRotation = newSave.MaxYRotation;
		minZRotation = newSave.MinZRotation;
		maxZRotation = newSave.MaxZRotation;
		
		maxSlope = newSave.MaxSlope;
		minSlope = newSave.MinSlope;
		
		rotationSet = newSave.RotationSet;
		
		minScale = newSave.MinScale;
		maxScale = newSave.MaxScale;
		
		opacityValue = newSave.OpacityValue;
	}
	
	void OnDestroy() 
	{
		// When the window is destroyed, remove the delegate so that it will no longer do any drawing.
		SceneView.onSceneGUIDelegate -= this.OnSceneGUI;

        PrefabBrushSaveMeta save = new PrefabBrushSaveMeta();

		//Save a list of the saved keys.
		savesCount = saveList.Count;
        save.SaveCount = savesCount;
        save.SaveName = new string[savesCount];
		
		for(int i = 0; i < saveList.Count; i++)
		{
            save.SaveName[i] = saveList[i];
		}

        SaveBrushMeta(save);

		//Save the settings from the settings tab
		EditorPrefs.SetFloat ("HWmaxBrushSize", maxBrushSize);
		EditorPrefs.SetFloat ("HWmaxBrushSizeRemove", removeMaxBrushSize);
		EditorPrefs.SetInt("HWmaxRemoveUndo", maxRemoveUndo);

		//Color
		EditorPrefs.SetFloat (name + "placeBrushR", placeBrush.r);
		EditorPrefs.SetFloat (name + "placeBrushG", placeBrush.g);
		EditorPrefs.SetFloat (name + "placeBrushB", placeBrush.b);
		EditorPrefs.SetFloat (name + "placeBrushA", placeBrush.a);

		EditorPrefs.SetFloat (name + "removeBrushR", removeBrush.r);
		EditorPrefs.SetFloat (name + "removeBrushG", removeBrush.g);
		EditorPrefs.SetFloat (name + "removeBrushB", removeBrush.b);
		EditorPrefs.SetFloat (name + "removeBrushA", removeBrush.a);

        EditorPrefs.SetFloat(name + "backGroundR", backGround.r);
        EditorPrefs.SetFloat(name + "backGroundG", backGround.g);
        EditorPrefs.SetFloat(name + "backGroundB", backGround.b);
        EditorPrefs.SetFloat(name + "backGroundA", backGround.a);

        EditorPrefs.SetFloat(name + "backGroundTwoR", backGroundTwo.r);
        EditorPrefs.SetFloat(name + "backGroundTwoG", backGroundTwo.g);
        EditorPrefs.SetFloat(name + "backGroundTwoB", backGroundTwo.b);
        EditorPrefs.SetFloat(name + "backGroundTwoA", backGroundTwo.a);

        EditorPrefs.SetFloat (name + "selectedTabR", selectedTab.r);
		EditorPrefs.SetFloat (name + "selectedTabG", selectedTab.g);
		EditorPrefs.SetFloat (name + "selectedTabB", selectedTab.b);
		EditorPrefs.SetFloat (name + "selectedTabA", selectedTab.a);

        EditorPrefs.SetFloat(name + "deleteButtonR", deleteButton.r);
        EditorPrefs.SetFloat(name + "deleteButtonG", deleteButton.g);
        EditorPrefs.SetFloat(name + "deleteButtonB", deleteButton.b);
        EditorPrefs.SetFloat(name + "deleteButtonA", deleteButton.a);

        EditorPrefs.SetFloat(name + "TextColorR", textColor.r);
        EditorPrefs.SetFloat(name + "TextColorG", textColor.g);
        EditorPrefs.SetFloat(name + "TextColorB", textColor.b);
        EditorPrefs.SetFloat(name + "TextColorA", textColor.a);
	}
}

[System.Serializable]
public class PrefabBrushSaveMeta
{
    public string[] SaveName { get; set; }
    public int SaveCount { get; set; }
}

[System.Serializable]
public class PrefabBrushSave
{
    public bool ShowBrushSettings { get; set; }
    public bool ShowObjectSettings { get; set; }
    public int ObjectCount { get; set; }
    public string[] SelectedObject { get; set; }

    public float MaxBrushSize { get; set; }
    public float BrushSize { get; set; }
    public float Opacity { get; set; }
    public int Layers { get; set; }
    public string Tag { get; set; }

    public bool RotateToSurface { get; set; }
    public bool RandomScale { get; set; }
    public bool RandomRotation { get; set; }
    public bool SetParentToBrushBase { get; set; }
    public bool SetParentToObject { get; set; }
    public bool CheckLayer { get; set; }
    public bool CheckTag { get; set; }
    public bool CheckSlope { get; set; }
    public bool CheckTagRemove { get; set; }
    public bool CheckLayerRemove { get; set; }

    public float OffsetPrefabsOriginX { get; set; }
    public float OffsetPrefabsOriginY { get; set; }
    public float OffsetPrefabsOriginZ { get; set; }

    public float OffsetPrefabRotationX { get; set; }
    public float OffsetPrefabRotationY { get; set; }
    public float OffsetPrefabRotationZ { get; set; }

    public float RotToSurfaceVectorX { get; set; }
    public float RotToSurfaceVectorY { get; set; }
    public float RotToSurfaceVectorZ { get; set; }

    public int Parent { get; set; }

    public int RemoveObjectSize { get; set; }
    public int AddObjectSize { get; set; }

    public float MinXRotation { get; set; }
    public float MaxXRotation { get; set; }
    public float MinYRotation { get; set; }
    public float MaxYRotation { get; set; }
    public float MinZRotation { get; set; }
    public float MaxZRotation { get; set; }

    public float MaxSlope { get; set; }
    public float MinSlope { get; set; }

    public float RotationSet { get; set; }

    public float MinScale { get; set; }
    public float MaxScale { get; set; }

    public float OpacityValue { get; set; }
}