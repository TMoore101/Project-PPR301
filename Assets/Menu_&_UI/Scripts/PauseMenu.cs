using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]GameObject pauseMenu;
    private bool isPaused;
    [SerializeField] private Player_CameraController cameraController;

    // Input variables
    private Player_InputActions input;

    //== On Start
    private void Start()
    {
        // Get input actions
        input = InputHandler.instance.input;
    }

    public void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        cameraController.inControl = false;
        isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void Home()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        cameraController.inControl = true;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
    
    }

    public void Options()
    {
        //todo later
        Debug.Log("TODO for final implementation");
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    //== On Update
    private void Update()
    {
        if (input.Player.Pause.WasPressedThisFrame())
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }
}
