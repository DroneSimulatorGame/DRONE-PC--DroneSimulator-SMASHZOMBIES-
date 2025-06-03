using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : Building
{

    public GameObject effect;
    public float explosionRadius = 10f;
    public BombScript() : base("BombScript", 0, 10000, 0, "", false) { }
    public override void UpgradePrefab() { }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Untagged") || collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Target"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        GameObject bombEffect = Instantiate(effect, transform.position, Quaternion.identity);

        // Portlash radiusidagi barcha ob'ektlarni topamiz
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        HashSet<GameObject> damagedEnemies = new HashSet<GameObject>();

        foreach (Collider nearbyObject in colliders)
        {
            if (nearbyObject.CompareTag("Enemy"))
            {

                GameObject enemy = nearbyObject.gameObject;
                GameObject rootObject = enemy.transform.root.gameObject;

                if (damagedEnemies.Contains(rootObject))
                    continue;
                // Dushman va bomba orasidagi masofani butun son qiymatiga o'zgartiramiz
                int distanceToEnemy = Mathf.RoundToInt(Vector3.Distance(transform.position, rootObject.transform.position));

                // Masofaga qarab foizni int sifatida hisoblaymiz
                int damagePercentage = Mathf.Clamp(100 - ((distanceToEnemy * 100) / Mathf.RoundToInt(explosionRadius)), 10, 100);

                // Zararni hisoblab, dushmanga yuboramiz
                DetectBullet detectBullet = rootObject.GetComponent<DetectBullet>();
                if (detectBullet != null)
                {
                    int damage = damagePercentage; // Foiz sifatida zarar
                    detectBullet.TakeingDamage(damage);
                    Debug.Log("Enemy " + damage);
                    damagedEnemies.Add(rootObject);
                }
                else
                {
                    int damage = damagePercentage;
                    Debug.Log("Enemy " + damage);
                }

                // Zararni ekranga chiqaramiz
                Debug.Log("Enemy at distance: " + distanceToEnemy + " - Damage Percentage: " + damagePercentage + "%");
            }
            else
            {
                Debug.Log("");
            }
        }

        //Debug.Log("BOOOOM!!!");
        Destroy(gameObject); // Bombani yo'q qilamiz
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(transform.position, explosionRadius);
    //}
}
