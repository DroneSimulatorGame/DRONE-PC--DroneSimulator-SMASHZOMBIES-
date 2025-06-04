using Unity.Mathematics;
using UnityEngine;

public class EnimyDetect : MonoBehaviour
{
    //public GameObject smoke;
    public GameObject health;
    public int devorHP = 100;
    private int devorHPstore;
    public GameObject buzilganPrefab;

    private bool buzilganStart = false;
    private float waitForSeconds = 0.5f;

    //[SerializeField] private AudioSource audioSource;

    private void Start()
    {
        health.GetComponent<HealthScript>().SetMaxHealth(devorHP);
        devorHPstore = devorHP;
    }
    private void Update()
    {
        if (buzilganStart)
        {
            waitForSeconds -= Time.deltaTime;
        }

        if (devorHP <= 0)
        {

            Buzilgan();
        }
        health.GetComponent<HealthScript>().SetHealth(devorHP);
    }

    public void Damage(int damage)
    {
        devorHP -= damage;
        health.SetActive(true);
    }
    private void Buzilgan()
    {
        buzilganStart = true;
        // gameObject.GetComponent<AudioSource>().Play();  

        if (waitForSeconds <= 0)
        {
            ParticleSystemManager.Instance.PlayWallDestroy(transform.position, transform.rotation);
            GameObject buzilgan = Instantiate(buzilganPrefab, transform.position, transform.rotation);
            buzilgan.transform.SetParent(transform.parent);
            waitForSeconds = 0.5f;
            Destroy(gameObject);

        }
    }
    public void ResetWallHealth()
    {
        devorHP = devorHPstore;
        health.SetActive(false);
    }
}
