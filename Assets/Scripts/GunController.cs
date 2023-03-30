using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GunController : MonoBehaviour
{
    public TrailRenderer BulletTrail;
    public Transform BulletSpawnPoint;
    public Transform BulletTrailSpawnPoint;
    public float ShootDebounce = 0.5f;
    public float BulletSpeed = 100;
    public float FallOffDistance = 10.0f;

    private new Camera camera;
    private float lastShootTime = 0.0f;

    void Awake() {
        camera = Camera.main;
    }

    void Update() {
        if (!Input.GetButtonDown("Click")) return;
        if (lastShootTime + ShootDebounce > Time.time) return;

        Ray shootingRay = new Ray(BulletSpawnPoint.position, Vector3Utils.ProjectHorizontally(transform.forward));

        RaycastHit[] hitEnemies = Physics.RaycastAll(shootingRay, FallOffDistance);
        foreach (RaycastHit hitEnemy in hitEnemies)
            if (hitEnemy.transform.gameObject.layer == 6)
                Destroy(hitEnemy.transform.gameObject);

        if (Physics.Raycast(shootingRay, out RaycastHit hit, float.MaxValue)) {
            Vector3 hitPoint = hit.point;
            TrailRenderer trail = Instantiate(BulletTrail, BulletTrailSpawnPoint.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hitPoint));
        } else {
            TrailRenderer trail = Instantiate(BulletTrail, BulletTrailSpawnPoint.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, BulletTrailSpawnPoint.position + transform.forward * FallOffDistance));
        }

        lastShootTime = Time.time;
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, Vector3 hitPoint) {
        Vector3 startPosition = trail.transform.position;
        float distance = Mathf.Clamp(Vector3.Distance(startPosition, hitPoint), 0, FallOffDistance);
        Debug.Log(distance);
        float remainingDistance = distance;

        while (remainingDistance > 0) {
            trail.transform.position = Vector3.Lerp(startPosition, hitPoint, 1 - (remainingDistance / distance));
            remainingDistance -= BulletSpeed * Time.deltaTime;

            yield return null;
        }

        Destroy(trail.gameObject, trail.time);
    }
}
