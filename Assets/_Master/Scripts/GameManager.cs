using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public LevelManager levelManager;
    public int currentLevelIndex = 0;
    public Level CurrentLevel => levelManager.GetLevel(currentLevelIndex);
    public int totalLevel=10;
    public int countHeart=3;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        Debug.Log(CurrentLevel.background);
        UIManager.instance.SetFlagImage(CurrentLevel.background);
        Debug.Log(UIManager.instance.flagObject.GetComponent<Image>().sprite);
        Invoke(nameof(FlagActive), 2f);
        LoadCurrentLevelData();
    }
    private void FlagActive()
    {
        UIManager.instance.flagObject.SetActive(false);
    }
    public void LoadCurrentLevelData()
    {
        // Tải level hiện tại từ DataManager (nếu có)
        currentLevelIndex = DataManager.LoadInt("CurrentLevel", 0);
    }

    public void SetLevel(int index)
    {
        currentLevelIndex = index;
        DataManager.SaveInt("CurrentLevel", index);
    }
    public void NextLevel()
    {
        currentLevelIndex++;
        DataManager.SaveInt("CurrentLevel", currentLevelIndex);
        UIManager.instance.NextLevel();
    }
}
