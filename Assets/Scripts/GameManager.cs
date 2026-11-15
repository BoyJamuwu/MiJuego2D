using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject panelGameOver;
    private bool juegoTerminado;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        if (panelGameOver != null) panelGameOver.SetActive(false);
    }

    void Update()
    {
        if (juegoTerminado && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void GameOver()
    {
        if (juegoTerminado) return;
        juegoTerminado = true;
        if (panelGameOver != null) panelGameOver.SetActive(true);
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }
}