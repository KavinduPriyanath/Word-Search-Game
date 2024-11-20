using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public List<string> objectives = new List<string>();
    public Transform headingBox;
    [SerializeField] private GameObject winMenu;
    [SerializeField] private Transform canvasObj;
    public int objectivesLeft = 7;
    public Dictionary<string, int> hints = new Dictionary<string, int>();
    public List<GameObject> cells= new List<GameObject>();
    [SerializeField] private List<Color> hintColors = new List<Color>();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {

            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //LoadHints();
    }

    public void PopulateLetterBox(BoardData boardData)
    {
        Transform headingText = headingBox.Find("Mask").Find("HeadingText");
        headingText.GetChild(0).GetComponent<TMP_Text>().text = boardData.theme.ToString();
        headingText.GetComponent<Image>().color = boardData.headingColor;

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

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadSceneAsync("Menu");
    }

    public IEnumerator OpenWinMenu()
    {
        yield return new WaitForSeconds(0.7f);

        winMenu.SetActive(true);
        PlayerData.Instance.lastPuzzleId += 1;
        //PlayerData.Instance.SaveLastPlayedPuzzle(PlayerData.Instance.lastPuzzleId);
        winMenu.transform.Find("Mask").Find("ThemeImage").GetComponent<Image>().sprite = GameManager.Instance.puzzleSet[PlayerData.Instance.lastPuzzleId].themeImage;
        TMP_Text text = winMenu.transform.Find("Theme Name").GetComponent<TMP_Text>();
        text.text = GameManager.Instance.puzzleSet[PlayerData.Instance.lastPuzzleId].theme.ToString();
        text.color = GameManager.Instance.puzzleSet[PlayerData.Instance.lastPuzzleId].headingColor;
        foreach (Transform obj in canvasObj)
        {
            if (obj.name == "Win Panel" || obj.name == "Theme")
                obj.gameObject.SetActive(true);
            else 
                obj.gameObject.SetActive(false);
        }
    }

    public void ShowHint(int hintNo)
    {
        GameObject hintObj = cells[hints.FirstOrDefault().Value].transform.Find("Hint").gameObject;
        hintObj.SetActive(true);
        //hintObj.GetComponent<Image>().color = hintColors[hintNo-1];
        hints.Remove(hints.FirstOrDefault().Key);
    }

    private void LoadHints()
    {
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
            hints.Add("Cloud", 34);
            hints.Add("Snow", 12);
            hints.Add("Foot", 20);
            hints.Add("Road", 22);
            hints.Add("Far", 42);
            hints.Add("Blue", 25);
            hints.Add("Sky", 2);
        }
        else if (index == 5)
        {
            hints.Add("Home", 0);
            hints.Add("High", 0);
            hints.Add("Moon", 0);
            hints.Add("Dark", 0);
            hints.Add("Calm", 0);
            hints.Add("Owl", 0);
            hints.Add("Eve", 0);
        }
        else if (index == 6)
        {
            hints.Add("Elf", 0);
            hints.Add("Mage", 0);
            hints.Add("Orc", 0);
            hints.Add("Pond", 0);
            hints.Add("Wish", 0);
            hints.Add("Drag", 0);
            hints.Add("Cool", 0);
        }
        else if (index == 7)
        {
            hints.Add("Art", 0);
            hints.Add("Tile", 0);
            hints.Add("Dot", 0);
            hints.Add("Rose", 0);
            hints.Add("Blur", 0);
            hints.Add("Pink", 0);
            hints.Add("Calm", 0);
        }
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
