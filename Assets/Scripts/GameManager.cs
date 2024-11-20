using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    public List<Color> colors = new List<Color>();
    [SerializeField] private GameObject highlighter;
    [SerializeField] private WordDetector wordDetector;
    [SerializeField] private GridCustomizer gridCustomizer;
    public int colorIndex;

    public int hintsCount = 3;
    [SerializeField] private GameObject hintButton;
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private GameObject blur;

    public List<Sprite> themes = new List<Sprite>();
    
    public List<BoardData> puzzleSet = new List<BoardData>();

    public int tempIndex = -1;

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
        ShuffleColors();
        highlighter.SetActive(false);
        gridCustomizer.GeneratePuzzle(puzzleSet[PlayerData.Instance.lastPuzzleId]);

    }

    private void Update()
    {
    }

    public void UseHint()
    {
        if (hintsCount > 0)
        {
            LevelManager.Instance.ShowHint(hintsCount);
            hintsCount--;
            hintButton.transform.Find("Count").GetChild(0).gameObject.GetComponent<TMP_Text>().text = hintsCount.ToString();
            if (hintsCount == 0)
            {
                hintButton.transform.Find("Count").gameObject.SetActive(false);
                hintButton.transform.Find("Ads").gameObject.SetActive(true);
            }
            //ShowHint
        }
        else
        {
            //ShowAd Popup
            hintPanel.SetActive(true);
            blur.SetActive(true);
        }
    }

    public void HighlightLetterBanner(string letter)
    {
        highlighter.GetComponent<Image>().color = colors[colorIndex];
        highlighter.SetActive(true);
        GameObject highlightedWord = highlighter.transform.Find("Text").gameObject;
        TMP_Text tmpText = highlightedWord.GetComponent<TMP_Text>();

        tmpText.text += " " + letter;

        //float newWidth = tmpText.preferredWidth / 2;
        float newWidth = 25f;

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
                LevelManager.Instance.objectivesLeft--;
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
        Debug.Log(letterPositions.Length + " letters && " + wordDetector.selectedCells.Count + " cells");

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

        //LevelManager.Instance.objectives.RemoveAt(tempIndex);
        textBox.GetComponent<TMP_Text>().color = Color.gray;

        if (LevelManager.Instance.objectivesLeft == 0)
            StartCoroutine(LevelManager.Instance.OpenWinMenu());
    }

    private Vector3[] GetLetterPositions(TMP_Text textComponent)
    {
        // Force text to generate geometry
        textComponent.ForceMeshUpdate();

        // Get the text info
        TMP_TextInfo textInfo = textComponent.textInfo;
        int characterCount = textInfo.characterCount;

        // Initialize the array to store positions
        Vector3[] letterPositions = new Vector3[characterCount];

        for (int i = 0; i < characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            // Only consider visible characters
            if (!charInfo.isVisible)
                continue;

            // Get the center position of the character
            Vector3 bottomLeft = charInfo.bottomLeft;
            Vector3 topRight = charInfo.topRight;
            letterPositions[i] = (bottomLeft + topRight) / 2f;

            // Convert from local space to world space
            letterPositions[i] = textComponent.transform.TransformPoint(letterPositions[i]);
        }

        // Log or use the positions as needed
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
            // Calculate the fraction of time passed
            float t = elapsedTime / duration;

            // Move the object smoothly
            letter.transform.position = Vector3.Lerp(startPos, endPos, t);

            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Ensure the object ends exactly at the target position
        letter.transform.position = endPos;
        letter.gameObject.SetActive(false);   
    }

    public void ResetHighlighter()
    {
        highlighter.transform.Find("Text").gameObject.GetComponent<TMP_Text>().text = "";
        highlighter.GetComponent<RectTransform>().sizeDelta = new Vector2(64, 64);
        highlighter.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -820);
        highlighter.SetActive(false);
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
