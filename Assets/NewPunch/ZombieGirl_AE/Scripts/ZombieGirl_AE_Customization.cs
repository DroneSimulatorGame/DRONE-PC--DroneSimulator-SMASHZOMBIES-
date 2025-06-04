using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieGirl_AE_Customization : MonoBehaviour
{

    private int bodyTyp;
    private int lowerbodyTyp;
    private int sweaterTyp;
    private int hairTyp;
    private bool eyesTyp;
    private bool dHair;
    public GameObject partsParent;
    public GameObject hairObject;
   
    public Material[] BodyMaterials = new Material[4];
    public Material[] HairMaterials = new Material[4];

    private Cloth myCloth;


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


    public void charCustomize(int body, int sweater, int lowerbody, int hair, bool eyes, bool dhair)
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
                    mat[0] = BodyMaterials[body];
                    mat[2] = BodyMaterials[sweater];
                    mat[1] = BodyMaterials[lowerbody];

                    renderer.materials = mat;
                }

                myCloth = hairObject.GetComponent<Cloth>();
                Renderer skinRend = hairObject.GetComponent<Renderer>();
                skinRend.material = HairMaterials[hair];
                if (dhair)
                {


                   

                    myCloth.enabled = true;
                   
                   
                }
                else
                {
                  
                   
                    myCloth.enabled = false;
                   
                }



            }
        }







    }

    void OnValidate()
    {
        //code for In Editor customize

        bodyTyp = (int)bodyType;
        lowerbodyTyp = (int)lowerbodyType;
        sweaterTyp = (int)sweaterType;
        hairTyp = (int)hairType;
        eyesTyp = (bool)eyesGlow;
        dHair = (bool)dynamicHair;

        charCustomize(bodyTyp, sweaterTyp, lowerbodyTyp, hairTyp, eyesTyp, dHair);

    }
}
