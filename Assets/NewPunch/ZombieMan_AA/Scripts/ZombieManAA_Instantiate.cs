using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieManAA_Instantiate : MonoBehaviour
{
    private int bodyTyp;
    private int eyesTyp;
    public Transform prefabObject;
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

    void Start()
    {
        Transform pref = Instantiate(prefabObject, gameObject.transform.position, gameObject.transform.rotation);
        bodyTyp = (int)bodyType;
        eyesTyp = (int)eyesGlow;

        pref.gameObject.GetComponent<ZombieManAA_Customization>().charCustomize(bodyTyp, eyesTyp);


    }

}