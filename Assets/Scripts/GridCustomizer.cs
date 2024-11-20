using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridCustomizer : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup gridLayout;
    private BoardData boardData;
    [SerializeField] private GameObject cellPrefab;

    private float gridWidth;

    public int columnCount = 7; 
    public int rowCount;       

    public float minSpacing = 1f; 
    public float maxSpacing = 6f; 

    private void Start()
    {
        
    }

    public void GeneratePuzzle(BoardData puzzle)
    {
        boardData = puzzle;
        UpdateGridLayout();
        GenerateGrid();
        LevelManager.Instance.PopulateLetterBox(boardData);
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
                cell.GetComponent<BlockData>().id = i * columnCount + j;
                LevelManager.Instance.cells.Add(cell);  

            }
        }
    }

    private void GenerateWordHolder()
    {

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

        // Calculate the spacing between cells based on the remaining space
        float spacing = remainingSpace / (columnCount - 1);  // Subtract 1 because there are columnCount - 1 gaps between cells

        // Ensure that the spacing is within the specified range
        spacing = Mathf.Clamp(spacing, minSpacing, maxSpacing);

        return spacing; // Return the calculated spacing
    }

    private float GetGridWidth()
    {
        RectTransform rectTransform = gridLayout.GetComponent<RectTransform>();
        return rectTransform.rect.width;
    }
}
