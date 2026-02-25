using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{


    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoToListOfStages()
    {
        StartCoroutine(LoadAfterDelay("List"));
    }

    IEnumerator LoadAfterDelay(string sceneName)
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName);
    }

}
