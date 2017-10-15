using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;


public class PathRequestManager : MonoBehaviour
{

    public bool multiThreading = false;

    //Store The Response Result
    Queue<PathResponse> _results = new Queue<PathResponse>();


    public static PathRequestManager Singleton;

    private void Awake()
    {
        if(Singleton == null)
        {
            Singleton = this;
        }
    }

    private void Update()
    {
        CallBackTheResult();
    }


    // Call back The Result to Each Agent Request
    void CallBackTheResult()
    {
        if (_results.Count > 0)
        {

            lock (_results)
            {
                for (int i = 0; i < _results.Count; i++)
                {
                    PathResponse pathResponse = _results.Dequeue();

                    pathResponse.callBack(pathResponse.path, pathResponse.succes);
                }
            }
        }
    }

    public void Request(PathRequest pathRequest)
    {

        if(multiThreading)
        {
            //Make New Thread for Finding Path
            ThreadStart threadStart = delegate
            {
                Astar_Manager.Singleton.FindingPath(pathRequest, FinishProcessing);
            };

            threadStart.Invoke();
        }
        else
        {
            Astar_Manager.Singleton.FindingPath(pathRequest, FinishProcessing);
        }

    }


    public void FinishProcessing(PathResponse pathResponse)
    {
        if(multiThreading)
        {
            //Add New Result To Queue Result To Call Back it 
            lock (_results)
            {
                _results.Enqueue(pathResponse);
            }
        }
        else
        {
            pathResponse.callBack(pathResponse.path, pathResponse.succes);
        }


    }

}


public struct PathResponse
{
    public Vector3[] path;
    public bool succes;
    public Action<Vector3[], bool> callBack;

    public PathResponse(Vector3[] path, bool succes, Action<Vector3[], bool> callBack)
    {
        this.path = path;
        this.succes = succes;
        this.callBack = callBack;
    }

}


public struct PathRequest
{
    public Vector3 startNode;
    public Vector3 targetNode;
    public Action<Vector3[], bool> callBack;

    public PathRequest(Vector3 startNode, Vector3 targetNode, Action<Vector3[], bool> callBack)
    {
        this.startNode = startNode;
        this.targetNode = targetNode;
        this.callBack = callBack;
    }

}
