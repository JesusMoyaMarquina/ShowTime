using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public GameObject meleeUnit;
    public GameObject rangedUnit;
    public GameObject unitContaner;
    public GameObject[] spawnAreas;
    public int minMeleePercentage, maxMeleePercentage, minRangedPercentage;
    public float meleeUnitPercentage, rangedUnitPercentage;

    // Start is called before the first frame update
    void Start()
    {
        GetSpawnAreas();
        unitContaner = GameObject.Find("Units");
    }

    public void GenerateUnits(int numberOfUnits)
    {
        if (spawnAreas.Length <= 0)
        {
            return;
        }

        bool addExtra = false;

        if (maxMeleePercentage == 0 && minRangedPercentage == 0)
        {
            maxMeleePercentage = 70;
            minRangedPercentage = 30;
        }

        float totalUnitsPercentage = maxMeleePercentage + minRangedPercentage;

        meleeUnitPercentage = Random.Range(minMeleePercentage / totalUnitsPercentage, maxMeleePercentage / totalUnitsPercentage);
        rangedUnitPercentage = 1 - meleeUnitPercentage;

        if (numberOfUnits / spawnAreas.Length * spawnAreas.Length < numberOfUnits)
        {
            addExtra = true;
        }

        foreach (GameObject area in spawnAreas)
        {
            int areaSpawnUnits = numberOfUnits / spawnAreas.Length;
            int meleeUnitsToSpawn = Mathf.RoundToInt(areaSpawnUnits * meleeUnitPercentage);
            int rangedUnitsToSpawn = Mathf.RoundToInt(areaSpawnUnits * rangedUnitPercentage);

            while (meleeUnitsToSpawn > 0) 
            {
                float spawnXPos = Random.Range(- area.transform.localScale.x / 2, area.transform.localScale.x / 2) + area.transform.position.x;
                float spawnYPos = Random.Range(- area.transform.localScale.y / 2, area.transform.localScale.y / 2) + area.transform.position.y;

                Instantiate(meleeUnit, new Vector3(spawnXPos, spawnYPos, 0), Quaternion.identity, unitContaner.transform);

                meleeUnitsToSpawn--;
            }

            while(rangedUnitsToSpawn > 0)
            {
                float spawnXPos = Random.Range(- area.transform.localScale.x / 2, area.transform.localScale.x / 2) + area.transform.position.x;
                float spawnYPos = Random.Range(- area.transform.localScale.y / 2, area.transform.localScale.y / 2) + area.transform.position.y;

                Instantiate(rangedUnit, new Vector3(spawnXPos, spawnYPos, 0), Quaternion.identity, unitContaner.transform);

                rangedUnitsToSpawn--;
            }
        }

        if (addExtra){
            GameObject area = spawnAreas[spawnAreas.Length - 1];

            float spawnXPos = Random.Range(-area.transform.localScale.x / 2, area.transform.localScale.x / 2) + area.transform.position.x;
            float spawnYPos = Random.Range(-area.transform.localScale.y / 2, area.transform.localScale.y / 2) + area.transform.position.y;

            Instantiate(meleeUnit, new Vector3(spawnXPos, spawnYPos, 0), Quaternion.identity, unitContaner.transform);
        }
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
