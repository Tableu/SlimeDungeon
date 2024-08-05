using Array2DEditor;
using UnityEditor;

[CustomPropertyDrawer(typeof(Array2DCellType))]
public class Array2DCellTypeDrawer : Array2DEnumDrawer<Generator2D.CellType>
{
    protected override object GetDefaultCellValue() => 1;
}
