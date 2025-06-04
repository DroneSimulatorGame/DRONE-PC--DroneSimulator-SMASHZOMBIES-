using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieManAC_Customization : MonoBehaviour
{
    // Start is called before the first frame update

    private int bodySkn;
    private int shirtSkn;
    private int trousersSkn;
    private int eyesTyp;
   
   
    public GameObject headA_Parent;
    public GameObject headB_Parent;
    public GameObject shirtParent;
    public GameObject trousersParent;
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
        //Material[] mat;

        if(body > 1)
        {
            headA_Parent.SetActive(false);
            headB_Parent.SetActive(true);


            if (headB_Parent != null)
            {
                Renderer[] childRenderers = headB_Parent.GetComponentsInChildren<Renderer>();

                foreach (Renderer renderer in childRenderers)
                {

                    renderer.sharedMaterial = BodyMaterials[body];
                }

            }

        }
        else
        {
            headB_Parent.SetActive(false);
            headA_Parent.SetActive(true);

            if (headA_Parent != null)
            {
                Renderer[] childRenderers = headA_Parent.GetComponentsInChildren<Renderer>();

                foreach (Renderer renderer in childRenderers)
                {

                    renderer.sharedMaterial = BodyMaterials[body];
                }

            }
        }

        //


        if (trousersParent != null)
        {
            Renderer[] childRenderers = trousersParent.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in childRenderers)
            {

                renderer.sharedMaterial = BodyMaterials[trousers];
            }

        }

        //


        if (shirtParent != null)
        {
            Renderer[] childRenderers = shirtParent.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in childRenderers)
            {

                renderer.sharedMaterial = BodyMaterials[shirt];
            }

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