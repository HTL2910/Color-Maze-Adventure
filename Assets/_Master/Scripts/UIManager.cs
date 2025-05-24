using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [Header("UI")]
    public GameObject flagObject;
    public Image flagImage;
    public GameObject statusPanel;
    public GameObject winObject;
    public GameObject loseObject;
    private void Start()
    {
        instance = this;
    }
    public void WinGame()
    {
        statusPanel.SetActive(true);
        winObject.SetActive(true);
        loseObject.SetActive(false);
    }
    public void LoseGame()
    {
        statusPanel.SetActive(true);
        winObject.SetActive(false);
        loseObject.SetActive(true);
    }
    public void PlayAgain()
    {
        statusPanel.SetActive(false);
        winObject.SetActive(false);
        loseObject.SetActive(false);
    }
    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void SetFlagImage(Sprite sprite)
    {
        flagImage.sprite = sprite;
    }
}
