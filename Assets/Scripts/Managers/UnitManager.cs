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
            print(SelectDifficultyScript.Instance.GetDifficulty());
            switch (SelectDifficultyScript.Instance.GetDifficulty())
            {
                case 0:
                    generateUnit = 3;
                    unitIncremental = 0;
                    break;
                case 1:
                    generateUnit = 3;
                    unitIncremental = 3;
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
        previousGenerateUnit = generateUnit;

        if (spawnAreas.Length <= 0)
        {
            return;
        }

        bool addExtra = false;

        foreach (GameObject area in spawnAreas)
        {
            float areaSpawnUnits = (float) generateUnit / spawnAreas.Length;
            int meleeUnitsToSpawn = Mathf.RoundToInt(areaSpawnUnits * meleeUnitPercentage);
            int rangedUnitsToSpawn = Mathf.RoundToInt(areaSpawnUnits * rangedUnitPercentage);

            if (areaSpawnUnits - meleeUnitsToSpawn - rangedUnitsToSpawn > 0)
            {
                addExtra = true;
            }

            while (meleeUnitsToSpawn > 0 && generateUnit > 0) 
            {
                float spawnXPos = Random.Range(- area.transform.localScale.x / 2, area.transform.localScale.x / 2) + area.transform.position.x;
                float spawnYPos = Random.Range(- area.transform.localScale.y / 2, area.transform.localScale.y / 2) + area.transform.position.y;

                Instantiate(meleeUnit, new Vector3(spawnXPos, spawnYPos, 0), Quaternion.identity, unitContaner.transform);

                meleeUnitsToSpawn--;
                generateUnit--;
            }

            while(rangedUnitsToSpawn > 0 && generateUnit > 0)
            {
                float spawnXPos = Random.Range(- area.transform.localScale.x / 2, area.transform.localScale.x / 2) + area.transform.position.x;
                float spawnYPos = Random.Range(- area.transform.localScale.y / 2, area.transform.localScale.y / 2) + area.transform.position.y;

                Instantiate(rangedUnit, new Vector3(spawnXPos, spawnYPos, 0), Quaternion.identity, unitContaner.transform);

                rangedUnitsToSpawn--;
                generateUnit--;
            }

            if (addExtra && generateUnit > 0)
            {
                float spawnXPos = Random.Range(-area.transform.localScale.x / 2, area.transform.localScale.x / 2) + area.transform.position.x;
                float spawnYPos = Random.Range(-area.transform.localScale.y / 2, area.transform.localScale.y / 2) + area.transform.position.y;

                if (meleeUnitPercentage > rangedUnitPercentage)
                {
                    Instantiate(meleeUnit, new Vector3(spawnXPos, spawnYPos, 0), Quaternion.identity, unitContaner.transform);
                }
                else
                {
                    Instantiate(rangedUnit, new Vector3(spawnXPos, spawnYPos, 0), Quaternion.identity, unitContaner.transform);
                }
                generateUnit--;
            }
        }

        generateUnit = previousGenerateUnit;
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
