using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomLayoutData", menuName = "Data/Rooms/Room Layout Data")]
public class RoomLayoutData : ScriptableObject
{
    [Serializable]
    private struct LayoutObject
    {
        [Vector2Range(-1f, 1f, -1f, 1f)]
        public Vector2 RelativePosition;
        public Vector3 Rotation;
        public GameObject Prefab;
    }
    
    [SerializeField] private float maxSize;
    [SerializeField] private float minSize;
    [SerializeField] private List<LayoutObject> layoutObjects;
    
    public float MaxSize => maxSize;
    public float MinSize => minSize;

    public List<Transform> PlaceRoomLayout(Transform center, RectInt bounds, float tileSize, List<Vector3> doors)
    {
        List<Transform> decorationSpots = new List<Transform>();
        foreach (LayoutObject layoutObject in layoutObjects)
        {
            Vector3 pos = new Vector3((bounds.width-3)*tileSize/2*layoutObject.RelativePosition.x,
                0, (bounds.height-3)*tileSize/2*layoutObject.RelativePosition.y);
            if (doors.Any(o => Vector3.Distance(pos, o) < 2))
                continue;
            GameObject instance = Instantiate(layoutObject.Prefab, center.position + pos, 
                Quaternion.Euler(layoutObject.Rotation), center);
            DecorationSpots spots = instance.GetComponent<DecorationSpots>();
            if(spots != null)
                decorationSpots.AddRange(spots.Locations);
        }

        return decorationSpots;
    }
}

//https://www.saguiitay.com/unity-tutorial-vector2rangeattribute/
public class Vector2RangeAttribute : PropertyAttribute
{
    // Min/Max values for the X axis
    public readonly float MinX;
    public readonly float MaxX;
    // Min/Max values for the Y axis
    public readonly float MinY;
    public readonly float MaxY;
 
    public Vector2RangeAttribute(float fMinX, float fMaxX, float fMinY, float fMaxY)
    {
        MinX = fMinX;
        MaxX = fMaxX;
        MinY = fMinY;
        MaxY = fMaxY;
    }
}

[CustomPropertyDrawer(typeof(Vector2RangeAttribute))]
public class Vector2RangeAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginChangeCheck();
        // Create a regular Vector2 field
        Vector2 val = EditorGUI.Vector2Field(position, label, property.vector2Value);
        // If the value changed
        if (EditorGUI.EndChangeCheck())
        {
            var rangeAttribute = (Vector2RangeAttribute)attribute;
            // Clamp the X/Y values to be within the allowed range
            val.x = Mathf.Clamp(val.x, rangeAttribute.MinX, rangeAttribute.MaxX);
            val.y = Mathf.Clamp(val.y, rangeAttribute.MinY, rangeAttribute.MaxY);
            // Update the value of the property to the clampped value
            property.vector2Value = val;
        }
    }
}