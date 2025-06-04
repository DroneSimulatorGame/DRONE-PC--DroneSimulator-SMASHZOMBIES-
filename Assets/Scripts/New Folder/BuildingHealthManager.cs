using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building1 : MonoBehaviour
{

    public static Building1 instance;

    //public GameObject smoke;
    public int buildingHP = 200;

    public int buildingHPStore = 0;
    public GameObject health;
    public GameObject buzilganPrefab;
    //MyCharacterController changeTargets; /*= MyCharacterController.FindObjectOfType<MyCharacterController>();*/
    public int index = 0;

    private void Awake()
    {
        instance = this;
    }

    //[SerializeField] private AudioSource destroySound;
    void Start()
    {
        health.GetComponent<HealthScript>().SetMaxHealth(buildingHP); 
        index = gameObject.GetComponent<Building>().index;

        buildingHPStore = buildingHP;
    }
    void Update()
    {
        if (buildingHP <= 0)
        {
            Buzilgan();
        }
        health.GetComponent<HealthScript>().SetHealth(buildingHP);   
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.name == "Bomb(Clone)")
        {
            buildingHP -= 10;
        }
    }
    public void Damage(int damage)
    {
        buildingHP -= damage;
        health.SetActive(true);
    }

    private void Buzilgan()
    {
        //changeTargets = MyCharacterController.FindObjectOfType<MyCharacterController>();
        ParticleSystemManager.Instance.PlayDestruction(index);

        GameObject buzilgan = Instantiate(buzilganPrefab, transform.position, transform.rotation);
        buzilgan.transform.SetParent(transform.parent);
        buzilgan.GetComponent<Building>().index = index;             

        Buzilgan buzilganScript = buzilgan.GetComponent<Buzilgan>();
        //if (buzilganScript != null)
        //{
        //    buzilganScript.SetIndex(index);  // Set the index value from the building to the prefab
        //}
        Destroy(gameObject);
    }

    public void ResetHealth()
    {
        buildingHP = buildingHPStore;
        health.SetActive(false);
    }

    //public void SetIndex(int buildingIndex)
    //{
    //    index = buildingIndex;                      -------------------------------------------
    //}
}
