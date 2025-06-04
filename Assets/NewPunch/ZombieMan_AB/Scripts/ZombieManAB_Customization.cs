using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieManAB_Customization : MonoBehaviour
{
    // Start is called before the first frame update

    private int bodySkn;
    private int shirtSkn;
    private int trousersSkn;
    private int eyesTyp;
    private int shirtVis;
   // public GameObject partsParent;
    public GameObject visiblePartsParent;
    public GameObject tohidePartsParent;
    public GameObject shirtParent;
    public GameObject trousersParent;
    public Material[] BodyMaterials = new Material[4];
    public Material[] ShirtMaterials = new Material[4];


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

    public enum ShirtVisible
    {
        No,
        Yes
    }


    public BodySkin bodySkin;
    public ShirtSkin shirtSkin;
    public TrousersSkin trousersSkin;
    public EyesGlow eyesGlow;
    public ShirtVisible shirtVisible;


    public void charCustomize(int body, int shirt, int trousers, int eyes, int shirtV)
    {
        Material[] mat;
       

        if (tohidePartsParent != null)
        {
            Renderer[] childRenderers = tohidePartsParent.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in childRenderers)
            {
                
                Material[] materials = renderer.sharedMaterials;

               
               
                if (materials.Length > 1)
                {
                    mat = new Material[2];
                    mat[0] = BodyMaterials[body];
                    mat[1] = BodyMaterials[trousers];

                    
                    renderer.materials = mat;
                }
                else
                {
                    renderer.sharedMaterial = BodyMaterials[body];
                }

            }

        }
        if (trousersParent != null)
        {
            Renderer[] childRenderers = trousersParent.GetComponentsInChildren<Renderer>();


            foreach (Renderer renderer in childRenderers)
            {
               
                renderer.sharedMaterial = BodyMaterials[trousers];
            }
        }

        if (shirtV == 0)
        {
            shirtParent.SetActive(false);
            tohidePartsParent.SetActive(true);
        }
        else
        {
            shirtParent.SetActive(true);
            tohidePartsParent.SetActive(false);
            Renderer[] shirtChildRenderers = shirtParent.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in shirtChildRenderers)
            {
                renderer.sharedMaterial = ShirtMaterials[shirt];
            }

        }

        if (visiblePartsParent != null)
        {
            Renderer[] childRenderers = visiblePartsParent.GetComponentsInChildren<Renderer>();

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
                
                Material[] materials = renderer.sharedMaterials;

               
                if(materials.Length > 1)
                {
                    mat = new Material[2];
                    mat[0] = BodyMaterials[body];
                    mat[1] = BodyMaterials[trousers];

                    
                    renderer.materials = mat;
                }
                else
                {
                    renderer.sharedMaterial = BodyMaterials[body];
                }
               
            }
        }



    }

    void OnValidate()
    {
        //code for In Editor customize

        bodySkn = (int)bodySkin;
        shirtSkn = (int)shirtSkin;
        trousersSkn = (int)trousersSkin;


        eyesTyp = (int)eyesGlow;
        shirtVis = (int)shirtVisible;

        charCustomize(bodySkn, shirtSkn, trousersSkn, eyesTyp, shirtVis);

    }
}