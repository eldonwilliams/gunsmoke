using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    // The type of camera following algorithm to use
    public enum FollowType
    {
        // This is recommended for moving targets, such as the player. It tracks movement over time.
        MOVING,
        // This is recommended for a stationary or a seldomly moving target, such as a box. It will track movement, but not accurately.
        STATIONARY,
    }

    // The type of following currently active
    public FollowType followType = FollowType.MOVING;
    // The currently tracked object
    public Transform trackedObject;
    // The offset in the +Y direction
    public float offsetY = 2.0f;
    // The speed to track the object at
    public float trackSpeed = 2.0f;

    /*
        FOR FollowType.STATIONARY

        Calculates the Camera's position and rotation.
        This an instantaneous rate of change, no lerp.
        First value is Pos, second is Rotation.
    */
    private (Vector3, Vector3) calculateCameraPosAndRot() {
        Vector3 position = trackedObject.position - Vector3Utils.Abs(trackedObject.forward * trackedObject.localScale.magnitude * 3);
        position.y += offsetY;
        Vector3 rotation = Quaternion.LookRotation(((trackedObject.forward + trackedObject.position) - position).normalized).eulerAngles;
        return (position, rotation);
    }

    void Start()
    {
        var (position, rotation) = calculateCameraPosAndRot();
        transform.position = position;
        transform.rotation = Quaternion.Euler(rotation);
    }

    void Update()
    {
        switch (followType)
        {
            case FollowType.MOVING:
                movingUpdate();
                break;
            case FollowType.STATIONARY:
                stationaryUpdate();
                break;
            default:
                break;
        }
    }

    private void movingUpdate() {
        // This is the goal position of the camera
        Vector3 position;
        // This is the goal rotation of the camera
        Quaternion rotation;

        /*
            This is where we get the mouse's screen position as a world position
            If it is not hitting an object within 30 units, it will not pick up anything.

            The midpoint is the midpoint between the mouse's 3d position and the target
            If no mouse exists, it will be the midpoint between the target's forward vector,
            and a circle of configurable radius.
        */
        Vector3 mousePos = Input.mousePosition;
        Ray mouseRay = transform.GetComponent<Camera>().ScreenPointToRay(mousePos);
        RaycastHit mouseRaycastHit;
        // The midpoint between trackedObject.position and mouseRaycastHit.point
        Vector3 midpoint;
        // The radius of the circle made by trackedObject.position and mouseRaycastHit.point
        float radius;
        if (Physics.Raycast(mouseRay, out mouseRaycastHit, 30.0f)) {
            Vector3 point = mouseRaycastHit.point;
            point.y = 0;
            midpoint = Vector3Utils.Midpoint(trackedObject.position, point);
            midpoint = Vector3.ClampMagnitude(midpoint, 3.0f);
            // radius of circle formed by two points
            radius = midpoint.magnitude * 2;
        } else {
            midpoint = Vector3Utils.Midpoint(trackedObject.position, new Vector3(1, 0, 1));
            midpoint = Vector3.ClampMagnitude(midpoint, 3.0f);
            radius = midpoint.magnitude * 2;
        }
        midpoint.y = 0;

        position = trackedObject.position;
        // Move back by radius, putting this as a point on cylinder where h=inf
        position -= trackedObject.forward * radius;
        position.y += offsetY;

        rotation = Quaternion.LookRotation((trackedObject.position + midpoint) - transform.position);

        transform.position = Vector3.Lerp(transform.position, position, trackSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, trackSpeed * Time.deltaTime);
    }

    private void stationaryUpdate() {
        var (position, rotation) = calculateCameraPosAndRot();
        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * trackSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotation), Time.deltaTime * trackSpeed);
    }

    // The Old Moving Update
    // private void movingUpdate() {
    //     /*
    //         On first update, lastPosition will be Vector3.zero.
    //             Set to trackedObject.position, b/c deltaPosition on first frame should be ~0,0,0
    //         Find the change in position (newPos - oldPos)
    //         Update last position for next frame
    //     */
    //     if (lastPosition == Vector3.zero) lastPosition = trackedObject.position;
    //     Vector3 deltaPosition = trackedObject.position - lastPosition;
    //     deltaPosition.y = 0;
    //     lastPosition = trackedObject.position;
    //     // The deltaVector but with a magnitude of 1, used for adding to other vectors
    //     Vector3 deltaVector = deltaPosition / Time.deltaTime;
    //     deltaVector.y = 0;
        
    //     // trackedObject.position where y compoonent is 0
    //     Vector3 noYPosition = trackedObject.position;
    //     noYPosition.y = 0;

    //     // Position calculation
    //     /*
    //         Position is the trackedObject's position subtracted by the change in position ()
    //     */
    //     Vector3 position = trackedObject.position;
    //     position -= Vector3Utils.ProjectHorizontally(trackedObject.forward) * 5;
    //     // Removed: position -= trackedObject.forward;
    //     position.y += offsetY;
    //     position.y -= Mathf.Clamp(deltaVector.magnitude, 0, offsetY/4);
    //     /*
    //         Use the deltaVector 
    //     */
    //     Vector3 rotation = Quaternion.LookRotation((deltaVector + noYPosition - position).normalized).eulerAngles;

    //     /*
    //         Rotation stops when moving b/c it can lead to weird movement errors.
    //         Lerp is used because of the nice cinematic effect it creates.
    //     */
    //     rotationLerpSave = Mathf.Lerp(
    //         rotationLerpSave,
    //         deltaPosition.magnitude > 0 ? 0.0f : 0.75f,
    //         Time.deltaTime);
    //     //
    //     Vector3 newPosition = Vector3.Lerp(cameraTransform.position, position, Time.deltaTime * trackSpeed);
    //     newPosition.y = Mathf.Lerp(cameraTransform.position.y, position.y, Time.deltaTime * trackSpeed);
    //     cameraTransform.position = newPosition;

    //     cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, Quaternion.Euler(rotation), Time.deltaTime * trackSpeed * rotationLerpSave);
    // }
}
