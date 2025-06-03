// Lamp.cs
using UnityEngine;

public class Lamp : Building
{
    public Lamp() : base("Lamp", 0, 1000, 0, "", false) { }

    public override void UpgradePrefab()
    {
    }
}