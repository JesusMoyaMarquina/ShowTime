using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrainManagerScript : MonoBehaviour
{
    public static TrainManagerScript Instance;

    public GameObject meleeUnit, rangedUnit, bossUnit, player, screw;
    public bool attackingTrain;
    [SerializeField] private GameObject unitContainer, playerContainer;
    [SerializeField] private GameObject enemySpawnPoint, playerSpawnPoint, screwSpawnPoint;
    [SerializeField] private TextMeshProUGUI trainingModeText;
    private PlayerInput combatInput;



    void Start()
    {
        Instance = this;
        combatInput = GetComponent<PlayerInput>();
        attackingTrain = false;
        Instantiate(player, new Vector3(playerSpawnPoint.transform.position.x, playerSpawnPoint.transform.position.y, 0), Quaternion.identity, playerContainer.transform);
        Instantiate(screw, new Vector3(screwSpawnPoint.transform.position.x, screwSpawnPoint.transform.position.y, 0), Quaternion.identity, playerContainer.transform);
        GetEnemyType();
    }

    public void SwapTrainingMode()
    {
        attackingTrain = !attackingTrain;
        trainingModeText.text = trainingModeText.text.Replace(trainingModeText.text.Contains("Static") ? "Static" : "Combat",
                                                              trainingModeText.text.Contains("Static") ? "Combat" : "Static");
    }

    private void GetEnemyType()
    {
        if (TrainingZoneSettingsScript.Instance != null)
        {
            switch (TrainingZoneSettingsScript.Instance.GetEnemyType())
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
