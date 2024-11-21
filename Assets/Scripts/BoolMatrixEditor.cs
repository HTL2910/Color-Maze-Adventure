using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoolMatrix))]
public class BoolMatrixEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BoolMatrix boolMatrix = (BoolMatrix)target;

        // Điều chỉnh số hàng và cột
        boolMatrix.rows = EditorGUILayout.IntField("Rows", boolMatrix.rows);
        boolMatrix.columns = EditorGUILayout.IntField("Columns", boolMatrix.columns);

        // Đảm bảo ma trận khớp với kích thước
        if (boolMatrix.matrix.Count != boolMatrix.rows * boolMatrix.columns)
        {
            boolMatrix.matrix = new List<bool>(new bool[boolMatrix.rows * boolMatrix.columns]);
        }

        // Hiển thị ma trận dưới dạng bảng
        for (int i = boolMatrix.rows-1; i>=0; i--)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = boolMatrix.columns-1; j >=0 ; j--)
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
}
