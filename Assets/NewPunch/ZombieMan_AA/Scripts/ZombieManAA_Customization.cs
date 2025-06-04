using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieManAA_Customization : MonoBehaviour
{
    // Start is called before the first frame update

    private int bodyTyp;
    private int eyesTyp;
    public GameObject partsParent;
    public Material[] BodyMaterials = new Material[4];


    public enum BodyType
    {
        V1,
        V2,
        V3,
        V4
    }

    public enum EyesGlow
    {
        No,
        Yes
    }


    public BodyType bodyType;
    public EyesGlow eyesGlow;


    public void charCustomize(int body, int eyes)
    {

        if (partsParent != null)
        {
            Renderer[] childRenderers = partsParent.GetComponentsInChildren<Renderer>();

            if (eyes == 0)
            {



                BodyMaterials[body].DisableKeyword("_EMISSION");
                BodyMaterials[body].SetFloat("_EmissiveExposureWeight", 1);
            }
            else
            {


                BodyMaterials[body].EnableKeyword("_EMISSION");
                BodyMaterials[body].SetFloat("_EmissiveExposureWeight", 0);

            }


            foreach (Renderer renderer in childRenderers)
            {
                renderer.sharedMaterial = BodyMaterials[body];
            }
        }



    }

    void OnValidate()
    {
        //code for In Editor customize

        bodyTyp = (int)bodyType;
        eyesTyp = (int)eyesGlow;

        charCustomize(bodyTyp, eyesTyp);

    }
}