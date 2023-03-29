using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrderInLayerScript : MonoBehaviour
{
    private List<GameObject> units;

    private void Start()
    {
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        CreateArray();
    }

    private void CreateArray()
    {
        units = new List<GameObject>();
        
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy")) 
            units.Add(enemy);

        foreach (var player in GameObject.FindGameObjectsWithTag("Player")) 
            units.Add(player);
        
        units = units.OrderBy(unit => unit.transform.position.y * -1).ToList();
        
        for (int i = 0; i < units.Count; i++) 
            units[i].GetComponent<SpriteRenderer>().sortingOrder = i;
    }
}
