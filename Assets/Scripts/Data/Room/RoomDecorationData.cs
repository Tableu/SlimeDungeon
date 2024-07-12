using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomData", menuName = "Data/Rooms/Room Data")]
public class RoomDecorationData : ScriptableObject
{
    [Serializable]
    public struct LayoutObject
    {
        [Vector2Range(-1f, 1f, -1f, 1f)]
        public Vector2 RelativePosition;
        public Vector3 Rotation;
        public GameObject Prefab;
        public bool DecorationSpot;
        public bool NearWall;
    }
    
    [Serializable]
    public struct DecorationSpot
    {
        public Transform Location;
        public bool NearWall;

        public DecorationSpot(Transform location, bool nearWall)
        {
            Location = location;
            NearWall = nearWall;
        }
    }

    [SerializeField] private float maxSize;
    [SerializeField] private float minSize;
    [SerializeField] private List<LayoutObject> layoutObjects;
    
    public float MaxSize => maxSize;
    public float MinSize => minSize;
    public List<LayoutObject> LayoutObjects => layoutObjects;
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