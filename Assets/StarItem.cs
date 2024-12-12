using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarItem : MonoBehaviour,IRestartGameElement
{
    private void Start()
    {
        GameManager.GetGameManager().AddRestartGameElement(this);
    }

    public void RestartGame()
    {
        gameObject.SetActive(true);
    }
}
