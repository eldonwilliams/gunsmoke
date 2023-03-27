using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform trackedObject;
    public Vector3 offset;
    public float trackSpeed = 2.0f;

    private Transform cameraTransform;

    /*
        Calculates the Camera's position and rotation.
        This an instantaneous rate of change, no lerp.
        First value is Pos, second is Rotation.
    */
    private (Vector3, Vector3) calculateCameraPosAndRot() {
        Vector3 position = trackedObject.position - trackedObject.forward + offset;
        Vector3 rotation = Quaternion.LookRotation(((trackedObject.forward + trackedObject.position) - position).normalized).eulerAngles;
        return (position, rotation);
    }

    void Start()
    {
        cameraTransform = gameObject.GetComponent<Camera>().transform;
        var (position, rotation) = calculateCameraPosAndRot();
        cameraTransform.position = position;
        cameraTransform.rotation = Quaternion.Euler(rotation);
    }

    void Update()
    {
        var (position, rotation) = calculateCameraPosAndRot();
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, position, Time.deltaTime * trackSpeed);
        cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, Quaternion.Euler(rotation), Time.deltaTime * trackSpeed);
    }
}
