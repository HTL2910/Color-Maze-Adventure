using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] TextMeshProUGUI levelText;
    public List<AudioClip> audioList=new List<AudioClip>();
    public GameObject settingPanel;
    public GameObject startPanel;
    public GameObject losePanel;
    public Slider audioSlider;
    public AudioSource audioSource;
    public AudioClip winGame;
    public AudioClip background;
    public AudioClip loseGame;
    public bool isLose=false;
    private void Start()
    {
        levelText.text = "Level "+GameManager.Instance.level.ToString();
        
        audioSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        audioSlider.value = PlayerPrefs.GetFloat("volume", 0.5f);
        StartCoroutine(StartGame(1f));
    }
    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Setting()
    {
        settingPanel.gameObject.SetActive(true);
    }
    public void CloseSetting()
    {
        settingPanel.gameObject.SetActive(false);
    }
    void ValueChangeCheck()
    {
        audioSource.volume=audioSlider.value;
        PlayerPrefs.SetFloat("volume", audioSlider.value);
        PlayerPrefs.Save();
    }
    public void WinGame()
    {
        audioSource.PlayOneShot(winGame);
    } 
    
    IEnumerator StartGame(float time)
    {
        yield return new WaitForSeconds(time);
        startPanel.SetActive(false);
    }
    public void RandomAudio()
    {
        int index = Random.Range(0, audioList.Count);
        AudioClip clip = audioList[index];
        audioSource.PlayOneShot(clip);
    }
    public void LoseGame()
    {

        audioSource.PlayOneShot(loseGame);
        StartCoroutine(Lose(1f));
        isLose = true;
    }
    IEnumerator Lose(float time)
    {
        yield return new WaitForSeconds(time);
        losePanel.SetActive(true);
    }
    public void RestartGame()
    {
        losePanel.SetActive(false);
        Play();
    }
}
