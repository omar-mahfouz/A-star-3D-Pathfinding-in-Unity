using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesBuilder : MonoBehaviour
{
    public GameObject obstaclePrefab;



    private PointGrid grid;

    private void Start()
    {
        grid = Astar_Manager.Singleton.gameObject.GetComponent<PointGrid>();
    }

    private void Update()
    {

            if(Input.GetMouseButtonDown(1))
            {
               AddObstacles();
            }



    }

    



    void AddObstacles()
    {
        Vector3 Pos = Input.mousePosition;
        Pos.z = 20;

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Pos);

        GameObject obstacle = Instantiate(obstaclePrefab, mouseWorldPosition, new Quaternion(0, 0, 0, 0));

        obstacle.transform.position = new Vector3(obstacle.transform.position.x, obstacle.transform.localScale.y/2, obstacle.transform.position.z);

        grid.AddObstaclesPointsToGrid(obstacle);

    }
}
