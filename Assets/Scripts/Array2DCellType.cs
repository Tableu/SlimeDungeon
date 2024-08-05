using Array2DEditor;
using UnityEngine;

[System.Serializable]
public class Array2DCellType : Array2D<Generator2D.CellType>
{
    [SerializeField]
    CellRowCellType[] cells = new CellRowCellType[Consts.defaultGridSize];

    protected override CellRow<Generator2D.CellType> GetCellRow(int idx)
    {
        return cells[idx];
    }
    
    [System.Serializable]
    public class CellRowCellType : CellRow<Generator2D.CellType> { }
}