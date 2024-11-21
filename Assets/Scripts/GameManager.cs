using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject highlighter;
    [SerializeField] private WordDetector wordDetector;
    [SerializeField] private GridCustomizer gridCustomizer;
    [SerializeField] private GameObject hintButton;
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private GameObject blur;

    [Header("Shared Data")]
    public List<Color> colors = new List<Color>();
    public List<BoardData> puzzleSet = new List<BoardData>();
    [HideInInspector] public int colorIndex;
    [HideInInspector] public int hintsCount = 3;
    [HideInInspector] public int objectivesLeft = 7;

    private int tempIndex = -1;
    private Transform hintCount;
    private Transform hintAds;

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
        hintCount = hintButton.transform.Find("Count");
        hintAds = hintButton.transform.Find("Ads");

        ShuffleColors();
        highlighter.SetActive(false);
        if (PlayerData.Instance.lastPuzzleId >= 6) PlayerData.Instance.lastPuzzleId = 0;
        gridCustomizer.GeneratePuzzle(puzzleSet[PlayerData.Instance.lastPuzzleId]);

    }

    public void ActiveHints()
    {
        if (hintsCount > 0)
        {
            hintCount.gameObject.SetActive(true);
            hintAds.gameObject.SetActive(false);
        }
        else
        {
            hintCount.gameObject.SetActive(false);
            hintAds.gameObject.SetActive(true);
        }
        hintCount.GetChild(0).gameObject.GetComponent<TMP_Text>().text = hintsCount.ToString();
    }

    public void UseHint()
    {
        if (hintsCount > 0)
        {
            ShowHint();
            hintsCount--;
            PlayerData.Instance.SavePuzzleData(PlayerData.Instance.lastPuzzleId, hintsCount);
            hintCount.GetChild(0).gameObject.GetComponent<TMP_Text>().text = hintsCount.ToString();
            if (hintsCount == 0)
            {
                hintCount.gameObject.SetActive(false);
                hintAds.gameObject.SetActive(true);
            }
        }
        else
        {
            hintPanel.SetActive(true);
            blur.SetActive(true);
        }
    }

    private void ShowHint()
    {
        if (LevelManager.Instance.hints.Count > 0)
        {
            GameObject hintObj = LevelManager.Instance.cells[LevelManager.Instance.hints.First().Value].transform.Find("Hint").gameObject;
            hintObj.SetActive(true);
            LevelManager.Instance.hints.Remove(LevelManager.Instance.hints.First().Key);
        }
    }

    public void HighlightLetterBanner(string letter)
    {
        highlighter.GetComponent<Image>().color = colors[colorIndex];
        highlighter.SetActive(true);
        GameObject highlightedWord = highlighter.transform.Find("Text").gameObject;
        TMP_Text tmpText = highlightedWord.GetComponent<TMP_Text>();

        tmpText.text += " " + letter;

        float newWidth = 30f;

        RectTransform rectTransform = highlighter.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.sizeDelta += new Vector2(newWidth, 0);
        }
    }

    public bool CheckWord(List<GameObject> selectedCells)
    {
        string word = "";
        foreach (GameObject cell in selectedCells)
        {
            word += cell.GetComponent<TMP_Text>().text;
        }

        for (int i=0; i < LevelManager.Instance.objectives.Count; i++)
        {
            string objective = LevelManager.Instance.objectives[i];
            if (objective.ToUpper() == word.ToUpper())
            {
                tempIndex = i;
                objectivesLeft--;
                LevelManager.Instance.hints.Remove(objective);
                return true;
            }
        }

        tempIndex = -1;
        return false;
    }

    public void SelectedEffect()
    {
        Transform box = LevelManager.Instance.headingBox.Find("LetterBox");
        GameObject textBox;
        if (tempIndex <= 3)
            textBox = box.Find("Row_1").GetChild(tempIndex).gameObject;
        else
            textBox = box.Find("Row_2").GetChild(tempIndex - 4).gameObject;


        Vector3 endPosition = textBox.transform.position;
        Vector3[] letterPositions = GetLetterPositions(textBox.GetComponent<TMP_Text>());

        tempIndex = -1;

        for (int i=0; i < wordDetector.selectedCells.Count; i++)
        {
            GameObject cell = wordDetector.selectedCells[i];
            if (cell != null)
            {
                GameObject newLetter = Instantiate(cell, cell.transform.position, cell.transform.rotation);
                newLetter.transform.parent = wordDetector.grid;
                StartCoroutine(MoveLetters(newLetter, letterPositions[i], 0.5f));
            }
        }

        textBox.GetComponent<TMP_Text>().color = Color.gray;

        if (GameManager.Instance.objectivesLeft == 0)
            StartCoroutine(LevelManager.Instance.OpenWinMenu());
    }

    private Vector3[] GetLetterPositions(TMP_Text textComponent)
    {
        textComponent.ForceMeshUpdate();

        TMP_TextInfo textInfo = textComponent.textInfo;
        int characterCount = textInfo.characterCount;

        Vector3[] letterPositions = new Vector3[characterCount];

        for (int i = 0; i < characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
                continue;

            Vector3 bottomLeft = charInfo.bottomLeft;
            Vector3 topRight = charInfo.topRight;
            letterPositions[i] = (bottomLeft + topRight) / 2f;

            letterPositions[i] = textComponent.transform.TransformPoint(letterPositions[i]);
        }

        Debug.Log("Letter positions retrieved.");
        return letterPositions;
    }

    private IEnumerator MoveLetters(GameObject letter, Vector3 endPos, float duration)
    {
        if (letter == null) yield break;

        Vector3 startPos = letter.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {

            float t = elapsedTime / duration;
            letter.transform.position = Vector3.Lerp(startPos, endPos, t);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        letter.transform.position = endPos;
        letter.gameObject.SetActive(false);   
    }

    public void ResetHighlighter()
    {
        highlighter.transform.Find("Text").gameObject.GetComponent<TMP_Text>().text = "";
        highlighter.GetComponent<RectTransform>().sizeDelta = new Vector2(64, 64);
        highlighter.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -660);
        highlighter.SetActive(false);
    }

    public void GetRewarded()
    {
        hintsCount += 3;
        hintCount.gameObject.SetActive(true);
        hintAds.gameObject.SetActive(false);
        hintCount.GetChild(0).gameObject.GetComponent<TMP_Text>().text = hintsCount.ToString();
    }

    public void ShuffleColors()
    {
        for (int i = colors.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1); 
            Color temp = colors[i];                
            colors[i] = colors[randomIndex];
            colors[randomIndex] = temp;
        }
    }
}

public enum Themes
{
    Adventure,
    Dawn,
    Sunset,
    Snow,
    Night,
    Fantasy,
    Pixel
}
