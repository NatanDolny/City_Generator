using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityConfig : MonoBehaviour
{
    public static int cityWidth = 0;
    public static int cityLength = 0;
    public static float roadPercentage = 0.2f;
    public static float smallToBigRatio = 0.35f;
    public static float extraFiller = 0.2f;

    public static void ResetValues()
    {
        cityWidth = 0;
        cityLength = 0;
        roadPercentage = 0.2f;
        smallToBigRatio = 0.35f;
        extraFiller = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
