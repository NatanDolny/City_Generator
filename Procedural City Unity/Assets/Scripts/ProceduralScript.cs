//using System; 
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Linq;


[RequireComponent(typeof(MeshFilter))]

public class ProceduralScript : MonoBehaviour
{
    private Plot plotScript;
    private GameObject City;
    private GameObject cityFloor;

    public Texture grassTexture;
    public Texture groundTexture;
    public Material grassMaterial;
    public Material groundMaterial;
    public Material pavementMaterial;

    public GameObject Building1x1;
    public GameObject Building2x1;

    public GameObject buildingPrefab;
    public GameObject roadPrefab;
    BuildingGenerator buildingGeneratorScript;

    public GameObject parentObject;
    private List<Vector3> roadPoints;

    public float width = 10f;
    public float length = 10f;



    public List<GameObject> landPlots = new List<GameObject>();

    private Mesh mapMesh;
    private Vector3[] vertices;
    private int[] triangles;

    private Mesh[] mesh;
    private Vector3[] tiles;

    List<GameObject> minutes;


    public List<Road> roads = new List<Road>();
    public GameObject[,] roadObjects;

    public GameObject land;
    public float timer = 0;
    public bool timerPaused = false;

    [Range(0, 1)] public float percentageOfRoad;

    public int citySize = 100;
    public int cityWidth = 10;
    public int cityLength = 10;

    public float smallToBigRatio = 0.35f;
    public float coroutineResetTime = 0.01f;
    public float extraFillerRatio = 0.3f;

    private void Awake()
    {
        Configuration();
        roadObjects = new GameObject[cityLength - 1, cityWidth - 1];
        citySize = cityWidth * cityLength;
        plotScript = GetComponent<Plot>();
    }


    public void Configuration()
    {
        cityWidth = CityConfig.cityWidth;
        cityLength = CityConfig.cityLength;
        smallToBigRatio = CityConfig.smallToBigRatio;
        extraFillerRatio = 1 - CityConfig.extraFiller;
        percentageOfRoad = CityConfig.roadPercentage / 100;
    }

    void Start()
    {
        //Configuration();

        land = transform.Find("Land").gameObject;
        land.GetComponent<Renderer>().material = grassMaterial;
        land.GetComponent<Renderer>().material.mainTextureScale = new Vector2(10 * cityWidth, 10 * cityLength);
        land.transform.localScale = new Vector3(cityWidth * width /5, 1, cityLength * length / 5);

        mapMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mapMesh;


        parentObject = this.gameObject;
        tiles = new Vector3[citySize];
        mesh = new Mesh[citySize];
        landPlots = new List<GameObject>();
        roadPoints = new List<Vector3>();

        CreateMap();
    }

    private void Update()
    {
        if (!timerPaused)
        {
            timer += Time.deltaTime;
            if (timer > 3)
            {
                timer = 0;
                timerPaused = true;
                StartCoroutine(CreateBuildings());
            }
        }
    }

    public void CreateMap()
    {
        Vector3 startingPoint = new Vector3(
            parentObject.transform.position.x - width * (cityWidth - 1),
            parentObject.transform.position.y,
            parentObject.transform.position.z - length * (cityLength - 1));

        vertices = new Vector3[(cityLength + 1) * (cityWidth + 1)];

        for (int y = 0, i = 0; y < cityLength; y++)
        {
            for (int x = 0; x < cityWidth; x++)
            {
                GameObject temp = new GameObject();
                temp.transform.parent = transform.Find("LandPlots");
                temp.name = "Plot " + i;
                Plot plot = temp.AddComponent<Plot>();
                plot.planePrefab = plotScript.planePrefab;
                temp.transform.localPosition = new Vector3(startingPoint.x + x * width * 2, startingPoint.y, startingPoint.z + y * length * 2);
                plot.SetPlot(temp.transform.localPosition, width, length);
                plot.generator = this.GetComponent<ProceduralScript>();
                plot.SetGroundMaterial(grassMaterial);

                landPlots.Add(temp);
                if (x < cityWidth - 1 && y < cityLength - 1)
                {
                    roadPoints.Add(plot.vertices[1]);
                    GameObject tempRoad = Instantiate(roadPrefab, transform.Find("Roads"));
                    tempRoad.transform.position = plot.vertices[1];
                    roadObjects[y, x] = tempRoad;
                    roadObjects[y, x].GetComponent<Road>().identifier = new Vector2(x, y);

                    if (x == cityWidth - 1 || x == 0)
                    {
                        tempRoad.GetComponent<Road>().canExpandX = false;
                    }
                    if (y == cityLength - 1 || y == 0)
                    {
                        tempRoad.GetComponent<Road>().canExpandY = false;
                    }
                    //roads.Add()
                }

                if (x == cityWidth)
                {
                    startingPoint = new Vector3(startingPoint.x, startingPoint.y, startingPoint.z + length);
                }
                i++;
            }
        }
        MapRoads();
    }

    public void UpdateMesh()
    {
        mapMesh.Clear();
        mapMesh.vertices = vertices;
        mapMesh.triangles = triangles;
        mapMesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        if (vertices == null)
        {
            return;
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }


    public void MapRoads()
    {
        int startX = 0;
        int startY = 0;
        int maxX = cityWidth - 1;
        int maxY = cityLength - 1;
        int currentX = startX;
        int currentY = startY; 
        

        roadObjects[startY, startX].GetComponent<Road>().identifier = new Vector2(startY, startX);
        roadObjects[startY, startX].GetComponent<Road>().roadType = Road.RoadType.CROSSROAD;

        for (int _y = 0; _y < maxY; _y ++)
        {
            for (int _x = 0; _x < maxX; _x++)
            {
                GameObject tempObject = roadObjects[_y, _x];
                Road tempRoadScript = roadObjects[_y, _x].GetComponent<Road>();
                if (_y > 0 && _y < maxY - 1 && _x > 0 && _x < maxX - 1)
                {
                    tempRoadScript.roadType = Road.RoadType.CROSSROAD;
                    tempRoadScript.connections.Add(Plot.Direction.SOUTH);
                    tempRoadScript.connections.Add(Plot.Direction.EAST);
                    tempRoadScript.connections.Add(Plot.Direction.WEST);
                    tempRoadScript.connections.Add(Plot.Direction.NORTH);
                }

                if (tempRoadScript.identifier.x == 0)
                {
                    tempRoadScript.roadType = Road.RoadType.INTERSECTION;
                    tempRoadScript.connections.Add(Plot.Direction.SOUTH);
                    tempRoadScript.connections.Add(Plot.Direction.EAST);
                    tempRoadScript.connections.Add(Plot.Direction.NORTH);
                }
                else if (tempRoadScript.identifier.x == maxX - 1)
                {
                    tempRoadScript.roadType = Road.RoadType.INTERSECTION;
                    tempObject.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                    tempRoadScript.connections.Add(Plot.Direction.SOUTH);
                    tempRoadScript.connections.Add(Plot.Direction.WEST);
                    tempRoadScript.connections.Add(Plot.Direction.NORTH);
                }
                else if (tempRoadScript.identifier.y == 0)
                {
                    tempRoadScript.roadType = Road.RoadType.INTERSECTION;
                    tempObject.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
                    tempRoadScript.connections.Add(Plot.Direction.NORTH);
                    tempRoadScript.connections.Add(Plot.Direction.EAST);
                    tempRoadScript.connections.Add(Plot.Direction.WEST);
                }
                else if (tempRoadScript.identifier.y == maxY - 1)
                {
                    tempRoadScript.roadType = Road.RoadType.INTERSECTION;
                    tempObject.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                    tempRoadScript.connections.Add(Plot.Direction.WEST);
                    tempRoadScript.connections.Add(Plot.Direction.EAST);
                    tempRoadScript.connections.Add(Plot.Direction.SOUTH);
                }
            }
        }
        roadObjects[0, 0].GetComponent<Road>().roadType = Road.RoadType.CORNER;
        roadObjects[0, 0].transform.rotation = Quaternion.Euler(0,0,0);
        roadObjects[0, 0].GetComponent<Road>().connections.Clear();
        roadObjects[0, 0].GetComponent<Road>().connections.Add(Plot.Direction.EAST);
        roadObjects[0, 0].GetComponent<Road>().connections.Add(Plot.Direction.NORTH);


        roadObjects[0, maxX - 1].GetComponent<Road>().roadType = Road.RoadType.CORNER;
        roadObjects[0, maxX - 1].transform.rotation = Quaternion.Euler(0, -90, 0);
        roadObjects[0, maxX - 1].GetComponent<Road>().connections.Clear();
        roadObjects[0, maxX - 1].GetComponent<Road>().connections.Add(Plot.Direction.WEST);
        roadObjects[0, maxX - 1].GetComponent<Road>().connections.Add(Plot.Direction.NORTH);


        roadObjects[maxY - 1, 0].GetComponent<Road>().roadType = Road.RoadType.CORNER;
        roadObjects[maxY - 1, 0].transform.rotation = Quaternion.Euler(0, 90, 0);
        roadObjects[maxY - 1, 0].GetComponent<Road>().connections.Clear();
        roadObjects[maxY - 1, 0].GetComponent<Road>().connections.Add(Plot.Direction.EAST);
        roadObjects[maxY - 1, 0].GetComponent<Road>().connections.Add(Plot.Direction.SOUTH);


        roadObjects[maxY - 1, maxX - 1].GetComponent<Road>().roadType = Road.RoadType.CORNER;
        roadObjects[maxY - 1, maxX - 1].transform.rotation = Quaternion.Euler(0, 180, 0);
        roadObjects[maxY - 1, maxX - 1].GetComponent<Road>().connections.Clear();
        roadObjects[maxY - 1, maxX - 1].GetComponent<Road>().connections.Add(Plot.Direction.WEST);
        roadObjects[maxY - 1, maxX - 1].GetComponent<Road>().connections.Add(Plot.Direction.SOUTH);


        foreach (GameObject gO in roadObjects)
        {
            Road tempScript = gO.GetComponent<Road>();
            tempScript.SetRoad();
        }

        int toDeleteCount = (int)(roadObjects.Length * (1 - percentageOfRoad));
        int consecutiveFails = 0;


        int y = 0;
        int x = 0;

        int yLength = maxY;
        int xLength = maxX;


        List<Identifiers> identifiers = new List<Identifiers>();

        for (int _y = 0; _y < yLength; _y ++)
        {
            for (int _x = 0; _x < xLength; _x++)
            {
                identifiers.Add(new Identifiers());
                
                identifiers[identifiers.Count - 1].yID = _y;
                identifiers[identifiers.Count - 1].xID = _x;
            }
        }

        Shuffle(identifiers);
        int calc = 0;
        int dif = Mathf.Abs(maxY - maxX);
        int greaterVector = maxX < maxY ? maxX : maxY;
        int maxDirectionCounter = (int)Mathf.Sqrt(maxX * maxY) + (maxX < maxY? maxX : maxY) / 2 -  Mathf.Abs(dif / greaterVector);
        int directionCounter = maxDirectionCounter;
        bool goVertical = greaterVector == maxY ? true : false;
        while (toDeleteCount > 0 && identifiers.Count > 0 )
        {
            y = identifiers[0].yID;
            x = identifiers[0].xID;
            identifiers.RemoveAt(0);

            bool resetted = false;

            if (roadObjects[y, x].GetComponent<Road>().roadType != Road.RoadType.VOID)
            {
                if (x > 0 && (roadObjects[y, x - 1].GetComponent<Road>().connections.Count > 2 ||
                    (roadObjects[y, x - 1].GetComponent<Road>().roadType == Road.RoadType.VOID &&
                    !goVertical)))
                {
                    if (x + 1 < cityWidth - 1 && (roadObjects[y, x + 1].GetComponent<Road>().connections.Count > 2 ||
                        (roadObjects[y, x + 1].GetComponent<Road>().roadType == Road.RoadType.VOID &&
                        !goVertical)))
                    {
                        if (y > 0 && (roadObjects[y - 1, x].GetComponent<Road>().connections.Count > 2 ||
                            (roadObjects[y - 1, x].GetComponent<Road>().roadType == Road.RoadType.VOID &&
                            goVertical)))
                        {
                            if (y + 1 < cityLength - 1 && (roadObjects[y + 1, x].GetComponent<Road>().connections.Count > 2 ||
                                (roadObjects[y + 1, x].GetComponent<Road>().roadType == Road.RoadType.VOID &&
                                goVertical)))
                            {
                                roadObjects[y, x - 1].GetComponent<Road>().connections.Remove(Plot.Direction.EAST);
                                roadObjects[y - 1, x].GetComponent<Road>().connections.Remove(Plot.Direction.NORTH);
                                roadObjects[y, x + 1].GetComponent<Road>().connections.Remove(Plot.Direction.WEST);
                                roadObjects[y + 1, x].GetComponent<Road>().connections.Remove(Plot.Direction.SOUTH);

                                resetted = true;
                                roadObjects[y, x].GetComponent<Road>().ResetRoad();
                            }
                        }
                    }
                }
                if (!resetted)
                {
                    if (x > 0 && x < maxX - 1)
                    {
                        if (roadObjects[y, x - 1].GetComponent<Road>().connections.Count > 2 || 
                            (roadObjects[y, x - 1].GetComponent<Road>().roadType == Road.RoadType.VOID &&
                            !goVertical))
                        {
                            if (roadObjects[y, x + 1].GetComponent<Road>().connections.Count > 2 ||
                                (roadObjects[y, x + 1].GetComponent<Road>().roadType == Road.RoadType.VOID &&
                                !goVertical))
                            {
                                if (y == 0)
                                {
                                    if (roadObjects[y + 1, x].GetComponent<Road>().connections.Count > 2 ||
                                        (roadObjects[y + 1, x].GetComponent<Road>().roadType == Road.RoadType.VOID &&
                                        goVertical))
                                    {
                                        roadObjects[y + 1, x].GetComponent<Road>().connections.Remove(Plot.Direction.SOUTH);
                                        roadObjects[y, x].GetComponent<Road>().ResetRoad();
                                        resetted = true;
                                    }
                                }
                                else if (y == maxY - 1)
                                {
                                    if (roadObjects[y - 1, x].GetComponent<Road>().connections.Count > 2 ||
                                        (roadObjects[y - 1, x].GetComponent<Road>().roadType == Road.RoadType.VOID &&
                                        goVertical))
                                    {
                                        roadObjects[y - 1, x].GetComponent<Road>().connections.Remove(Plot.Direction.NORTH);
                                        roadObjects[y, x].GetComponent<Road>().ResetRoad();
                                        resetted = true;
                                    }
                                }
                                if (resetted)
                                {
                                    roadObjects[y, x - 1].GetComponent<Road>().connections.Remove(Plot.Direction.EAST);
                                    roadObjects[y, x + 1].GetComponent<Road>().connections.Remove(Plot.Direction.WEST);
                                }
                            }
                        }
                    }
                }
                if (!resetted)
                {
                    if (y > 0 && y < maxY - 1)
                    {
                        if (roadObjects[y - 1, x].GetComponent<Road>().connections.Count > 2 ||
                            (roadObjects[y - 1, x].GetComponent<Road>().roadType == Road.RoadType.VOID &&
                            goVertical))
                        {
                            if (roadObjects[y + 1, x].GetComponent<Road>().connections.Count > 2 ||
                                (roadObjects[y + 1, x].GetComponent<Road>().roadType == Road.RoadType.VOID &&
                                goVertical))
                            {
                                if (x == 0)
                                {
                                    if (roadObjects[y, x + 1].GetComponent<Road>().connections.Count > 2 ||
                                        (roadObjects[y, x + 1].GetComponent<Road>().roadType == Road.RoadType.VOID &&
                                        !goVertical))
                                    {
                                        roadObjects[y, x + 1].GetComponent<Road>().connections.Remove(Plot.Direction.WEST);
                                        roadObjects[y, x].GetComponent<Road>().ResetRoad();
                                        resetted = true;
                                    }
                                }
                                else if (x == maxX - 1)
                                {
                                    if (roadObjects[y, x - 1].GetComponent<Road>().connections.Count > 2 ||
                                        (roadObjects[y, x - 1].GetComponent<Road>().roadType == Road.RoadType.VOID &&
                                        !goVertical))
                                    {
                                        roadObjects[y, x - 1].GetComponent<Road>().connections.Remove(Plot.Direction.EAST);
                                        roadObjects[y, x].GetComponent<Road>().ResetRoad();
                                        resetted = true;
                                    }
                                }

                                if (resetted)
                                {
                                    roadObjects[y - 1, x].GetComponent<Road>().connections.Remove(Plot.Direction.NORTH);
                                    roadObjects[y + 1, x].GetComponent<Road>().connections.Remove(Plot.Direction.SOUTH);
                                }
                            }
                        }
                    }
                }
            }
            
            if (resetted)
            {
                calc++;
                toDeleteCount--;
                directionCounter--;
                if (directionCounter < 0)
                {
                    directionCounter = maxDirectionCounter;
                    goVertical = !goVertical;
                }
            }
        }

        for (int _y = 0; _y < maxY; _y++)
        {
            for (int _x = 0; _x < maxX; _x++)
            {
                Road tempScript = roadObjects[_y, _x].GetComponent<Road>();
                if (tempScript.connections.Count == 3)
                {
                    Destroy(roadObjects[_y, _x].transform.Find("Road").gameObject);
                    tempScript.roadType = Road.RoadType.INTERSECTION;
                    tempScript.SetRoad();

                    int total = 0;
                    for (int i = 0; i < tempScript.connections.Count; i++)
                    {
                        total += (int)tempScript.connections[i];
                    }

                    if (total == 4)
                    {
                        roadObjects[_y, _x].transform.rotation = Quaternion.Euler(0, 90, 0);
                    }
                    else if (total == 3)
                    {
                        roadObjects[_y, _x].transform.rotation = Quaternion.Euler(0, 180, 0);
                    }
                    else if (total == 6)
                    {
                        roadObjects[_y, _x].transform.rotation = Quaternion.Euler(0, -90, 0);
                    }
                    else if (total == 5)
                    {
                        roadObjects[_y, _x].transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                }
                else if (tempScript.connections.Count == 2)
                {
                    int total = 0;
                    for (int i = 0; i < tempScript.connections.Count; i++)
                    {
                        total += (int)tempScript.connections[i];
                    }

                    Destroy(roadObjects[_y, _x].transform.Find("Road").gameObject);

                    if (total == 2 || total == 4)
                    {
                        tempScript.roadType = Road.RoadType.STRAIGHT;
                    }
                    else
                    {
                        tempScript.roadType = Road.RoadType.CORNER;
                    }

                    tempScript.SetRoad();

                    if (total == 1)
                    {
                        roadObjects[_y, _x].transform.rotation = Quaternion.Euler(0, 180, 0);
                    }
                    else if (total == 5)
                    {
                        roadObjects[_y, _x].transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    else if (total == 3)
                    {
                        if (tempScript.connections.Contains(Plot.Direction.NORTH))
                        {
                            roadObjects[_y, _x].transform.rotation = Quaternion.Euler(0, -90, 0);
                        }
                        else
                        {
                            roadObjects[_y, _x].transform.rotation = Quaternion.Euler(0, 90, 0);
                        }
                    }
                    else if (total == 2)
                    {
                        roadObjects[_y, _x].transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    else if (total == 4)
                    {
                        roadObjects[_y, _x].transform.rotation = Quaternion.Euler(0, 90, 0);
                    }
                }
            }
        }
    }

    public IEnumerator CreateBuildings()
    {
        for (int i = 0; i < landPlots.Count; i++)
        {
            if (!landPlots[i].GetComponent<Plot>().occupied)
            {
                Vector3 original = landPlots[i].transform.position;
                Vector3 temp = original;
                Vector3 lookAt = landPlots[i].transform.position;
                List<Vector3> lookAts = new List<Vector3>();
                Vector3 rot = Vector3.zero;
                Ray ray;
                RaycastHit hit;

                List<GameObject> objects = new List<GameObject>();

                bool foundRoad = false;

                ray = new Ray(new Vector3(temp.x - width, temp.y + 100, temp.z), Vector3.down * 10);

                if (Physics.Raycast(ray, out hit, 1000))
                {
                    if (hit.collider.CompareTag("Road"))
                    {
                        lookAt = hit.point;
                        lookAts.Add(hit.point);
                        foundRoad = true; 
                    }
                    else if (hit.collider.CompareTag("Land"))
                    {
                        if (Random.value >= extraFillerRatio)
                        {
                            landPlots[i].GetComponent<Plot>().CreateBuilding(Building1x1, hit.point, rot, true);
                        }
                    }
                }
                ray = new Ray(new Vector3(temp.x + width, temp.y + 100, temp.z), Vector3.down);
                if (Physics.Raycast(ray, out hit, 1000))
                {
                    if (hit.collider.CompareTag("Road"))
                    {
                        lookAt = hit.point;
                        lookAts.Add(hit.point);
                        foundRoad = true;
                    }
                    else if (hit.collider.CompareTag("Land"))
                    {
                        if (Random.value >= extraFillerRatio)
                        {
                            landPlots[i].GetComponent<Plot>().CreateBuilding(Building1x1, hit.point, rot, true);
                        }
                    }
                }
                ray = new Ray(new Vector3(temp.x, temp.y + 100, temp.z - length), Vector3.down);
                if (Physics.Raycast(ray, out hit, 1000))
                {
                    if (hit.collider.CompareTag("Road"))
                    {
                        lookAt = hit.point;
                        lookAts.Add(hit.point);
                        foundRoad = true;
                    }
                    else if (hit.collider.CompareTag("Land"))
                    {
                        if (Random.value >= extraFillerRatio)
                        {
                            landPlots[i].GetComponent<Plot>().CreateBuilding(Building1x1, hit.point, rot, true);
                        }
                    }
                }
                ray = new Ray(new Vector3(temp.x, temp.y + 100, temp.z + length), Vector3.down);
                if (Physics.Raycast(ray, out hit, 1000))
                {
                    if (hit.collider.CompareTag("Road"))
                    {
                        lookAt = hit.point;
                        lookAts.Add(hit.point);
                        foundRoad = true;
                    }
                    else if (hit.collider.CompareTag("Land"))
                    {
                        if (Random.value >= extraFillerRatio)
                        {
                            landPlots[i].GetComponent<Plot>().CreateBuilding(Building1x1, hit.point, rot, true);
                        }
                    }
                }

                if (foundRoad)
                {
                    lookAt = lookAts[Random.Range(0, lookAts.Count() - 1)];
                    if (Random.value >= smallToBigRatio)
                    {
                        landPlots[i].GetComponent<Plot>().CreateBuilding(Building1x1, lookAt, rot, false);
                    }
                    else
                    {
                        landPlots[i].GetComponent<Plot>().CreateBuilding(Building2x1, lookAt, rot, false);
                    }
                }

                for (int c = 0; c < landPlots[i].transform.childCount; c++)
                {
                    bool stay = false;
                    List<int> rotations = new List<int>();
                    for (int r = 0; r < 4; r++)
                    {
                        landPlots[i].transform.GetChild(c).transform.rotation = Quaternion.Euler(new Vector3(0, 90 * r, 0));
                        Vector3 origin = new Vector3(
                            landPlots[i].transform.GetChild(c).transform.position.x,
                            landPlots[i].transform.GetChild(c).transform.position.y + 50,
                            landPlots[i].transform.GetChild(c).transform.position.z);
                        origin = origin + landPlots[i].transform.GetChild(c).transform.forward * width;

                        Ray _ray = new Ray(origin, Vector3.down);
                        RaycastHit _hit;
                        if (Physics.Raycast(_ray, out _hit, 60))
                        {
                            if (_hit.collider.CompareTag("Road"))
                            {
                                rotations.Add(r);
                                stay = true;
                            }
                        }
                    }
                    if (!stay)
                    {
                        landPlots[i].GetComponent<Plot>().buildings.Remove(landPlots[i].transform.GetChild(c).gameObject);
                        Destroy(landPlots[i].transform.GetChild(c).gameObject);
                    }
                    else
                    {
                        landPlots[i].transform.GetChild(c).transform.rotation = Quaternion.Euler(new Vector3(0, 90 * rotations[Random.Range(0, rotations.Count())], 0));
                    }
                }
            }
            if (i == landPlots.Count -1)
            {
                Debug.Log(Time.realtimeSinceStartup);
            }
            yield return new WaitForSeconds(coroutineResetTime);
        }
    }

    public class RoadCheck
    {
        public bool isRoad = false;
        public int index = 0;
    }

    public void CreateMinutes()
    {
        Vector3 startPos = landPlots[0].transform.position; 
        startPos = new Vector3(startPos.x + width, startPos.y, startPos.z);
        GameObject minutesParent = new GameObject();
        minutesParent.name = "Minutes";

        minutesParent.transform.parent = transform;
        minutes = new List<GameObject>();
        List<RoadCheck> roadChecks = new List<RoadCheck>();

        for (int y = 0; y < cityLength - 1; y ++)
        {
            for (int x = 0; x < cityWidth - 1; x++)
            {
                Vector3 pos = new Vector3(startPos.x + width * 2 * x + 0.05f, startPos.y, startPos.z + length * 2 * y);
                Ray ray = new Ray(new Vector3(pos.x, pos.y + 5, pos.z), Vector3.down * 10);
                RaycastHit hit;

                minutes.Add(new GameObject());

                minutes[y * (cityWidth - 1) + x].name = "Minute";

                if (!Physics.Raycast(ray, out hit, 10))
                {
                    GameObject temp;
                    if (Random.value >= smallToBigRatio)
                    {
                        temp = Instantiate(Building1x1, minutes[y * (cityWidth - 1) + x].transform);
                    }
                    else
                    {
                        temp = Instantiate(Building2x1, minutes[y * (cityWidth - 1) + x].transform);
                    }
                    temp.transform.localPosition = minutes[y * (cityWidth - 1) + x].transform.position;
                }

                minutes[y * (cityWidth - 1) + x].transform.parent = minutesParent.transform;
                minutes[y * (cityWidth - 1) + x].transform.position = pos;
            }
        }
    }

    public class Identifiers
    {
        public int xID = 0;
        public int yID = 0;
    }
    public void Shuffle(List<Identifiers> list)
    {
        var count = list.Count;
        var lastID = count - 1;
        for (var i = 0; i < lastID; ++i)
        {
            var rand = Random.Range(i, count);
            var temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }
}
