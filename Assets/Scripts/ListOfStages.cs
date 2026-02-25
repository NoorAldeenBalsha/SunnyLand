using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ListOfStages : MonoBehaviour
{
    public void GoBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void GoToFirst()
    {
        StartCoroutine(LoadAfterDelay("First"));
    }
    IEnumerator LoadAfterDelay(string sceneName)
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneName);
    }
}
