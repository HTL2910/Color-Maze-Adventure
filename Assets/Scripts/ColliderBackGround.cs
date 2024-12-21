using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderBackGround : MonoBehaviour
{
    public Color newColor;
    public Color currentColor;
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
        Invoke("GetCurrentColor",1f);
    }

    private void GetCurrentColor()
    {
        currentColor = spriteRenderer.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Collided with: {other.name}");

        if (other.CompareTag("Player") )
        {
            UIManager.Instance.RandomAudio();
            if (isActive == 1)
            {
                spriteRenderer.color = Color.white;
                spriteRenderer.color = newColor;
                isActive = 0;
                if (create.CheckCount())
                {
                    PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);
                    PlayerPrefs.Save();
                    UIManager.Instance.WinGame();
                    StartCoroutine(NextGame());
                }
            }
            
        }
        if (other.CompareTag("Boss"))
        {
            Debug.Log("Boss collider");
            StartCoroutine(ColliderBoss(4f));
        }
    }
    IEnumerator ColliderBoss(float time)
    {
        yield return new WaitForSeconds(time);
        isActive = 1;
        spriteRenderer.color = currentColor;
    }

    IEnumerator NextGame()
    {
        yield return new WaitForSeconds(1f);
        UIManager.Instance.Play();
    }
   
}
