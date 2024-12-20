using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoolMatrix))]
public class BoolMatrixEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BoolMatrix boolMatrix = (BoolMatrix)target;

        // Hiển thị các trường cơ bản
        boolMatrix.ballMaterial = (Material)EditorGUILayout.ObjectField("Ball Material", boolMatrix.ballMaterial, typeof(Material), false);
        boolMatrix.wallMaterial = (Material)EditorGUILayout.ObjectField("Wall Material", boolMatrix.wallMaterial, typeof(Material), false);
        boolMatrix.colorBackground = EditorGUILayout.ColorField("Color Background", boolMatrix.colorBackground);
        boolMatrix.newColorBackground = EditorGUILayout.ColorField("New Color Background", boolMatrix.newColorBackground);

        // Hiển thị và chỉnh sửa trapInMap
        EditorGUILayout.LabelField("Trap In Map", EditorStyles.boldLabel);
        if (boolMatrix.trapInMap == null || boolMatrix.trapInMap.Length == 0)
        {
            if (GUILayout.Button("Add Trap"))
            {
                boolMatrix.trapInMap = new TileType[1];
                boolMatrix.trapInMap[0] = new TileType();
            }
        }
        else
        {
            for (int i = 0; i < boolMatrix.trapInMap.Length; i++)
            {
                EditorGUILayout.BeginVertical("box");

                // Hiển thị các thuộc tính của TileType
                EditorGUILayout.LabelField($"Trap {i + 1}", EditorStyles.boldLabel);
                boolMatrix.trapInMap[i].x = EditorGUILayout.IntField("X", boolMatrix.trapInMap[i].x);
                boolMatrix.trapInMap[i].y = EditorGUILayout.IntField("Y", boolMatrix.trapInMap[i].y);
                boolMatrix.trapInMap[i].tileKind = (TileTrap)EditorGUILayout.EnumPopup("Tile Kind", boolMatrix.trapInMap[i].tileKind);

                // Nút xóa trap
                if (GUILayout.Button("Remove Trap"))
                {
                    RemoveTrapAt(ref boolMatrix.trapInMap, i);
                    break;
                }

                EditorGUILayout.EndVertical();
            }

            // Nút thêm trap mới
            if (GUILayout.Button("Add Trap"))
            {
                AddTrap(ref boolMatrix.trapInMap);
            }
        }

        // Hiển thị và chỉnh sửa ma trận
        boolMatrix.rows = EditorGUILayout.IntField("Rows", boolMatrix.rows);
        boolMatrix.columns = EditorGUILayout.IntField("Columns", boolMatrix.columns);

        if (boolMatrix.matrix.Count != boolMatrix.rows * boolMatrix.columns)
        {
            boolMatrix.matrix = new List<bool>(new bool[boolMatrix.rows * boolMatrix.columns]);
        }

        for (int i = 0; i < boolMatrix.rows; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < boolMatrix.columns; j++)
            {
                bool currentValue = boolMatrix.GetValue(i, j);
                bool newValue = EditorGUILayout.Toggle(currentValue, GUILayout.Width(20));
                if (currentValue != newValue)
                {
                    boolMatrix.SetValue(i, j, newValue);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        // Lưu thay đổi
        if (GUI.changed)
        {
            EditorUtility.SetDirty(boolMatrix);
        }
    }

    private void AddTrap(ref TileType[] trapArray)
    {
        if (trapArray == null)
        {
            trapArray = new TileType[1];
        }
        else
        {
            var tempList = new List<TileType>(trapArray);
            tempList.Add(new TileType()); // Thêm một trap mới với giá trị mặc định
            trapArray = tempList.ToArray();
        }
    }

    private void RemoveTrapAt(ref TileType[] trapArray, int index)
    {
        var tempList = new List<TileType>(trapArray);
        tempList.RemoveAt(index);
        trapArray = tempList.ToArray();
    }
}
