using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObjects/levelObject", order = 1)]
public class LevelObject : ScriptableObject
{
    public List<BoolMatrix> list_Matrix;
}
