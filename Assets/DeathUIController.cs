using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathUIController : MonoBehaviour
{

    // Start is called before the first frame update
    public GameObject m_DeathUi;

    void Start()
    {
        HideUI();
        
    }
    private void OnDestroy()
    {

    }
    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        MarioController.OnPlayerDeath += DisplayUI;
    }

    private void OnDisable()
    {
        MarioController.OnPlayerDeath -= DisplayUI;
    }

    public void DisplayUI()
    {
        Cursor.lockState = CursorLockMode.None;
        m_DeathUi.SetActive(true);
        if (MarioController.m_vidasInt == 0)
        {
            
        }
            
    }

    public void RestartGame()
    {
        if (MarioController.m_vidasInt > 0)
        {
            HideUI();
            Debug.Log("RESTART GAME");
            MarioController.m_CharacterController.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            MarioController.m_AudioSource.Play();
        }

    }
    public void ExitGameButton()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    void HideUI()
    {
        m_DeathUi.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

}
