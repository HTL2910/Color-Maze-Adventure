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
    public GameObject settingPanel;
    public GameObject startPanel;
    public Slider audioSlider;
    public AudioSource audioSource;
    public AudioClip winGame;
    public AudioClip background;
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
}
