using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{

    public int generateUnit, unitIncremental;
    private int previousGenerateUnit;
    public GameObject meleeUnit, rangedUnit, bossUnit;
    public GameObject unitContaner;
    public GameObject bossSpawnPoint;
    public GameObject[] spawnAreas;
    public float meleeUnitPercentage, rangedUnitPercentage;

    void Start()
    {
        GetSpawnAreas();
        GetDifficulty();
        unitContaner = GameObject.Find("Units");
    }

    private void GetDifficulty()
    {
        if (SelectDifficultyScript.Instance != null)
        {
            switch (SelectDifficultyScript.Instance.GetDifficulty())
            {
                case 0:
                    generateUnit = 4;
                    unitIncremental = 2;
                    break;
                case 1:
                    generateUnit = 6;
                    unitIncremental = 2;
                    break;
                case 2:
                    generateUnit = 6;
                    unitIncremental = 4;
                    break;
                default:
                    break;
            }
        }
    }

    public void IncrementUntinsToGenerate()
    {
        generateUnit += unitIncremental;
    }

    public void GenerateUnits()
    {
        if (spawnAreas.Length <= 0)
        {
            return;
        }

        bool addExtra = !(generateUnit % spawnAreas.Length == 0);
        int unitToGenerate = generateUnit;

        for (int i = 0; i < spawnAreas.Length; i++)
        {
            GameObject area = spawnAreas[i];

            float areaSpawnUnits = (float) generateUnit / spawnAreas.Length;
            int meleeUnitsToSpawn = Mathf.RoundToInt(areaSpawnUnits * meleeUnitPercentage);
            int rangedUnitsToSpawn = Mathf.RoundToInt(areaSpawnUnits * rangedUnitPercentage);

            bool addZoneExtra = false;

            while (meleeUnitsToSpawn > 0 && areaSpawnUnits > 0 && unitToGenerate > 0) 
            {
                float spawnXPos = Random.Range(- area.transform.localScale.x / 2, area.transform.localScale.x / 2) + area.transform.position.x;
                float spawnYPos = Random.Range(- area.transform.localScale.y / 2, area.transform.localScale.y / 2) + area.transform.position.y;

                Instantiate(meleeUnit, new Vector3(spawnXPos, spawnYPos, 0), Quaternion.identity, unitContaner.transform);

                meleeUnitsToSpawn--;
                areaSpawnUnits--;
                unitToGenerate--;
            }

            while(rangedUnitsToSpawn > 0 && areaSpawnUnits > 0 && unitToGenerate > 0)
            {
                float spawnXPos = Random.Range(- area.transform.localScale.x / 2, area.transform.localScale.x / 2) + area.transform.position.x;
                float spawnYPos = Random.Range(- area.transform.localScale.y / 2, area.transform.localScale.y / 2) + area.transform.position.y;

                Instantiate(rangedUnit, new Vector3(spawnXPos, spawnYPos, 0), Quaternion.identity, unitContaner.transform);

                rangedUnitsToSpawn--;
                areaSpawnUnits--;
                unitToGenerate--;
            }

            if (areaSpawnUnits >= 1 && unitToGenerate > 0)
            {
                addZoneExtra = true;
            }

            if ((addZoneExtra || addExtra && i == spawnAreas.Length - 1) && unitToGenerate > 0)
            {
                float spawnXPos = Random.Range(-area.transform.localScale.x / 2, area.transform.localScale.x / 2) + area.transform.position.x;
                float spawnYPos = Random.Range(-area.transform.localScale.y / 2, area.transform.localScale.y / 2) + area.transform.position.y;

                if (meleeUnitPercentage >= rangedUnitPercentage)
                {
                    Instantiate(meleeUnit, new Vector3(spawnXPos, spawnYPos, 0), Quaternion.identity, unitContaner.transform);
                }
                else if (meleeUnitPercentage < rangedUnitPercentage)
                {
                    Instantiate(rangedUnit, new Vector3(spawnXPos, spawnYPos, 0), Quaternion.identity, unitContaner.transform);
                }
                addExtra = false;
                unitToGenerate--;
            }
        }
    }

    public GameObject GenerateBoss()
    {
        return Instantiate(bossUnit, new Vector3(bossSpawnPoint.transform.position.x, bossSpawnPoint.transform.position.y, 0), Quaternion.identity, unitContaner.transform);
    }

    private void GetSpawnAreas()
    {
        GameObject spawnAreasGO = GameObject.Find("SpawnAreas");
        spawnAreas = new GameObject[spawnAreasGO.transform.childCount];
        for (int i = 0; i < spawnAreas.Length; i++)
        {
            spawnAreas[i] = spawnAreasGO.transform.GetChild(i).gameObject;
        }
    }
}
