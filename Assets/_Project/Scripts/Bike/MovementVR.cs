using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementVR : MonoBehaviour
{
    private BicycleControls controls = null;

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    } 

    private void Awake()
    {
        controls = new BicycleControls();
    }   

    public void HorizontalVR() {
        Debug.Log("[HorizontalVR] Event Triggered!");
    }

    private void Update()
    {
        var horizontal = controls.Player.HorizontalVR.ReadValue<Vector2>();
        Debug.Log($"[HorizontalMovementVR] Horizontal: {horizontal}");
    }
}
