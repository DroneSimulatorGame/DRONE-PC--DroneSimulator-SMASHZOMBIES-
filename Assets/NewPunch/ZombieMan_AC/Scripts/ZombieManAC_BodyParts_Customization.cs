using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieManAC_BodyParts_Customization : MonoBehaviour
{
    // Start is called before the first frame update

    private int bodySkn;
    private int shirtSkn;
    private int trousersSkn;
    private int eyesTyp;


    public GameObject[] headA_OBJ;
    public GameObject[] headB_OBJ;
    public GameObject[] hands_OBJ;

   
    public GameObject[] trousersOBJ;
    public Material[] BodyMaterials = new Material[4];



    public enum BodySkin
    {
        V1,
        V2,
        V3,
        V4
    }

    public enum ShirtSkin
    {
        V1,
        V2,
        V3,
        V4
    }

    public enum TrousersSkin
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




    public BodySkin bodySkin;
    public ShirtSkin shirtSkin;
    public TrousersSkin trousersSkin;
    public EyesGlow eyesGlow;

    public void charCustomize(int body, int shirt, int trousers, int eyes)
    {
        Material[] mat;
        if (body > 1)
        {
            headA_OBJ[0].SetActive(false);
            headB_OBJ[0].SetActive(true);
            headA_OBJ[1].SetActive(false);
            headB_OBJ[1].SetActive(true);


            Renderer renderer = headB_OBJ[0].GetComponent<Renderer>();
            renderer.material = BodyMaterials[body];

            renderer = headB_OBJ[1].GetComponent<Renderer>();
            mat = new Material[3];
            mat[0] = BodyMaterials[body];
            mat[1] = BodyMaterials[shirt];
            mat[2] = BodyMaterials[trousers];

            renderer.materials = mat;


        }
        else
        {
            headB_OBJ[0].SetActive(false);
            headA_OBJ[0].SetActive(true);
            headB_OBJ[1].SetActive(false);
            headA_OBJ[1].SetActive(true);

            Renderer renderer = headA_OBJ[0].GetComponent<Renderer>();
            renderer.material = BodyMaterials[body];

            renderer = headA_OBJ[1].GetComponent<Renderer>();
            mat = new Material[3];
            mat[0] = BodyMaterials[body];
            mat[1] = BodyMaterials[shirt];
            mat[2] = BodyMaterials[trousers];

            renderer.materials = mat;
        }

        foreach (GameObject obj in hands_OBJ)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            Material[] materials = renderer.sharedMaterials;

           

            if (materials.Length > 1)
            {
                mat = new Material[2];
                mat[0] = BodyMaterials[body];
                mat[1] = BodyMaterials[shirt];


                renderer.materials = mat;
            }
            else
            {
              
                renderer.material = BodyMaterials[body];
            }


        }




        foreach (GameObject obj in trousersOBJ)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            renderer.material = BodyMaterials[trousers];
        }

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

    }
    void OnValidate()
    {
        //code for In Editor customize

        bodySkn = (int)bodySkin;
        shirtSkn = (int)shirtSkin;
        trousersSkn = (int)trousersSkin;


        eyesTyp = (int)eyesGlow;


        charCustomize(bodySkn, shirtSkn, trousersSkn, eyesTyp);

    }
}
