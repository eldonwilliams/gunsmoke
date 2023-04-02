using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GunController : MonoBehaviour
{
    /// <summary>
    ///  A reference to a TrailRenderer to be instantiated for bullets
    /// </summary>
    [SerializeField, Tooltip("A reference to a TrailRenderer to be instantiated for bullets")]
    private TrailRenderer BulletTrail;

    /// <summary>
    ///  The origin of bullet rays, but not where they appear to come from
    /// </summary>
    [SerializeField, Tooltip("The origin of bullet rays, but not where they appear to come from")]
    private Transform BulletRayOrigin;

    /// <summary>
    ///  The origin of bullet graphics, should be the gun tip
    /// </summary>
    [SerializeField, Tooltip("The origin of bullet graphics, should be the gun tip")]
    private Transform BulletTrailSpawnPoint;

    /// <summary>
    ///  The time between shots
    /// </summary>
    [SerializeField, Tooltip("The time between shots")]
    private float ShootDebounce = 0.5f;

    /// <summary>
    ///  The damage the bullet does to enemies
    /// </summary>
    [SerializeField, Tooltip("The damage the bullet does to enemies")]
    private float BulletDamage = 25;

    /// <summary>
    ///  The speed bullets travel
    /// </summary>
    [SerializeField, Tooltip("The speed bullets travel")]
    private float BulletSpeed = 100;

    /// <summary>
    ///  How far away bullets can travel
    /// </summary>
    [SerializeField, Tooltip("How far away bullets can travel")]
    private float FallOffDistance = 10.0f;

    /// <summary>
    ///  The last time the gun was successfully shot, used for debounce
    /// </summary>
    private float _lastShootTime = 0.0f;

    class RaycastHitComparer : IEqualityComparer<RaycastHit>
    {
        public bool Equals(RaycastHit x, RaycastHit y)
        {
            return x.transform.Equals(y.transform);
        }

        /// <summary>
        ///  Always returns 0 to force check for equality
        ///  This is not recommended, and a better solution will be found if this becomes a performance bottleneck
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(RaycastHit obj)
        {
            return obj.transform.GetHashCode();
        }
    }

    void Update() {
        if (!Input.GetButtonDown("Click")) return;
        if (_lastShootTime + ShootDebounce > Time.time) return;

        Ray shootingRay = new Ray(BulletRayOrigin.position, Vector3Utils.ProjectHorizontally(transform.forward));

        List<RaycastHit> hitEnemies = new List<RaycastHit>(Physics.RaycastAll(shootingRay, FallOffDistance));
        hitEnemies = new List<RaycastHit>(hitEnemies.Distinct(new RaycastHitComparer()));
        foreach (RaycastHit hitEnemy in hitEnemies)
            if (hitEnemy.transform.gameObject.layer == 6)
                onHitEnemy(hitEnemy);

        if (Physics.Raycast(shootingRay, out RaycastHit hit, float.MaxValue)) {
            Vector3 hitPoint = hit.point;
            TrailRenderer trail = Instantiate(BulletTrail, BulletTrailSpawnPoint.position, Quaternion.identity);
            StartCoroutine(spawnTrail(trail, hitPoint));
        } else {
            TrailRenderer trail = Instantiate(BulletTrail, BulletTrailSpawnPoint.position, Quaternion.identity);
            StartCoroutine(spawnTrail(trail, BulletTrailSpawnPoint.position + transform.forward * FallOffDistance));
        }

        _lastShootTime = Time.time;
    }

    private void onHitEnemy(RaycastHit enemyHit) {
        Transform enemyTransform = enemyHit.transform;
        EntityEnemy enemyController = enemyTransform.GetComponent<EntityEnemy>();
        enemyController.DealDamage(BulletDamage);
    }

    /// <summary>
    ///  Spawns the trail graphics for a bullet, does not deal damage.
    /// </summary>
    /// <param name="trail">The trail object</param>
    /// <param name="hitPoint">The point that was hit</param>
    /// <returns>An IEnumerator for StartCoroutine</returns>
    private IEnumerator spawnTrail(TrailRenderer trail, Vector3 hitPoint) {
        Vector3 startPosition = trail.transform.position;
        float distance = Mathf.Clamp(Vector3.Distance(startPosition, hitPoint), 0, FallOffDistance);
        float remainingDistance = distance;

        while (remainingDistance > 0) {
            trail.transform.position = Vector3.Lerp(startPosition, hitPoint, 1 - (remainingDistance / distance));
            remainingDistance -= BulletSpeed * Time.deltaTime;

            yield return null;
        }

        Destroy(trail.gameObject, trail.time);
    }
}
