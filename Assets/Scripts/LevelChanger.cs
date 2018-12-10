using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour {

    public Animator animator;
    string levelToLoad;

	public void FadeToLevel (string levelName)
    {
        animator.SetTrigger("FadeOut");
        levelToLoad = levelName;
    }

    public void OnFadeComplete()
    {
        Time.timeScale = 1;
        if (levelToLoad == "Quit")
        {
            Application.Quit();
        }
        else
        {
            SceneManager.LoadScene(levelToLoad);
        }
    }
}
