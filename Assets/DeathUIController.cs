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
        m_DeathUi.SetActive(false);
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
        m_DeathUi.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        m_DeathUi.SetActive(false);
    }

}