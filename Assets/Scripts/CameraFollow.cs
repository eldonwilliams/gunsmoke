using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    /// <summary>
    ///  Allows for multiple types of Camera tracking movement
    ///  Currently, only moving is ever used in game, but stationary and other forms may be implemented or used
    /// </summary>
    public enum FollowType
    {
        /// <summary>
        ///  Useful for moving targets, mainly the player
        /// </summary>
        MOVING,
        /// <summary>
        ///  Useful for a stationary object, but still will track movement
        /// </summary>
        STATIONARY,
    }

    /// <summary>
    ///  The currently active follow type
    /// </summary>
    [Tooltip("The currently active follow type")]
    public FollowType ActiveFollowType = FollowType.MOVING;
    
    /// <summary>
    ///  The Transform of the trackedObject
    /// </summary>
    [SerializeField, Tooltip("The Transform of the trackedObject")]
    private Transform TrackedObject;
    
    /// <summary>
    ///  The max distance the camera's mouse ray will go
    /// </summary>
    [SerializeField, Tooltip("The max distance the camera's mouse ray will go")]
    public float MaxMouseDistance = 30;

    /// <summary>
    ///  The clamp to the length away the mouse parallax effect will work
    /// </summary>
    [SerializeField, Tooltip("The clamp to the length away the mouse parallax effect will work")]
    public float MaxMidpointMagnitude = 3;

    /// <summary>
    ///  The offset of the camera in the +y direction
    /// </summary>
    [SerializeField, Tooltip("The offset of the camera in the +y direction")]
    private float OffsetY = 2.0f;
    
    /// <summary>
    ///  The speed of the camera
    /// </summary>
    [SerializeField, Tooltip("The speed of the camera")]
    private float TrackSpeed = 2.0f;

    void Update()
    {
        switch (ActiveFollowType)
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
        if (Physics.Raycast(mouseRay, out mouseRaycastHit, MaxMouseDistance)) {
            Vector3 point = mouseRaycastHit.point;
            point.y = 0;
            midpoint = Vector3Utils.Midpoint(TrackedObject.position, point);
            midpoint = Vector3.ClampMagnitude(midpoint, MaxMidpointMagnitude);
            // radius of circle formed by two points
            radius = midpoint.magnitude * 2;
        } else {
            midpoint = Vector3Utils.Midpoint(TrackedObject.position, new Vector3(1, 0, 1));
            midpoint = Vector3.ClampMagnitude(midpoint, MaxMidpointMagnitude);
            radius = midpoint.magnitude * 2;
        }
        midpoint.y = 0;

        position = TrackedObject.position;
        // Move back by radius, putting this as a point on cylinder where h=inf
        position -= TrackedObject.forward * radius;
        position.y += OffsetY;

        rotation = Quaternion.LookRotation((TrackedObject.position + midpoint) - transform.position);

        transform.position = Vector3.Lerp(transform.position, position, TrackSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, TrackSpeed * Time.deltaTime);
    }

    private void stationaryUpdate() {
        Vector3 position = TrackedObject.position - Vector3Utils.Abs(TrackedObject.forward * TrackedObject.localScale.magnitude * 3);
        position.y += OffsetY;
        Vector3 rotation = Quaternion.LookRotation(((TrackedObject.forward + TrackedObject.position) - position).normalized).eulerAngles;
        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * TrackSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotation), Time.deltaTime * TrackSpeed);
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
