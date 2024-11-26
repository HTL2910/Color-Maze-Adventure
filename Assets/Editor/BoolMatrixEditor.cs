using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoolMatrix))]
public class BoolMatrixEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BoolMatrix boolMatrix = (BoolMatrix)target;

        // Hiển thị các trường
        boolMatrix.ballMaterial = (Material)EditorGUILayout.ObjectField("Ball Material", boolMatrix.ballMaterial, typeof(Material), false);
        boolMatrix.wallMaterial = (Material)EditorGUILayout.ObjectField("Wall Material", boolMatrix.wallMaterial, typeof(Material), false);
        boolMatrix.colorBackground = EditorGUILayout.ColorField("Color Background", boolMatrix.colorBackground);
        boolMatrix.newColorBackground = EditorGUILayout.ColorField("New Color Background", boolMatrix.newColorBackground);

        // Nút sao chép
        if (GUILayout.Button("Copy 4 Fields"))
        {
            EditorGUIUtility.systemCopyBuffer = JsonUtility.ToJson(new SerializableFields(boolMatrix));
            Debug.Log("Copied 4 fields to clipboard.");
        }

        // Nút dán
        if (GUILayout.Button("Paste 4 Fields"))
        {
            SerializableFields pastedFields = JsonUtility.FromJson<SerializableFields>(EditorGUIUtility.systemCopyBuffer);
            if (pastedFields != null)
            {
                pastedFields.ApplyTo(boolMatrix);
                Debug.Log("Pasted 4 fields from clipboard.");
            }
            else
            {
                Debug.LogWarning("Clipboard does not contain valid data.");
            }
        }
        boolMatrix.rows = EditorGUILayout.IntField("Rows", boolMatrix.rows);
        boolMatrix.columns = EditorGUILayout.IntField("Columns", boolMatrix.columns);

        // Đảm bảo ma trận khớp với kích thước
        if (boolMatrix.matrix.Count != boolMatrix.rows * boolMatrix.columns)
        {
            boolMatrix.matrix = new List<bool>(new bool[boolMatrix.rows * boolMatrix.columns]);
        }

        // Hiển thị ma trận dưới dạng bảng
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

    [System.Serializable]
    private class SerializableFields
    {
        public Material ballMaterial;
        public Material wallMaterial;
        public Color colorBackground;
        public Color newColorBackground;

        public SerializableFields(BoolMatrix matrix)
        {
            ballMaterial = matrix.ballMaterial;
            wallMaterial = matrix.wallMaterial;
            colorBackground = matrix.colorBackground;
            newColorBackground = matrix.newColorBackground;
        }

        public void ApplyTo(BoolMatrix matrix)
        {
            matrix.ballMaterial = ballMaterial;
            matrix.wallMaterial = wallMaterial;
            matrix.colorBackground = colorBackground;
            matrix.newColorBackground = newColorBackground;
        }
    }
}
