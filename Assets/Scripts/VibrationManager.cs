//using System.Collections;
//using UnityEngine;

//public class VibrationManager : MonoBehaviour
//{
//    [Header("Vibration Profiles")]
//    public float gunFireRate = 0.1f; // Time between each vibration for gun shooting
//    public int gunVibrationStrength = 50; // Intensity for gun vibration (if using plugins)

//    public float explosionVibrationDuration = 0.5f; // Strong vibration duration for explosions
//    public int explosionVibrationStrength = 255; // Max intensity for explosion vibration

//    public float coinCollectionVibrationRate = 0.05f; // Time between each vibration for coin collection
//    public int coinVibrationStrength = 25; // Intensity for coin collection vibration

//    private bool canVibrate = true;

//    public void TriggerGunVibration(float duration)
//    {
//        StartCoroutine(GunVibrationRoutine(duration));
//    }

//    public void TriggerExplosionVibration()
//    {
//        if (canVibrate)
//        {
//            Handheld.Vibrate(); // Replace with a stronger vibration method if needed for specific platforms
//            StartCoroutine(ExplosionCooldown(explosionVibrationDuration));
//        }
//    }

//    public void TriggerCoinCollectionVibration(float duration)
//    {
//        StartCoroutine(CoinCollectionVibrationRoutine(duration));
//    }

//    private IEnumerator GunVibrationRoutine(float duration)
//    {
//        float endTime = Time.time + duration;
//        while (Time.time < endTime)
//        {
//            if (canVibrate)
//            {
//                Handheld.Vibrate();
//            }
//            yield return new WaitForSeconds(gunFireRate);
//        }
//    }

//    private IEnumerator CoinCollectionVibrationRoutine(float duration)
//    {
//        float endTime = Time.time + duration;
//        while (Time.time < endTime)
//        {
//            if (canVibrate)
//            {
//                Handheld.Vibrate();
//            }
//            yield return new WaitForSeconds(coinCollectionVibrationRate);
//        }
//    }

//    private IEnumerator ExplosionCooldown(float duration)
//    {
//        canVibrate = false;
//        yield return new WaitForSeconds(duration);
//        canVibrate = true;
//    }
//}
