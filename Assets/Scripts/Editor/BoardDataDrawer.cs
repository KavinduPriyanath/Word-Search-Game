using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(BoardData))]
[CanEditMultipleObjects]
[System.Serializable]
public class BoardDataDrawer : Editor
{
    private BoardData gameDataInstance => target as BoardData;
    private ReorderableList _dataList;

    private void OnEnable()
    {
        InitializeReordableList(ref _dataList, "searchWords", "Searching Words");
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("puzzleId"), new GUIContent("Id"));

        DrawColumnsRowsInputFields();
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("theme"), new GUIContent("Theme"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("themeImage"), new GUIContent("Theme Image"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("headingColor"), new GUIContent("Heading Color"));

        if (gameDataInstance.Board != null && gameDataInstance.Columns > 0 && gameDataInstance.Rows > 0)
        {
            DrawBoardTable();
        }

        GUILayout.BeginHorizontal();
        ClearBoardButton();
        FillupWithRandomLetterButton();
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
        _dataList.DoLayoutList();


        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(gameDataInstance);
        }


    }

    private void DrawColumnsRowsInputFields()
    {
        var columnsTemp = gameDataInstance.Columns;
        var rowsTemp = gameDataInstance.Rows;

        gameDataInstance.Columns = EditorGUILayout.IntField("Columns", gameDataInstance.Columns);
        gameDataInstance.Rows = EditorGUILayout.IntField("Rows", gameDataInstance.Rows);


        if ((gameDataInstance.Columns != columnsTemp || gameDataInstance.Rows != rowsTemp) && gameDataInstance.Columns > 0 && gameDataInstance.Rows > 0)
        {
            gameDataInstance.CreateNewBoard();
        }
    }

    private void DrawBoardTable()
    {
        var tableStyle = new GUIStyle("box");
        tableStyle.padding = new RectOffset(10, 10, 10, 10);
        tableStyle.margin.left = 32;

        var headerColumnStyle = new GUIStyle();
        headerColumnStyle.fixedWidth = 35;

        var columnStyle = new GUIStyle();
        columnStyle.fixedWidth = 50;

        var rowStyle = new GUIStyle();
        rowStyle.fixedHeight = 25;
        rowStyle.fixedWidth = 40;
        rowStyle.alignment = TextAnchor.MiddleCenter;

        var textFieldStyle = new GUIStyle();

        textFieldStyle.normal.background = Texture2D.grayTexture;
        textFieldStyle.normal.textColor = Color.white;
        textFieldStyle.fontStyle = FontStyle.Bold;
        textFieldStyle.alignment = TextAnchor.MiddleCenter;

        EditorGUILayout.BeginHorizontal(tableStyle);
        for (var x=0; x < gameDataInstance.Columns;  x++)
        {
            EditorGUILayout.BeginVertical(x == -1 ? headerColumnStyle : columnStyle);
            for (var y=0; y < gameDataInstance.Rows; y++)
            {
                if (x >= 0 && y >= 0)
                {
                    EditorGUILayout.BeginHorizontal(rowStyle);
                    var character = (string)EditorGUILayout.TextArea(gameDataInstance.Board[x].Row[y], textFieldStyle);
                    if (gameDataInstance.Board[x].Row[y].Length > 1)
                    {
                        character = gameDataInstance.Board[x].Row[y].Substring(0, 1);
                    }
                    gameDataInstance.Board[x].Row[y] = character;
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();

    }

    private void InitializeReordableList(ref ReorderableList list, string propertyName, string listLabel)
    {
        list = new ReorderableList(serializedObject, serializedObject.FindProperty(propertyName), true, true, true, true);

        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, listLabel);
        };

        var l = list;

        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = l.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            EditorGUI.PropertyField(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("word"), GUIContent.none);
        };
    }

    private void ClearBoardButton()
    {
        if (GUILayout.Button("Clear"))
        {
            for (int i=0; i < gameDataInstance.Columns; i++)
            {
                for (int j=0; j<gameDataInstance.Rows; j++)
                {
                    gameDataInstance.Board[i].Row[j] = " ";
                }
            }
        }
    }

    private void FillupWithRandomLetterButton()
    {
        if (GUILayout.Button("Fill Randomly"))
        {
            for (int i=0; i < gameDataInstance.Columns; i++)
            {
                for (int j=0; j<gameDataInstance.Rows; j++)
                {
                    int errorCounter = Regex.Matches(gameDataInstance.Board[i].Row[j], @"[a-zA-Z]").Count;
                    string letters = "ABCDEFGHIJKLMNOPQRSTUVWYZ";
                    int index = UnityEngine.Random.Range(0, letters.Length);

                    if (errorCounter == 0)
                    {
                        gameDataInstance.Board[i].Row[j] = letters[index].ToString();
                    }
                }
            }
        }
    }
    
}
