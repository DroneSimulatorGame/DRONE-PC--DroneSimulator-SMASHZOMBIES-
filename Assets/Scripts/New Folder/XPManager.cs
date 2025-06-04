using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XPManager : MonoBehaviour
{
    public Slider sliderXP;
    public Gradient color;
    public Image fill;

    public int value = 0;
    public int xpLevel = 1;

    private void Update()
    {
        sliderXP.value = value;
    }
}
