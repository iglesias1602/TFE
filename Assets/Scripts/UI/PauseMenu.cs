using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
    public string mainMenu;
    public GameObject pauseMenu;
    public GameObject mainCanvas;
    [SerializeField] private FirstPersonCam firstPersonCam;
    public bool isPaused;

    private void Start()
    {
        pauseMenu.SetActive(false);
        if (mainCanvas != null) mainCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        if (mainCanvas != null) mainCanvas.SetActive(false); // Hide the main canvas
        //Time.timeScale = 0f;
        isPaused = true;
        UnlockCursorAndDisableCamera();
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        if (mainCanvas != null) mainCanvas.SetActive(true); // Show the main canvas
        //Time.timeScale = 1f;
        isPaused = false;
        LockCursorAndEnableCamera();
    }
    public void LeaveGame()
    {
        SceneManager.LoadScene(mainMenu);
    }

    private void UnlockCursorAndDisableCamera()
    {
        // Disable camera movement
        firstPersonCam.enabled = false;

        // Unlock and show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void LockCursorAndEnableCamera()
    {
        // Enable camera movement
        firstPersonCam.enabled = true;

        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
