using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Plot : MonoBehaviour
{
    public ProceduralScript generator;
    //public BuildingCustomisation customisation; 

    public Texture groundTexture;
    public Material material;

    public bool occupied = false;

    public GameObject plane;
    public GameObject planePrefab;

    public GameObject road;
    public List<GameObject> buildings = new List<GameObject>();
    public GameObject buildingPrefab;

    public Direction direction = Direction.NORTH;

    public Vector3 centre = Vector3.zero;
    public Vector3[] vertices = new Vector3[4];
    public float width;
    public float length; 

    public enum Direction
    { 
        SOUTH =  0,
        WEST = 1,
        NORTH = 2,
        EAST = 3
    }


    void Awake()
    {

    }
    private void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {

    }
    
    public void SetPlot(Vector3 pos, float w, float l)
    {
        centre = pos;
        width = w;
        length = l;
        /*plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane = new GameObject();
        plane.isStatic = true;
        plane.name = "Ground";
        plane.transform.position = centre;*/
        /*float scalar = width / 5;
        plane.transform.localScale = new Vector3(scalar, scalar, scalar);
        plane.transform.parent = transform;

        plane.transform.position = pos;*/
        vertices[0] = new Vector3(pos.x - width, pos.y, pos.z + length);
        vertices[1] = new Vector3(pos.x + width, pos.y, pos.z + length);
        vertices[2] = new Vector3(pos.x - width, pos.y, pos.z - length);
        vertices[3] = new Vector3(pos.x + width, pos.y, pos.z - length);
    }

    public void SetGroundMaterial(Material mat)
    {
        //groundTexture = txt;
        //plane.GetComponent<Material>().mainTexture = groundTexture;
        //plane.GetComponent<Renderer>().material = mat;
    }

    public void CreateBuilding(GameObject bP, Vector3 pos, Vector3 rot, bool useRot)
    {
        buildings.Add(Instantiate(bP, transform));
        buildings.Last().transform.parent = transform;
        buildings.Last().transform.localPosition = Vector3.zero;
        buildings.Last().name = "Building " + buildings.Count();
        if (useRot)
        {
            buildings.Last().transform.rotation = Quaternion.Euler(rot);
            buildings.Last().transform.position = pos;
        }
        else
        {
            buildings.Last().transform.LookAt(pos);
        }
        Debug.Log("building " + buildings.Last().name + " time " + Time.time);
        //sbuilding.GetComponent<BuildingCustomisation>().SetUp();
    }
}
