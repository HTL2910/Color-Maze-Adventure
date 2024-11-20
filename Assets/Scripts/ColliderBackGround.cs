using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderBackGround : MonoBehaviour
{
    [SerializeField] Color newColor;
    [SerializeField] SpriteRenderer spriteRenderer;
    public int isActive = 1;
    private CreateMap create;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        create = FindObjectOfType<CreateMap>();
    }
    private void Start()
    {
       
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isActive == 1)
        {
            spriteRenderer.color = newColor;
            isActive = 0;
            if (create.CheckCount())
            {
                PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) +1);
                PlayerPrefs.Save();
                UIManager.Instance.WinGame();
                StartCoroutine(NextGame());
            }
        }
    }


    IEnumerator NextGame()
    {
        yield return new WaitForSeconds(1f);
        UIManager.Instance.Play();
    }
}
