using UnityEngine;

public class TimerManager : MonoBehaviour
{
    [SerializeField] Timer architect;
    [SerializeField] Timer command;
    [SerializeField] Timer payload;
    [SerializeField] Timer workshop;
    [SerializeField] Timer Tower1;
    [SerializeField] Timer Tower2;
    [SerializeField] Timer Tower3;
    [SerializeField] Timer Tower4;

    // Method for setting and activating Tower 1 timer
    public void StartTower1Timer(int duration)
    {
        Tower1
            .SetTimerFromExternal(duration)
            .OnEnd(() => Tower1.ResetAndHide())  // Automatically hides when time is up
            .Begin();
    }

    // Method for setting and activating Tower 2 timer
    public void StartTower2Timer(int duration)
    {
        Tower2
            .SetTimerFromExternal(duration)
            .OnEnd(() => Tower2.ResetAndHide())
            .Begin();
    }

    // Method for setting and activating Tower 3 timer
    public void StartTower3Timer(int duration)
    {
        Tower3
            .SetTimerFromExternal(duration)
            .OnEnd(() => Tower3.ResetAndHide())
            .Begin();
    }

    // Method for setting and activating Tower 4 timer
    public void StartTower4Timer(int duration)
    {
        Tower4
            .SetTimerFromExternal(duration)
            .OnEnd(() => Tower4.ResetAndHide())
            .Begin();
    }

    // Method for setting and activating Payload timer
    public void StartPayloadTimer(int duration)
    {
        payload
            .SetTimerFromExternal(duration)
            .OnEnd(() => payload.ResetAndHide())
            .Begin();
    }

    // Method for setting and activating Architect timer
    public void StartArchitectTimer(int duration)
    {
        architect
            .SetTimerFromExternal(duration)
            .OnEnd(() => architect.ResetAndHide())
            .Begin();
    }

    // Method for setting and activating Command timer
    public void StartCommandTimer(int duration)
    {
        command
            .SetTimerFromExternal(duration)
            .OnEnd(() => command.ResetAndHide())
            .Begin();
    }

    // Method for setting and activating Workshop timer
    public void StartWorkshopTimer(int duration)
    {
        workshop
            .SetTimerFromExternal(duration)
            .OnEnd(() => workshop.ResetAndHide())
            .Begin();
    }

    // New methods to cut time by 15 minutes for each timer
    public void CutTower1Time()
    {
        Tower1.ReduceTime(15 * 60); // 15 minutes in seconds
    }

    public void CutTower2Time()
    {
        Tower2.ReduceTime(15 * 60);
    }

    public void CutTower3Time()
    {
        Tower3.ReduceTime(15 * 60);
    }

    public void CutTower4Time()
    {
        Tower4.ReduceTime(15 * 60);
    }

    public void CutArchitectTime()
    {
        architect.ReduceTime(15 * 60);
    }

    public void CutCommandTime()
    {
        command.ReduceTime(15 * 60);
    }

    public void CutWorkshopTime()
    {
        workshop.ReduceTime(15 * 60);
    }

    public void CutPayloadTime()
    {
        payload.ReduceTime(15 * 60);
    }
}
