using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

[DisallowMultipleComponent]

public class XRSceneTransitionManager : MonoBehaviour
{
    public static XRSceneTransitionManager Instance;

    /////auto change to the initial scene
    public Material transitionMaterial;
    public float transitionSpeed = 1.0f;
    public string initialScene;

    public bool isLoading { get; private set; } = false;


    Scene xrScene;
    Scene currentScene;
    float currentTransitionAmount = 0.0f;


    private void Awake()///the very fist thing happen in the whole application
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Deteced rouge XRSceneTransitionManager.  Deleting it.");
            Destroy(this.gameObject);
            return;
        }

        xrScene = SceneManager.GetActiveScene();
        SceneManager.sceneLoaded += OnNewSceneAdded;///an event for scene loaded

        ///check if we are inside the editor
        if (!Application.isEditor)
        {
            TransitionTo(initialScene);
        }


    }

    ///A public function to do most of the work
    ///
    public void TransitionTo(string scene)
    {///if not laoding, we can do the transition
        if (!isLoading)
        {
            StartCoroutine(Load(name));
        }

    }

    void OnNewSceneAdded(Scene newScene, LoadSceneMode mode)
    {
        if(newScene != xrScene)
        {
            currentScene = newScene;
            SceneManager.SetActiveScene(currentScene);
            PlaceXRRig(xrScene, currentScene);
        }
            
    }

    IEnumerator Load(string scene)
    {
        isLoading = true;

        yield return StartCoroutine(Fade(1.0f));
        yield return StartCoroutine(UnloadCurrentScene());

        yield return StartCoroutine(LoadNewScene(scene));///yield return v.s.return?
        yield return StartCoroutine(Fade(0.0f));




        isLoading = false;
    }

    IEnumerator UnloadCurrentScene()
    {
        AsyncOperation unload = SceneManager.UnloadSceneAsync(currentScene);
        while (!unload.isDone)
        {
            yield return null;
        }////check everyframe until its loaded
    }

    IEnumerator LoadNewScene(string scene)
    {
        AsyncOperation load = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

        while (!load.isDone)
        {
            yield return null;
        }
    }

    static public void PlaceXRRig(Scene xrScene, Scene newScene)
    {
        GameObject[] xrObjects = xrScene.GetRootGameObjects();// What is root object??
        GameObject[] newSceneObjects = newScene.GetRootGameObjects();


        ////Get an object(xrRig)(the tag) here everytime we search for an array
        GameObject xrRig = xrObjects.First((obj) => { return obj.CompareTag("XRRig"); }); ///Lamda function ????
        GameObject xrRigOrigin = newSceneObjects.First((obj) => { return obj.CompareTag("XRRigOrigin"); });

        if (xrRig && xrRigOrigin) ///if both of them are found, switch scenes
        {
            xrRig.transform.position = xrRigOrigin.transform.position;
            xrRig.transform.rotation = xrRigOrigin.transform.rotation;
        }

    }

    IEnumerator Fade(float target)
    {
        while (!Mathf.Approximately(currentTransitionAmount, target))
        {
            currentTransitionAmount = Mathf.MoveTowards(currentTransitionAmount, target, transitionSpeed * Time.deltaTime);
            transitionMaterial.SetFloat("_FadeAmount", currentTransitionAmount);
            yield return null;
        }

        ///make sure the transition material is at the target, AN INSURANCE
        transitionMaterial.SetFloat("_FadeAmount", target);
    }
}

