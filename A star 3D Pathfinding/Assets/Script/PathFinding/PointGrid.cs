using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PointGrid : MonoBehaviour
{

    public Vector3 WorldPosition;

    public Vector3 WorldSize;

    public LayerMask UnWalkableLayers;


    public List<PointNode> grid = new List<PointNode>();




    float biggerBorder_X;
    float smallerBorder_X;
    float biggerBorder_Z;
    float smallerBorder_Z;


    private void Start()
    {

        CalculateWorldMapBorders();


        Generategrid();
    }

    void CalculateWorldMapBorders()
    {

        biggerBorder_X = WorldPosition.x + WorldSize.x / 2;
        smallerBorder_X = WorldPosition.x - WorldSize.x / 2;
        biggerBorder_Z = WorldPosition.z + WorldSize.z / 2;
        smallerBorder_Z = WorldPosition.z - WorldSize.z / 2;

    }

    public void Generategrid()
    {
        grid = new List<PointNode>();


        GameObject[] Objects = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];

        //Build Points Around Obstacles
        foreach(GameObject obj in Objects)
        {
            if (obj.layer == 8)
            {
                BuildPointNode(obj.transform);
            }
        }


        //Calculate Neighbors for each points
        for (int i = 0 ; i < grid.Count ; i++)
        {
            List<PointNode> neighbors = CalculateNeighbors(grid[i]);

            grid[i].AddNeighbors(neighbors);           
        }



    }


    void BuildPointNode(Transform obstacle)
    {

        float xSize = obstacle.transform.localScale.x / 2 + 0.35f;
        float zSize = obstacle.transform.localScale.z / 2 + 0.35f;

        Vector3 forwardDirection = obstacle.forward * zSize;
        Vector3 rightDirection = obstacle.right * xSize;

        //Build four point around obstacle
        for (int i=-1; i<=1;i=i+2)
        {
            for(int j=-1;j<=1;j=j+2)
            {

                Vector3 pos = i * forwardDirection + j * rightDirection + obstacle.position;
                //Check the new point is Walkable
                if (CheckIsWalkabel(pos))
                {
                    PointNode pointNode = new PointNode(pos);

                    grid.Add(pointNode);
                }

            }
        }



    }

    public PointNode GetPointNodeFromGridByPosition(Vector3 positions)
    {
        PointNode pointNode = new PointNode(positions);

        if(CheckIsWalkabel(positions))
        {
            pointNode.neighbors = CalculateNeighbors(pointNode);

            return pointNode;        
        }

        return null;
    }

    public List<PointNode> CalculateNeighbors(PointNode pointNode)
    {

        List<PointNode> neighbors = new List<PointNode>();


        for (int i=0;i<grid.Count;i++)
        {
            if(CheckIfPointsLinkedTogether(pointNode, grid[i])  && pointNode.position != grid[i].position)
            {
                neighbors.Add(grid[i]);
            }
        }

        return neighbors;

    }

    public bool CheckIfPointsLinkedTogether(PointNode point1,PointNode point2)
    {

        if (point1 == null || point2 == null)
        {
            return false;
        }


        if(Physics.Linecast(point1.position,point2.position,UnWalkableLayers))
        {
            return false;
        }



        return true;
    }

    bool CheckIsWalkabel(Vector3 pointNodePosition)
    {

        if (pointNodePosition.x > biggerBorder_X || pointNodePosition.x < smallerBorder_X)
        {
            return false;
        }

        if (pointNodePosition.z > biggerBorder_Z || pointNodePosition.z < smallerBorder_Z)
        {
            return false;
        }


        if (Physics.OverlapSphere(pointNodePosition, 0.2f, UnWalkableLayers).Length > 0  )
        {
            return false;
        }

        return true;
    }




    public void AddObstaclesPointsToGrid(GameObject obstacle)
    {
        for(int i=0;i<grid.Count;i++)
        {
            if( !CheckIsWalkabel( grid[i].position ) )
            {
                grid.RemoveAt(i);
            }
        }


        BuildPointNode(obstacle.transform);


        for(int i=grid.Count-4; i<grid.Count;i++)
        {
            List<PointNode> neighbors = CalculateNeighbors(grid[i]);

            grid[i].AddNeighbors(neighbors);
        }
        
        for (int i = 0; i < grid.Count; i++)
        {
            List<PointNode> neighbors = CalculateNeighbors(grid[i]);

            grid[i].AddNeighbors(neighbors);
        }

    }







}