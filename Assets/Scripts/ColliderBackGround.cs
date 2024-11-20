using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderBackGround : MonoBehaviour
{
    [SerializeField] Color newColor;
    [SerializeField] SpriteRenderer spriteRenderer;
    public int isActive=1;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isActive == 1)
        {
            spriteRenderer.color = newColor;
            isActive = 0;
        }
    }
}
