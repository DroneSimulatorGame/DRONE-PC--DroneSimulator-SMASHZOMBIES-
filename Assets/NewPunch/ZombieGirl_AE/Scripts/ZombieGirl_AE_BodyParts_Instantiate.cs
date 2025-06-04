using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieGirl_AE_BodyParts_Instantiate : MonoBehaviour
{
    public Transform prefabObject;
    private int bodyTyp;
    private int lowerbodyTyp;
    private int sweaterTyp;
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

    public enum SweaterType
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
        V2,
        V3,
        V4
    }




    public BodyType bodyType;

    public SweaterType sweaterType;
    public LowerBodyType lowerbodyType;
    public HairType hairType;
    public bool eyesGlow;
    public bool dynamicHair = true;

    void Start()
    {
        Transform pref = Instantiate(prefabObject, gameObject.transform.position, gameObject.transform.rotation);

        bodyTyp = (int)bodyType;
        lowerbodyTyp = (int)lowerbodyType;
        sweaterTyp = (int)sweaterType;
        hairTyp = (int)hairType;
        eyesTyp = (bool)eyesGlow;
        dHair = (bool)dynamicHair;

        pref.gameObject.GetComponent<ZombieGirl_AE_BodyParts_Customization>().charCustomize(bodyTyp, sweaterTyp, lowerbodyTyp, hairTyp, eyesTyp, dHair);


    }
}
