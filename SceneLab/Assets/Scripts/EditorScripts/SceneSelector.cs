using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Linq;


public static class SceneSelector
{
    [MenuItem("Scenes/Lobby")]///Adding attributes, a DIY window in the interface

    static void OpenLobby()
    {
        Load("Lobby");
    }

    [MenuItem("Scenes/GreenPlace")]

    static void OpenGreenPlace()
    {
        Load("GreenPlace");
    }


    [MenuItem("Scenes/RedPlace")]

    static void OpenRedPlace()
    {
        Load("RedPlace");
     }


    static void Load(string scene)
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();///prompt 

        ////store the file containing the scenes
        Scene xrScene = EditorSceneManager.OpenScene("Assets/Scenes/XR.unity", OpenSceneMode.Single);///first load XR scene, then load scenes on top of it
        Scene newScene = EditorSceneManager.OpenScene("Assets/Scenes/" + scene + ".unity", OpenSceneMode.Additive);

        XRSceneTransitionManager. PlaceXRRig(xrScene, newScene);//run the PlaceXRRig function
    }

    static public void PlaceXRRig(Scene xrScene, Scene newScene)
    {
        GameObject[] xrObjects = xrScene.GetRootGameObjects();// What is root object??
        GameObject[] newSceneObjects = newScene.GetRootGameObjects();


        ////Get an object(xrRig)(the tag) here everytime we search for an array
        GameObject xrRig = xrObjects.First((obj) => { return obj.CompareTag("XRRig"); }); ///Lamda function ????
        GameObject xrRigOrigin = newSceneObjects.First((obj) => { return obj.CompareTag("XRRigOrigin"); });

        if(xrRig && xrRigOrigin) ///if both of them are found, switch scenes
        {
            xrRig.transform.position = xrRigOrigin.transform.position;
            xrRig.transform.rotation = xrRigOrigin.transform.rotation;
        }

    }
}
