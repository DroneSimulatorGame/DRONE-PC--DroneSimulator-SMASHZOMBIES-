using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineScript : Building
{
    public GameObject effect;
    public float detectionRadius = 10f; // Minaning aniqlash radiusi
    public float explosionRadius = 10f; // Minaning portlash radiusi
    public float delayBeforeExplosion = 2f; // Portlashgacha bo'lgan vaqt

    private bool isTriggered = false; // Mina faollashtirilganligini tekshirish uchun
    private Collider targetEnemy; // Portlashga yaqin bo'lgan dushman
    public MineScript() : base("MineScript", 0, 10000, 0, "", false) { }
    public override void UpgradePrefab() { }

    private float timer = 0f;
    private float interval = 2f;
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            if (!isTriggered)
            {
                // Minaning aniqlash radiusidagi dushmanlarni qidiramiz
                Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);
                
                foreach (Collider nearbyObject in colliders)
                {

                    if (nearbyObject.CompareTag("Enemy"))
                    {
                        // Agar dushman aniqlansa, mina faollashtiriladi
                        Debug.Log("Enemy Detect");
                        isTriggered = true;
                        targetEnemy = nearbyObject;
                        StartCoroutine(ExplodeAfterDelay());
                        break;
                    }
                }
            }

            timer = 0f; // Timer qayta tiklanadi
        }
    }

    // 2 sekund kutib, mina portlaydi
    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeExplosion);

        Explode();
    }

    private void Explode()
    {
        GameObject mineEffect = Instantiate(effect, transform.position, Quaternion.identity);
        
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
                // Dushman va mina orasidagi masofani butun son qiymatiga o'zgartiramiz
                int distanceToEnemy = Mathf.RoundToInt(Vector3.Distance(transform.position, nearbyObject.transform.position));

                // Masofaga qarab foizni int sifatida hisoblaymiz
                int damagePercentage = Mathf.Clamp(100 - ((distanceToEnemy * 100) / Mathf.RoundToInt(explosionRadius)), 10, 100);

                // Zararni hisoblab, dushmanga yuboramiz
                DetectBullet detectBullet = rootObject.GetComponent<DetectBullet>();
                if (detectBullet != null)
                {
                    int damage = damagePercentage; // Foiz sifatida zarar
                    detectBullet.TakeingDamage(damage);
                }
                damagedEnemies.Add(rootObject);
                // Zararni ekranga chiqaramiz
                Debug.Log("Enemy at distance: " + distanceToEnemy + " - Damage Percentage: " + damagePercentage + "%");
            }
            else
            {
                Debug.Log("");
            }
        }

        Debug.Log("Mina portladi!");
        Destroy(gameObject); // Minani yo'q qilamiz
    }

    //private void OnDrawGizmos()
    //{
    //    // Mina aniqlash radiusini yashil sfera sifatida chizamiz
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(transform.position, detectionRadius);

    //    // Mina portlash radiusini qizil sfera sifatida chizamiz
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, explosionRadius);
    //}
}
