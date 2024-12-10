using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaZone : MonoBehaviour,IRestartGameElement
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //MarioController.m_Animator.SetBool("IsDead", true);
            //RestartGame();
        }
    }
    public void RestartGame()
    {

    }
}