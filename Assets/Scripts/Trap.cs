using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && UIManager.Instance.isLose==false)
        {
            UIManager.Instance.LoseGame();
        
        }
    }
    
}
