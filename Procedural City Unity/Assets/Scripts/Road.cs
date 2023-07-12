using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    public List<Plot.Direction> connections;

    public GameObject straight;
    public GameObject corner;
    public GameObject intersection;
    public GameObject crossRoads;

    public Vector2 identifier;

    public bool isRoad = false;

    public bool canExpandX = true;
    public bool canExpandY = true;

    public RoadType roadType = RoadType.STRAIGHT;
    public enum RoadType
    {
        VOID,
        STRAIGHT,
        CORNER,
        INTERSECTION,
        CROSSROAD
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetConnections(int total)
    {
        if (total == 1)
        {
            Plot.Direction connection = (Plot.Direction)Random.Range(0, 4);
            connections.Add(connection);
            if (connection == Plot.Direction.NORTH)
            {
                roadType = RoadType.STRAIGHT;
            }
            else if(connection == Plot.Direction.EAST)
            {
                roadType = RoadType.CORNER;
                transform.Rotate(new Vector3(0, 90, 0));
            }
            else if (connection == Plot.Direction.WEST)
            {
                roadType = RoadType.CORNER;
                transform.Rotate(new Vector3(0, -90, 0));
            }
        }
        if (total == 2)
        {
            Plot.Direction connection1 = (Plot.Direction)Random.Range(0, 4);
            Plot.Direction connection2 = (Plot.Direction)Random.Range(1, 4);

            while (connection2 == connection1)
            {
                connection2 = (Plot.Direction)Random.Range(1, 4);
            }

            connections.Add(connection1);
            connections.Add(connection2);
            roadType = RoadType.INTERSECTION;
        }
        if (total == 3)
        {
            connections.Add(Plot.Direction.WEST);
            connections.Add(Plot.Direction.NORTH);
            connections.Add(Plot.Direction.EAST);
            roadType = RoadType.CROSSROAD;
        }

        /*for (int i = 0; i < total; i++)
        {

            if (i)
            connections.Add(i);
        }*/
    }

    public void SetRoad()
    {
        GameObject temp;
        isRoad = true;
        if (roadType == RoadType.STRAIGHT)
        {
            temp = Instantiate(straight, transform);
            temp.transform.position = transform.position;
            temp.name = "Road";
        }
        else if (roadType == RoadType.CORNER)
        {
            temp = Instantiate(corner, transform);
            temp.transform.position = transform.position;
            temp.name = "Road";
        }
        else if (roadType == RoadType.INTERSECTION)
        {
            temp = Instantiate(intersection, transform);
            temp.transform.position = transform.position;
            temp.name = "Road";
        }
        else if (roadType == RoadType.CROSSROAD)
        {
            temp = Instantiate(crossRoads, transform);
            temp.transform.position = transform.position;
            temp.name = "Road";
        }
    }

    public void ResetRoad()
    {
        isRoad = false;
        roadType = RoadType.VOID;
        connections.Clear();
        Destroy(transform.Find("Road").gameObject);
    }
}
