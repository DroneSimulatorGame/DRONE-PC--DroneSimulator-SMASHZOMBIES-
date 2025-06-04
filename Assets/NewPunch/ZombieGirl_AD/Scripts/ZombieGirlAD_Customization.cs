using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieGirlAD_Customization : MonoBehaviour
{
    
    private int bodyTyp;
    private int lowerbodyTyp;
    private int tshirtTyp;
    private int hairTyp;
    private bool eyesTyp;
    private bool dHair;
    public GameObject partsParent;
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


    public void charCustomize(int body, int tshirt, int lowerbody, int hair, bool eyes, bool dhair)
    {


        Material[] mat;

        

        if (partsParent != null)
        {
            Renderer[] childRenderers = partsParent.GetComponentsInChildren<Renderer>();

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


            foreach (Renderer renderer in childRenderers)
            {

                Material[] materials = renderer.sharedMaterials;

                if (materials.Length > 1)
                {
                    mat = new Material[3];
                    mat[2] = BodyMaterials[body];
                    mat[1] = BodyMaterials[tshirt];
                    mat[0] = BodyMaterials[lowerbody];

                    renderer.materials = mat;
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



            }
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

