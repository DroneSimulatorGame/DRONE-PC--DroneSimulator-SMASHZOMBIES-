using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WaveDataScript : ScriptableObject
{
    public List<WaveData> WavesData;
}

[Serializable]
public class WaveData
{
    [field: SerializeField]
    public string Name { get; private set; }
    [field: SerializeField]
    public int ID { get; private set; }
    [field: SerializeField]
    public int NormalZombieSoni { get; private set; }
    [field: SerializeField]
    public int GigantZombieSoni { get; private set; }
    [field: SerializeField]
    public int BombZombieSoni { get; private set; }
    [field: SerializeField]
    public int NormalZombieJoni { get; private set; }
    [field: SerializeField]
    public int GigantZombieJoni { get; private set; }
    [field: SerializeField]
    public int BombZombieJoni { get; private set; }
}