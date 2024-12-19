using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
[CreateAssetMenu(fileName = "BoolMatrix", menuName = "ScriptableObjects/BoolMatrix", order = 1)]
public class BoolMatrix : ScriptableObject
{
    
    public int rows = 9; // Số hàng
    public int columns = 12; // Số cột
    public Material ballMaterial;
    public Material wallMaterial;
    public Color colorBackground;
    public Color newColorBackground;
   
   
    public List<bool> matrix = new List<bool>(); // Sử dụng List<bool> thay cho bool[,]
    //public List<bool> matrix = new List<bool>(); // Sử dụng List<bool> thay cho bool[,]

    private void Reset()
    {
        // Khởi tạo ma trận nếu không khớp kích thước
        if (matrix.Count != rows * columns)
        {
            matrix = new List<bool>(new bool[rows * columns]);
        }
        colorBackground= Color.blue;
        newColorBackground = Color.yellow;
    }
   
    // Hàm tiện ích để truy cập ma trận
    public bool GetValue(int row, int column)
    {
        return matrix[row * columns + column];
    }

    public void SetValue(int row, int column, bool value)
    {
        matrix[row * columns + column] = value;
    }
}
