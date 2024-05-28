using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    private void Update()
    {
        transform.position = Mouse.current.position.ReadValue();
    }
}
