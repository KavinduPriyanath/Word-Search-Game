using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEditor;
using TMPro;

public class WordDetector : MonoBehaviour
{
    public List<GameObject> selectedCells = new List<GameObject>();

    [Header("References")]
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject highlighterPrefab;
    public Transform grid;

    private GameObject currentHighlighter = null; 
    private Vector2 previousMousePosition;
    private Vector2 startMousePosition;
    private Vector2 startSize;
    private bool isDragging = false;


    void Update()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            isDragging = true;
            selectedCells.Clear();
            currentHighlighter = null;
            previousMousePosition = (Input.touchCount > 0) ? Input.GetTouch(0).position : Input.mousePosition;
            startMousePosition = previousMousePosition;
        }

        if (isDragging && (Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)))
        {
            Vector2 currentPosition = (Input.touchCount > 0) ? Input.GetTouch(0).position : Input.mousePosition;
            Vector2 mouseDelta = currentPosition - previousMousePosition;

            PointerEventData pointerData = new PointerEventData(eventSystem)
            {
                position = currentPosition
            };

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            graphicRaycaster.Raycast(pointerData, raycastResults);

            foreach (RaycastResult result in raycastResults)
            {
                GameObject hitObject = result.gameObject;
                if (result.gameObject.CompareTag("Letter"))
                {
                    if (!selectedCells.Contains(hitObject))
                    {
                        selectedCells.Add(hitObject);
                        SoundManager.Instance.PlayClip(SoundManager.Instance.letterPickSound);

                        if (currentHighlighter == null)
                        {
                            GameObject highlighter = Instantiate(highlighterPrefab, hitObject.transform.position, Quaternion.identity);
                            highlighter.GetComponent<Image>().color = GameManager.Instance.colors[GameManager.Instance.colorIndex];
                            highlighter.transform.SetParent(grid);
                            highlighter.SetActive(true);
                            highlighter.transform.SetAsFirstSibling();
                            startSize = highlighter.transform.localScale;
                            currentHighlighter = highlighter;
                        }

                        GameManager.Instance.HighlightLetterBanner(hitObject.GetComponent<TMP_Text>().text);
                    }                    
                }
            }

            if (currentHighlighter != null) 
            {
                RectTransform rectTransform = currentHighlighter.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    if (Mathf.Abs(mouseDelta.x) > Mathf.Abs(mouseDelta.y))
                    {
                        float widthChange = Mathf.Abs(mouseDelta.x) * 1f;

                        if (mouseDelta.x > 0)
                        {
                            if (currentPosition.x > startMousePosition.x)
                                rectTransform.sizeDelta += new Vector2(widthChange, 0);
                            else
                                rectTransform.sizeDelta -= new Vector2(widthChange, 0);

                            rectTransform.anchoredPosition += new Vector2(widthChange / 2, 0);
                            //might need to change the widthchange by some value depending on the width
                        }
                        else
                        {
                            if (currentPosition.x < startMousePosition.x)
                                rectTransform.sizeDelta += new Vector2(widthChange, 0);
                            else
                                rectTransform.sizeDelta -= new Vector2(widthChange, 0);

                            rectTransform.anchoredPosition -= new Vector2(widthChange / 2, 0);

                        }
                    }
                    else if (Mathf.Abs(mouseDelta.y) > Mathf.Abs(mouseDelta.x))
                    {
                        float heightChange = Mathf.Abs(mouseDelta.y) * 1f;

                        if (mouseDelta.y > 0)
                        {
                            if (currentPosition.y > startMousePosition.y)
                                rectTransform.sizeDelta += new Vector2(0, heightChange);
                            else
                                rectTransform.sizeDelta -= new Vector2(0, heightChange);

                            rectTransform.anchoredPosition += new Vector2(0, heightChange / 2);
                        }
                        else
                        {
                            if (currentPosition.y < startMousePosition.y)
                                rectTransform.sizeDelta += new Vector2(0, heightChange);
                            else
                                rectTransform.sizeDelta -= new Vector2(0, heightChange);


                            rectTransform.anchoredPosition -= new Vector2(0, heightChange / 2);
                        }
                    }
                }
            }

            previousMousePosition = Input.mousePosition; 
        }

        if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            isDragging = false;

            if (!GameManager.Instance.CheckWord(selectedCells))
            {
                Destroy(currentHighlighter);
                if (currentHighlighter != null) SoundManager.Instance.PlayClip(SoundManager.Instance.notSelectSound);
                currentHighlighter = null;
                GameManager.Instance.ResetHighlighter();
            } else
            {
                GameManager.Instance.colorIndex++;
                currentHighlighter = null;
                GameManager.Instance.ResetHighlighter();
                GameManager.Instance.SelectedEffect();
                SoundManager.Instance.PlayClip(SoundManager.Instance.selectSound);
                selectedCells[0].transform.parent.Find("Hint").gameObject.SetActive(false);
            }

            selectedCells.Clear();
            startMousePosition = new Vector2(0, 0);
        }
    }
}
