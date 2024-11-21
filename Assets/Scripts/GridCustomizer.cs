using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridCustomizer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridLayoutGroup gridLayout;
    [SerializeField] private GameObject cellPrefab;

    private float gridWidth;
    private BoardData boardData;
    private int columnCount = 7; 
    private int rowCount;

    [Header("Data")]
    [SerializeField] private float minSpacing = 1f;
    [SerializeField] private float maxSpacing = 6f; 

    public void GeneratePuzzle(BoardData puzzle)
    {
        boardData = puzzle;
        UpdateGridLayout();
        GenerateGrid();
        LevelManager.Instance.PopulateLetterBox(boardData);
        ResizeGrid();
    }

    private void ResizeGrid()
    {
        Transform grid = gridLayout.transform.parent;
        float totalHeight = (gridLayout.cellSize.y * boardData.Rows) + (gridLayout.spacing.y * (boardData.Rows - 1)) + 40; 
        Vector2 currentSize = grid.GetComponent<RectTransform>().sizeDelta;
        currentSize.y = totalHeight;
        grid.GetComponent<RectTransform>().sizeDelta = currentSize;
    }

    private void GenerateGrid()
    {
        int columnCount = boardData.Columns;
        int rowCount = boardData.Rows;

        LevelManager.Instance.objectives = boardData.searchWords.Select(sw => sw.word).ToList();

        foreach (Transform child in gridLayout.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < columnCount; j++)
            {
                GameObject cell = Instantiate(cellPrefab, gridLayout.transform);
                cell.transform.Find("Letter").GetComponent<TMP_Text>().text = boardData.Board[j].Row[i];
                LevelManager.Instance.cells.Add(cell);  
            }
        }
    }

    private void UpdateGridLayout()
    {
        columnCount = boardData.Columns;
        rowCount = boardData.Rows;

        gridWidth = GetGridWidth();

        float cellSize = CellSizeCalculate();
        float spacing = CalculateSpacing(cellSize);

        gridLayout.cellSize = new Vector2(cellSize, cellSize);  
        gridLayout.spacing = new Vector2(spacing, spacing);      
    }

    private float CellSizeCalculate()
    {
        float totalAvailableWidth = gridWidth - (CalculateSpacing(0) * (columnCount - 1));
        float cellWidth = totalAvailableWidth / columnCount;
        return cellWidth; 
    }

    private float CalculateSpacing(float cellSize)
    {
        float totalUsedSpace = cellSize * columnCount;
        float remainingSpace = gridWidth - totalUsedSpace;
        float spacing = remainingSpace / (columnCount - 1);  
        spacing = Mathf.Clamp(spacing, minSpacing, maxSpacing);

        return spacing; 
    }

    private float GetGridWidth()
    {
        RectTransform rectTransform = gridLayout.GetComponent<RectTransform>();
        return rectTransform.rect.width;
    }
}
