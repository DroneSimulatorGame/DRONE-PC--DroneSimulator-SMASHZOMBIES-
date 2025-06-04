using TMPro;
using UnityEngine;

public class DroneReturnCalculator : MonoBehaviour
{
    public GameObject droneObject; // Dron obyektini beriladi
    public GameObject initialPositionObject; // Boshlang'ich pozitsiya uchun obyekt
    public Rigidbody droneRigidbody; // Dronning Rigidbody-si (tezlikni hisoblash uchun)

    public TMP_Text distanceText; // Masofa uchun TMP text
    //public TMP_Text timeText; // Qolgan vaqt uchun TMP text
    //public TMP_Text speedText; // Tezlik uchun TMP text

    private int distanceToInitial; // Boshlang'ich pozitsiyaga masofa (butun sonlarda)
    private int minutes; // Daqiqalar
    private int seconds; // Sekundlar
    private Vector3 initialPosition;
    private float speed; // Dronning real vaqtdagi tezligi (m/s)
    private int kmhSpeed; // Dronning taxminiy tezligi km/h (1:100)

    private bool calculationActive = false; // Hisoblash faolligini belgilaydigan flag

    void Start()
    {
        if (droneObject != null && initialPositionObject != null)
        {
            initialPosition = initialPositionObject.transform.position; // Boshlang'ich pozitsiyani o'rnatish
        }
        else
        {
            Debug.LogError("Dron yoki boshlang'ich pozitsiya obyektlari aniqlanmagan.");
        }

        // UI larni dastlab yashirib qo'yamiz
        HideUI();
    }

    void Update()
    {
        if (calculationActive)
        {
            // Agar hisoblash faollashgan bo'lsa, doimiy ravishda masofa, vaqt va tezlikni yangilash
            CalculateRemainingDistanceAndTime();
        }
    }

    // Qolgan masofa va vaqtni hisoblaydigan metod
    public void CalculateRemainingDistanceAndTime()
    {
        // Hisoblashni faollashtiramiz
        calculationActive = true;

        // Boshlang'ich pozitsiyaga hozirgi masofani hisoblash
        distanceToInitial = Mathf.FloorToInt(Vector3.Distance(droneObject.transform.position, initialPosition)); // Butun sonlar bilan masofa

        // Agar masofa 5 metrdan kam bo'lsa, UI larni o'chirish va 0 qiymatlarni ko'rsatish
        if (distanceToInitial <=7)
        {
            distanceToInitial = 0;
            minutes = 0;
            seconds = 0;
            distanceText.text = "Distance: 0 m";
            //timeText.text = "Time: 0:00";
            //speedText.text = "Speed: 0 km/h";
            HideUI(); // UI larni o'chiramiz
            calculationActive = false; // Hisoblashni to'xtatamiz
            return;
        }

        ShowUI(); // UI larni faollashtiramiz

        // Tezlikni yangilab turamiz
        CalculateDroneSpeed();

        // Agar dronning tezligi 0 bo'lsa, vaqtni aniqlab bo'lmaydi
        if (speed > 0)
        {
            // Qolgan vaqt = masofa / tezlik
            float timeToReach = distanceToInitial / speed;

            // Vaqtni daqiqalar va sekundlarga ajratish
            minutes = Mathf.FloorToInt(timeToReach / 60); // Daqiqalar
            seconds = Mathf.FloorToInt(timeToReach % 60); // Sekundlar

            // TMP textlarni yangilash
            distanceText.text = "Distance: " + distanceToInitial + " m";
            //timeText.text = "Time: " + minutes + " min " + seconds + " sec";
            //speedText.text = "Speed: " + kmhSpeed + " km/h";

            Debug.Log("Qolgan masofa: " + distanceToInitial + " metr. Qolgan vaqt: " + minutes + " daqiqa " + seconds + " sekund. Tezlik: " + kmhSpeed + " km/h.");
        }
        else
        {
            distanceText.text = "Distance: " + distanceToInitial + " m";
            //timeText.text = "Time: Calculating...";
            //speedText.text = "Speed: 0 km/h";
        }
    }

    // Dron tezligini hisoblaydigan metod
    void CalculateDroneSpeed()
    {
        // Tezlikni hisoblash (Rigidbody orqali), m/s
        speed = droneRigidbody.velocity.magnitude;

        // Tezlikni km/h ga o'tkazish (1:100 nisbatda)
        kmhSpeed = Mathf.FloorToInt(speed * 5); // m/s dan km/h ga o'tkazish va 1:100 ko'rsatkich
    }

    // UI larning ko'rinishini yoqadigan metod
    void ShowUI()
    {
        distanceText.gameObject.SetActive(true);
        //timeText.gameObject.SetActive(true);
        //speedText.gameObject.SetActive(true);
    }

    // UI larning ko'rinishini o'chiradigan metod
    void HideUI()
    {
        distanceText.gameObject.SetActive(false);
        //timeText.gameObject.SetActive(false);
        //speedText.gameObject.SetActive(false);
    }
}
