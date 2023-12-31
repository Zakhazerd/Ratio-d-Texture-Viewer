﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ShibaHelper;
using System.Reflection;
using System;
using System.IO;

public class Ratiod : EditorWindow
{
    Material shaderMaterial;
    Texture2D currentTexture;
    Texture2D[] textureArray = new Texture2D[12];
    Vector2 scrollPos;
    Vector2 scrollPos2;
    public static bool alphaUsed;
    public static int currentProccessedTexture = 0;
    public static bool processingTexture = false;
    public static string newAssetPath;
    public static int textureSize = 0;
    public static string textureName;
    public static bool isNormalMap = false;
    public static float[] vramSize = new float[12];
    List<string> textureNameList = new List<string>(); //now this would be a good idea to make into its own class but im not going to :)
    List<Texture2D> textureList = new List<Texture2D>();
    int listIndex = 0;
    int whichSize =0;
    string whichResize = "Bilinear";
    string whichCompresssion = "DXT";
    int textureArrayLocation = 0;
    Rect extraView = new Rect(0, 0, 500, 500);
    [MenuItem("Tools/Ratio'd")]
    public static void ShowWindow()
    {
        GetWindow(typeof(Ratiod));
    }
    private void OnGUI()
    {
        //Intial labeling
        GUILayout.Label("Ratio'd Texture Viewer", EditorStyles.boldLabel);
        GUILayout.Label("Version 1.1 \n", EditorStyles.miniLabel);
        ShibaHelpers.DrawLine();

        //The avatar shader and getting of the texture list
        EditorGUI.BeginChangeCheck();
        shaderMaterial = EditorGUILayout.ObjectField("Avatar Shader", shaderMaterial, typeof(Material), true) as Material;
        if (EditorGUI.EndChangeCheck())
        {
            if (shaderMaterial != null)
            {
                textureNameList.Clear();
                textureList.Clear(); 
                string[] myProperties = shaderMaterial.GetTexturePropertyNames();
                for (int i = 0; i < myProperties.Length; i++)
                {
                    if (shaderMaterial.GetTexture(myProperties[i]) != null)
                    {
                        textureNameList.Add(myProperties[i]);
                        textureList.Add(shaderMaterial.GetTexture(myProperties[i]) as Texture2D);
                    }
                }
            }
            else
            {
                textureNameList.Clear();
                textureList.Clear();
            }
        }
        
       //If we are not processing a texture draw all of them
        if (textureArray[0] == null)
        {
            scrollPos2 = EditorGUILayout.BeginScrollView(scrollPos2, GUILayout.Height(Math.Min(215,textureList.Count * 21.5f)));
            for (int i = 0; i < textureNameList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(textureNameList[i], GUILayout.Width(100));
                textureList[i] = EditorGUILayout.ObjectField(textureList[i], typeof(Texture2D), true) as Texture2D;
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    currentTexture = textureList[i];
                    listIndex = i;
                    processingTexture = true;
                    CreateTextures();
                    processingTexture = false;

                }
                EditorGUILayout.EndHorizontal();
            }
             EditorGUILayout.EndScrollView();
            alphaUsed = EditorGUILayout.Toggle("Alpha Channel Used", alphaUsed);
        }
       
        else
            currentTexture = EditorGUILayout.ObjectField("Current Texture", currentTexture, typeof(Texture2D), true) as Texture2D; //else show current one select kind of no longer needed


       
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        if (textureArray[0] != null)
        {
            //Buttons to change texture size
            if (whichSize != 0)
                if (GUILayout.Button("Size: " + textureSize))
                {
                    if (whichSize == 1)
                    {

                        textureArrayLocation -= 1;

                    }
                    else
                    {
                        textureArrayLocation -= 2;

                    }
                    SetTexture();
                    whichSize = 0;
                    

                }
            if (whichSize != 1)
                if (GUILayout.Button("Size: " + textureSize / 2))
                {

                    if (whichSize == 0)
                    {

                        textureArrayLocation += 1;

                    }
                    else
                    {
                        textureArrayLocation -= 1;

                    }
                    whichSize = 1;
                    SetTexture();
                    

                }
            if (whichSize != 2)
                if (GUILayout.Button("Size: " + textureSize / 4))
                {

                    if (whichSize == 0)
                    {

                        textureArrayLocation += 2;

                    }
                    else
                    {
                        textureArrayLocation += 1;

                    }
                    whichSize = 2;
                    SetTexture();
                    

                }

            //Button to change compression
            if (GUILayout.Button(whichCompresssion))
            {
                if (whichCompresssion == "DXT")
                {
                    whichCompresssion = "BC";
                    textureArrayLocation += 6;

                }
                else
                {
                    whichCompresssion = "DXT";
                    textureArrayLocation -= 6;

                }
                SetTexture();
                

            }

            //Button to change resizing
            if (GUILayout.Button(whichResize))
            {
                if (whichResize == "Bilinear")
                {
                    whichResize = "Mitchell";
                    textureArrayLocation += 3;

                }
                else
                {
                    whichResize = "Bilinear";
                    textureArrayLocation -= 3;

                }
                SetTexture();
               

            }
           
            //Finish texture testing
            if (GUILayout.Button("Select and Delete Duplicates"))
            {
                for (int i = 0; i < textureArray.Length; i++)
                {
                    if(i != textureArrayLocation)
                    {
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(textureArray[i]));
                    }
                    else
                    {
                        textureList[listIndex] = currentTexture;
                    }
                    textureArray[i] = null;
                }
            }

            //Draw texture varitions
            for (int i = 0; i < textureArray.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                if (i == textureArrayLocation)
                {
                    GUIContent indicator = EditorGUIUtility.IconContent("PlayButton");
                    GUILayout.Label(indicator, GUILayout.Width(20));
                }
                textureArray[i] = EditorGUILayout.ObjectField(textureArray[i], typeof(Texture2D), true) as Texture2D;
                GUILayout.Label("Vram: " + vramSize[i].ToString(), GUILayout.Width(100)); // Adjust the width as needed
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();

    }

    //For making on scene and drawing extra view
    private void OnFocus()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
        SceneView.duringSceneGui += this.OnSceneGUI;
    }
    void OnDestroy()
    {
     
        SceneView.duringSceneGui -= this.OnSceneGUI;
    }
    void OnSceneGUI(SceneView sceneView)
    {
        if (textureArray[0] != null)
        {
            Vector3 newCameraPos = sceneView.camera.transform.position - .5f * (sceneView.camera.transform.forward);
            if (newCameraPos != Camera.main.transform.position)
            {
                //there must be a better way of doing this as this forces window focus
                System.Type gameViewType = System.Type.GetType("UnityEditor.GameView, UnityEditor");

                EditorWindow gameView = EditorWindow.GetWindow(gameViewType);


                if (gameView.position != extraView)
                {
                    extraView = gameView.position;
                    gameView.position = extraView;
                }

                Camera.main.transform.position = newCameraPos;
                Camera.main.transform.rotation = sceneView.camera.transform.rotation;

            }
        }
    }
    private void CreateTextures()
    {
        string currentFolder = ShibaHelpers.GetCurrentFolder();
        
        if (!AssetDatabase.IsValidFolder(currentFolder + "/Test Textures") && Path.GetFileNameWithoutExtension(currentFolder) != "Test Textures")
        {
            AssetDatabase.CreateFolder(currentFolder, "Test Textures");
        }
        if (Path.GetFileNameWithoutExtension(currentFolder) != "Test Textures")
            currentFolder = currentFolder + "/Test Textures";
        string assetPath = AssetDatabase.GetAssetPath(currentTexture);
        textureName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
        for (int i = 0; i < textureArray.Length; i++)
        {
            currentProccessedTexture = i;
            string newAsset = currentFolder + "/new.png";
            Debug.Log(CurrentFolderName(newAsset));

            AssetDatabase.CopyAsset(assetPath, newAsset);
            textureArray[i] = AssetDatabase.LoadAssetAtPath<Texture2D>(newAssetPath);

        }
        textureArrayLocation = 0;
        whichSize = 0;
        whichResize = "Bilinear";
        whichCompresssion = "DXT";
        SetTexture();
        AssetDatabase.Refresh();

    }
    private string CurrentFolderName(string path)
    {
        path = Path.GetFullPath(path);
        DirectoryInfo directoryInfo = new DirectoryInfo(path);
        string lastFolderName = directoryInfo.Name;
        return lastFolderName;
    }
    private void SetTexture()
    {
        currentTexture = textureArray[textureArrayLocation];
        shaderMaterial.SetTexture(textureNameList[listIndex], currentTexture);
    }
    class MyTexturePostprocessor : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if (processingTexture)
            {
                Debug.Log("PROCESSING TEXTURE " + currentProccessedTexture);
                TextureImporter textureImporter = (TextureImporter)assetImporter;
                TextureImporterPlatformSettings textureSettings = textureImporter.GetPlatformTextureSettings("Standalone");
                if (currentProccessedTexture % 3 == 1)
                    textureSettings.maxTextureSize = textureSettings.maxTextureSize / 2;
                else if (currentProccessedTexture % 3 == 2)
                {
                    textureSettings.maxTextureSize = textureSettings.maxTextureSize / 4;
                }
                else
                    textureSize = textureSettings.maxTextureSize;
                vramSize[currentProccessedTexture] = (float)Math.Round((float)(Math.Pow(textureSettings.maxTextureSize, 2.0) / (798915.0)), 1);
                if (currentProccessedTexture < 6)
                    if (textureImporter.textureType == TextureImporterType.NormalMap)
                    {
                        textureSettings.format = TextureImporterFormat.BC5;
                        isNormalMap = true;
                    }
                    else
                        textureSettings.format = TextureImporterFormat.BC7;
                else
                    if (alphaUsed || textureImporter.textureType == TextureImporterType.NormalMap)
                    textureSettings.format = TextureImporterFormat.DXT5;
                else
                {
                    textureSettings.format = TextureImporterFormat.DXT1;
                    vramSize[currentProccessedTexture] = (float)Math.Round((float)(Math.Pow(textureSettings.maxTextureSize, 2.0) / (798915.0 * 2.0)),1);
                }
                
                if(currentProccessedTexture % 6 < 3)
                {
                    textureSettings.resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
                }
                else
                    textureSettings.resizeAlgorithm = TextureResizeAlgorithm.Bilinear;
                textureSettings.overridden = true;
               
                textureImporter.SetPlatformTextureSettings(textureSettings);
                
                newAssetPath = assetImporter.assetPath.Replace("new.png", string.Format("{0} {1} {2} {3}.png", textureName, textureSettings.format, textureSettings.resizeAlgorithm, textureSettings.maxTextureSize));
                AssetDatabase.MoveAsset(assetImporter.assetPath, newAssetPath);
                

            }
        }
    }

}
