using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathUIController : MonoBehaviour
{
    static public Animator m_Animator;
    // Start is called before the first frame update
    public GameObject m_DeathUi;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_DeathUi.SetActive(false);
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
        m_Animator.SetBool("IsDead", false);
        Time.timeScale = 1f;
        m_DeathUi.SetActive(false);

    }

}
