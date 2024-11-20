using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEditor;
using TMPro;

public class WordDetector : MonoBehaviour
{
    public List<GameObject> selectedCells = new List<GameObject>();
    public bool isDragging = false;

    public GraphicRaycaster graphicRaycaster;
    public EventSystem eventSystem;
    public GameObject highlighterPrefab;
    public Transform grid;

    private GameObject currentHighlighter = null; 
    private Vector2 previousMousePosition;

    public bool wordFound = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            isDragging = true;
            selectedCells.Clear();
            currentHighlighter = null;
            previousMousePosition = (Input.touchCount > 0) ? (Vector2)Input.GetTouch(0).position : (Vector2)Input.mousePosition;
        }

        if (isDragging && (Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)))
        {
            Vector2 currentPosition = (Input.touchCount > 0) ? (Vector2)Input.GetTouch(0).position : (Vector2)Input.mousePosition;
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
                if (!selectedCells.Contains(hitObject) && result.gameObject.CompareTag("Letter"))
                {
                    Debug.Log(hitObject.name);
                    selectedCells.Add(hitObject);

                    if (currentHighlighter == null)
                    {
                        GameObject highlighter = Instantiate(highlighterPrefab, hitObject.transform.position, Quaternion.identity);
                        highlighter.GetComponent<Image>().color = GameManager.Instance.colors[GameManager.Instance.colorIndex];
                        highlighter.transform.SetParent(grid);  
                        highlighter.SetActive(true);
                        highlighter.transform.SetAsFirstSibling();

                        currentHighlighter = highlighter;
                    }

                    GameManager.Instance.HighlightLetterBanner(hitObject.GetComponent<TMP_Text>().text);
                }
            }

            if (currentHighlighter != null) 
            {
                //Vector2 mouseDelta = (Vector2)Input.mousePosition - previousMousePosition;

                RectTransform rectTransform = currentHighlighter.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    if (Mathf.Abs(mouseDelta.x) > Mathf.Abs(mouseDelta.y))
                    {
                        float widthChange = Mathf.Abs(mouseDelta.x) * 1f;

                        rectTransform.sizeDelta += new Vector2(widthChange, 0);

                        if (mouseDelta.x > 0)
                        {
                            rectTransform.anchoredPosition += new Vector2(widthChange / 2, 0);
                            //might need to change the widthchange by some value depending on the width
                        }
                        else
                        {
                            rectTransform.anchoredPosition -= new Vector2(widthChange / 2, 0);

                        }
                    }
                    else if (Mathf.Abs(mouseDelta.y) > Mathf.Abs(mouseDelta.x))
                    {
                        float heightChange = Mathf.Abs(mouseDelta.y) * 1f;
                        rectTransform.sizeDelta += new Vector2(0, heightChange);

                        if (mouseDelta.y > 0)
                            rectTransform.anchoredPosition += new Vector2(0, heightChange / 2);
                        else
                            rectTransform.anchoredPosition -= new Vector2(0, heightChange / 2);

                    }
                }
                
                //previousMousePosition = Input.mousePosition;
            }

            previousMousePosition = Input.mousePosition; 
        }

        if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            isDragging = false;

            if (!GameManager.Instance.CheckWord(selectedCells))
            {
                Destroy(currentHighlighter);
                currentHighlighter = null;
                GameManager.Instance.ResetHighlighter();
            } else
            {
                Debug.Log("Found One");
                GameManager.Instance.colorIndex++;
                currentHighlighter = null;
                GameManager.Instance.ResetHighlighter();
                GameManager.Instance.SelectedEffect();
                selectedCells[0].transform.parent.Find("Hint").gameObject.SetActive(false);
                //Add finding effect
            }

            selectedCells.Clear();
            currentHighlighter = null;
        }
    }
}
