using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public GameObject meleeUnit;
    public GameObject rangedUnit;
    public GameObject unitContaner;
    public GameObject[] spawnAreas;
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

        if (numberOfUnits / spawnAreas.Length * spawnAreas.Length < numberOfUnits)
        {
            addExtra = true;
        }

        int areaSpawnUnits = Mathf.CeilToInt((float) numberOfUnits / spawnAreas.Length);
        int meleeUnitsToSpawn = Mathf.RoundToInt(areaSpawnUnits * meleeUnitPercentage);
        int rangedUnitsToSpawn = Mathf.RoundToInt(areaSpawnUnits * rangedUnitPercentage);

        foreach (GameObject area in spawnAreas)
        {

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

        if (addExtra && meleeUnitsToSpawn > 0 || rangedUnitsToSpawn > 0)
        {
            GameObject area = spawnAreas[spawnAreas.Length - 1];

            float spawnXPos = Random.Range(-area.transform.localScale.x / 2, area.transform.localScale.x / 2) + area.transform.position.x;
            float spawnYPos = Random.Range(-area.transform.localScale.y / 2, area.transform.localScale.y / 2) + area.transform.position.y;

            if (meleeUnitsToSpawn < rangedUnitsToSpawn)
            {
                Instantiate(meleeUnit, new Vector3(spawnXPos, spawnYPos, 0), Quaternion.identity, unitContaner.transform);
            }
            else
            {
                Instantiate(rangedUnit, new Vector3(spawnXPos, spawnYPos, 0), Quaternion.identity, unitContaner.transform);
            }
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
