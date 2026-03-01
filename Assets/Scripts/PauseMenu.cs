using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    private bool isPaused = false;

    [Header("UI")]
    public GameObject pauseMenu;

    [Header("Music Sources")]
    public AudioSource[] musicSources;

    [Header("Sound Effects Sources")]
    public AudioSource[] soundSources;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }



    void Start()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }


    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoBack()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ToggleMusic()
    {
        foreach (AudioSource music in musicSources)
            music.mute = !music.mute;
    }

    public void ToggleSound()
    {
        foreach (AudioSource sound in soundSources)
            sound.mute = !sound.mute;
    }
}
