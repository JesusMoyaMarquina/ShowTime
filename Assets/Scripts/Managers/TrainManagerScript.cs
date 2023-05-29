using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainManagerScript : MonoBehaviour
{
    public static TrainManagerScript Instance;

    public GameObject meleeUnit, rangedUnit, bossUnit, player;
    [SerializeField] private GameObject unitContainer, playerContainer;
    [SerializeField] private GameObject enemySpawnPoint, playerSpawnPoint;

    void Start()
    {
        Instance = this;
        Instantiate(player, new Vector3(playerSpawnPoint.transform.position.x, playerSpawnPoint.transform.position.y, 0), Quaternion.identity, playerContainer.transform);
        GetDifficulty();
    }

    private void Update()
    {
        HandleTrainInputs();
    }

    private void HandleTrainInputs()
    {

    }

    private void GetDifficulty()
    {
        if (SelectDifficultyScript.Instance != null)
        {
            switch (SelectDifficultyScript.Instance.GetDifficulty())
            {
                case 0:
                    Instantiate(meleeUnit, new Vector3(enemySpawnPoint.transform.position.x, enemySpawnPoint.transform.position.y, 0), Quaternion.identity, unitContainer.transform);
                    break;
                case 1:
                    Instantiate(rangedUnit, new Vector3(enemySpawnPoint.transform.position.x, enemySpawnPoint.transform.position.y, 0), Quaternion.identity, unitContainer.transform);
                    break;
                case 2:
                    Instantiate(bossUnit, new Vector3(enemySpawnPoint.transform.position.x, enemySpawnPoint.transform.position.y, 0), Quaternion.identity, unitContainer.transform);
                    break;
                default:
                    break;
            }
        }
    }
}
