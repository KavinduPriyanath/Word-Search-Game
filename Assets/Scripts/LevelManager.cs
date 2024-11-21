using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject winMenu;
    [SerializeField] private Transform canvasObj;

    [Header("Shared Data")]
    public List<string> objectives = new List<string>();
    public Dictionary<string, int> hints = new Dictionary<string, int>();
    public List<GameObject> cells= new List<GameObject>();
    public Transform headingBox;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {

            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadHints();
    }

    public void PopulateLetterBox(BoardData boardData)
    {
        Transform headingText = headingBox.Find("Mask").Find("HeadingText");
        headingText.GetChild(0).GetComponent<TMP_Text>().text = boardData.theme.ToString();
        headingText.GetComponent<Image>().color = boardData.headingColor;
        canvasObj.Find("Theme").GetComponent<Image>().sprite = boardData.themeImage;

        Transform letterBox = headingBox.Find("LetterBox");

        for (int i=0; i < objectives.Count; i++)
        {
            if (i < 4)
            {
                letterBox.Find("Row_1").GetChild(i).GetComponent<TMP_Text>().text = objectives[i].ToUpper();
            }
            else
            {
                letterBox.Find("Row_2").GetChild(i-4).GetComponent<TMP_Text>().text = objectives[i].ToUpper();
            }
        }
    }

    //Functions for buttons
    public void RestartLevel()
    {
        StartCoroutine(LoadSceneName("Game", 0.1f));
    }

    public void ReturnToMenu()
    {
        StartCoroutine(LoadSceneName("Menu", 0.1f));
    }

    public void NextLevel()
    {
        StartCoroutine(LoadSceneName("Game", 0.1f));
    }

    private IEnumerator LoadSceneName(string name, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadSceneAsync(name);
    }

    public IEnumerator OpenWinMenu()
    {
        yield return new WaitForSeconds(0.7f);

        SoundManager.Instance.PlayClip(SoundManager.Instance.victorySound);

        winMenu.SetActive(true);
        int puzzleId = (PlayerData.Instance.lastPuzzleId < 6) ? PlayerData.Instance.lastPuzzleId += 1 : 0;
        PlayerData.Instance.SavePuzzleData(PlayerData.Instance.lastPuzzleId, GameManager.Instance.hintsCount);

        winMenu.transform.Find("Mask").Find("ThemeImage").GetComponent<Image>().sprite = GameManager.Instance.puzzleSet[puzzleId].themeImage;
        TMP_Text text = winMenu.transform.Find("Theme Name").GetComponent<TMP_Text>();
        text.text = GameManager.Instance.puzzleSet[puzzleId].theme.ToString();
        text.color = GameManager.Instance.puzzleSet[puzzleId].headingColor;

        foreach (Transform obj in canvasObj)
        {
            if (obj.name == "Win Panel" || obj.name == "Theme")
                obj.gameObject.SetActive(true);
            else 
                obj.gameObject.SetActive(false);
        }
    }


    private void LoadHints()
    {
        GameManager.Instance.hintsCount = PlayerData.Instance.LoadPuzzleData(false);
        GameManager.Instance.ActiveHints();
        int index = PlayerData.Instance.lastPuzzleId + 1;
        if (index == 1)
        {
            hints.Add("Map", 10);
            hints.Add("Quest", 29);
            hints.Add("Hero", 44);
            hints.Add("Camp", 13);
            hints.Add("Palace", 0);
            hints.Add("Rock", 2);
            hints.Add("Sword", 40);
        }
        else if (index == 2)
        {
            hints.Add("Tree", 5);
            hints.Add("Water", 6);
            hints.Add("River", 35);
            hints.Add("Rock", 7);
            hints.Add("Sun", 22);
            hints.Add("Dawn", 25);
            hints.Add("Cloud", 40);
        }
        else if (index == 3)
        {
            hints.Add("Sky", 34);
            hints.Add("Dusk", 26);
            hints.Add("Sun", 17);
            hints.Add("Glow", 22);
            hints.Add("Red", 7);
            hints.Add("Calm", 54);
            hints.Add("Haze", 1);
        }
        else if (index == 4)
        {
            hints.Add("Cloud", 31);
            hints.Add("Snow", 12);
            hints.Add("Foot", 23);
            hints.Add("Road", 22);
            hints.Add("Far", 42);
            hints.Add("Blue", 25);
            hints.Add("Sky", 2);
        }
        else if (index == 5)
        {
            hints.Add("Home", 8);
            hints.Add("High", 30);
            hints.Add("Moon", 19);
            hints.Add("Dark", 13);
            hints.Add("Calm", 5);
            hints.Add("Owl", 42);
            hints.Add("Eve", 39);
        }
        else if (index == 6)
        {
            hints.Add("Elf", 48);
            hints.Add("Mage", 21);
            hints.Add("Orc", 42);
            hints.Add("Pond", 5);
            hints.Add("Wish", 41);
            hints.Add("Drag", 30);
            hints.Add("Cool", 4);
        }
        else if (index == 7)
        {
            hints.Add("Art", 34);
            hints.Add("Tile", 7);
            hints.Add("Dot", 36);
            hints.Add("Rose", 6);
            hints.Add("Blur", 26);
            hints.Add("Pink", 37);
            hints.Add("Calm", 4);
        }
    }
}
