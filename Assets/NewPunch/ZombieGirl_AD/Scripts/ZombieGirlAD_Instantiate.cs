using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieGirlAD_Instantiate : MonoBehaviour
{


    public Transform prefabObject;
    private int bodyTyp;
    private int lowerbodyTyp;
    private int tshirtTyp;
    private int hairTyp;
    private bool eyesTyp;
    private bool dHair;



    public enum BodyType
    {
        V1,
        V2,
        V3,
        V4
    }

    public enum TshirtType
    {
        V1,
        V2,
        V3,
        V4
    }

    public enum LowerBodyType
    {
        V1,
        V2,
        V3,
        V4
    }

    public enum HairType
    {
        V1,
        V2
    }




    public BodyType bodyType;

    public TshirtType tshirtType;
    public LowerBodyType lowerbodyType;
    public HairType hairType;
    public bool eyesGlow;
    public bool dynamicHair;

    void Start()
    {
        Transform pref = Instantiate(prefabObject, gameObject.transform.position, gameObject.transform.rotation);
        bodyTyp = (int)bodyType;
        lowerbodyTyp = (int)lowerbodyType;
        tshirtTyp = (int)tshirtType;
        hairTyp = (int)hairType;
        eyesTyp = (bool)eyesGlow;
        dHair = (bool)dynamicHair;

        pref.gameObject.GetComponent<ZombieGirlAD_Customization>().charCustomize(bodyTyp, tshirtTyp, lowerbodyTyp, hairTyp, eyesTyp, dHair);


    }



}

