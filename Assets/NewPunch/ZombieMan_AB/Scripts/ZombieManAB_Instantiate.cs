using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieManAB_Instantiate : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform prefabObject;
    private int bodySkn;
    private int shirtSkn;
    private int trousersSkn;
    private int eyesTyp;
    private int shirtVis;
    


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

    void Start()
    {
        Transform pref = Instantiate(prefabObject, gameObject.transform.position, gameObject.transform.rotation);
        bodySkn = (int)bodySkin;
        shirtSkn = (int)shirtSkin;
        trousersSkn = (int)trousersSkin;


        eyesTyp = (int)eyesGlow;
        shirtVis = (int)shirtVisible;

        pref.gameObject.GetComponent<ZombieManAB_Customization>().charCustomize(bodySkn, shirtSkn, trousersSkn, eyesTyp, shirtVis);


    }

    
    
}
