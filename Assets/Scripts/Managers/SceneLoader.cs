using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Animator Fade;

    //Loads scene transition
    public void SceneTransition(string Scene)
    {
        StartCoroutine(LoadingScene(Scene, "Black"));
    }

    //Loads scene non-additively
    IEnumerator LoadingScene(string Scene, string Transition)
    {
        Debug.Log("Transitioning to Scene: " + Scene);
        switch (Transition)
        {
            case "White":
                Fade.SetTrigger("FadeToWhite");
                break;
            case "Black":
                Fade.SetTrigger("FadeToBlack");
                break;
        }
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(Scene);
    }

    //Loads scene additively
    IEnumerator LoadingSceneAdditive(string Scene, string Scene2, string Transition)
    {
        Debug.Log("Transitioning to Scene: " + Scene);
        switch (Transition)
        {
            case "White":
                Fade.SetTrigger("FadeToWhite");
                break;
            case "Black":
                Fade.SetTrigger("FadeToBlack");
                break;
        }
        yield return new WaitForSeconds(2);
        Fade.SetTrigger("FadeDefault");
        SceneManager.UnloadSceneAsync(Scene2);
        SceneManager.LoadScene(Scene, LoadSceneMode.Additive);
    }


}
