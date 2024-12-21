using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DestroyTileNew : MonoBehaviour
{
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(DestroyObject(0.5f));
        }
    }
    IEnumerator DestroyObject(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }
   
}
