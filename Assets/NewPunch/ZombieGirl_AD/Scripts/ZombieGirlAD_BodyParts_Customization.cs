using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieGirlAD_BodyParts_Customization : MonoBehaviour
{
    private int bodyTyp;
    private int lowerbodyTyp;
    private int tshirtTyp;
    private int hairTyp;
    private bool eyesTyp;
    private bool dHair;
    public GameObject[] body_Parts;
    public GameObject[] lowerBody_Parts;
   
    public GameObject torsoObject;
    public GameObject hairObject;
    public GameObject dynHairObject;
    public Material[] BodyMaterials = new Material[4];
    public Material[] HairMaterials = new Material[2];


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
    public bool dynamicHair = true;
    void Start()
    {
        
    }

    public void charCustomize(int body, int tshirt, int lowerbody, int hair, bool eyes, bool dhair)
    {




        Material[] mat;

        foreach (GameObject obj in body_Parts)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            renderer.material = BodyMaterials[body];
        }

        foreach (GameObject obj in lowerBody_Parts)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            renderer.material = BodyMaterials[lowerbody];
        }

        

        if (dhair)
        {


            hairObject.SetActive(false);
            dynHairObject.SetActive(true);

            Renderer skinRend = dynHairObject.GetComponent<Renderer>();
            skinRend.material = HairMaterials[hair];
        }
        else
        {
            hairObject.SetActive(true);
            dynHairObject.SetActive(false);


            Renderer skinRend = hairObject.GetComponent<Renderer>();
            skinRend.material = HairMaterials[hair];
        }



        Renderer tskinRend = torsoObject.GetComponent<Renderer>();
        mat = new Material[3];
        mat[0] = BodyMaterials[body];
        mat[1] = BodyMaterials[lowerbody];
        mat[2] = BodyMaterials[tshirt];

        tskinRend.materials = mat;

        if (eyes)
        {

            BodyMaterials[body].EnableKeyword("_EMISSION");
            BodyMaterials[body].SetFloat("_EmissiveExposureWeight", 0);


        }
        else
        {

            BodyMaterials[body].DisableKeyword("_EMISSION");
            BodyMaterials[body].SetFloat("_EmissiveExposureWeight", 1);


        }







    }

        void OnValidate()
    {
        //code for In Editor customize

        bodyTyp = (int)bodyType;
        lowerbodyTyp = (int)lowerbodyType;
        tshirtTyp = (int)tshirtType;
        hairTyp = (int)hairType;
        eyesTyp = (bool)eyesGlow;
        dHair = (bool)dynamicHair;
        charCustomize(bodyTyp, tshirtTyp, lowerbodyTyp, hairTyp, eyesTyp, dHair);

    }
}
