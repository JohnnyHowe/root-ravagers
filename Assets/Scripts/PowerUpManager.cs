using System.Collections.Generic;
using UnityEngine;


public class PowerUpManager : MonoBehaviour
{
    public List<PowerUp> PowerUps;
    public List<PowerUp> InstantiatedPowerUps;
    public Transform SpawnLocationsContainer;
    public float PowerUpSpawnPeriod = 1f;
    private float _timeUntilPowerUpSpawn;

    public List<PowerUp> GetAvailablePowerUps()
    {
        List<PowerUp> availablePowerUps = new List<PowerUp>();
        foreach (PowerUp p in InstantiatedPowerUps)
        {
            if (p.IsAvailable)
            {
                availablePowerUps.Add(p);
            }
        }
        return availablePowerUps;
    }

    void Awake()
    {
        _timeUntilPowerUpSpawn = PowerUpSpawnPeriod;
    }

    void Update()
    {
        if (_ShouldSpawnPowerUp())
        {
            _SpawnNewPowerUp();
        }
        _ForgetDeadPowerUps();
    }

    private void _ForgetDeadPowerUps()
    {
        List<PowerUp> n = new List<PowerUp>();
        foreach (PowerUp p in InstantiatedPowerUps)
        {
            if (p != null) n.Add(p);
        }
        InstantiatedPowerUps = n;
    }

    private bool _ShouldSpawnPowerUp()
    {
        if (_timeUntilPowerUpSpawn < 0)
        {
            _timeUntilPowerUpSpawn += PowerUpSpawnPeriod;
            return true;
        }
        _timeUntilPowerUpSpawn -= Time.deltaTime;
        return false;
    }

    private void _SpawnNewPowerUp()
    {
        PowerUp p = Instantiate(GetNextPowerUpChoice());
        p.transform.parent = transform;
        p.transform.position = GetNextPowerUpSpawnLocation();
        InstantiatedPowerUps.Add(p);
    }

    private PowerUp GetNextPowerUpChoice()
    {
        if (PowerUps.Count == 0) return null;
        int choiceIndex = Random.Range(0, PowerUps.Count - 1);
        return PowerUps[choiceIndex];
    }

    private Vector3 GetNextPowerUpSpawnLocation()
    {
        return SpawnLocationsContainer.GetChild(Random.Range(0, SpawnLocationsContainer.childCount - 1)).position;
    }
}