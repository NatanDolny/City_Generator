using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
    public BuildingGenerator buildingGenerator;

    public int buildingStory;
    public enum BuildingType 
    {
        FLAT,
        HOUSE,
        SHOP

    }
    public BuildingType type;

    private GameObject building;
    private GameObject smallWindow;
    private GameObject largeWindow;
    private GameObject door;



    private void Awake()
    {
        building = this.transform.Find("Building").gameObject;
        smallWindow = this.transform.Find("SmallWindow").gameObject;
        largeWindow = this.transform.Find("LargeWindow").gameObject;
        door = this.transform.Find("Door").gameObject;
    }
    // Start is called before the first frame update
    void Start()
    {
        //CreateBuilding(type);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void CreateBuilding(BuildingType buildingType)
    {
        switch (buildingType)
        {
            case BuildingType.FLAT:
                building.transform.localScale = new Vector3(7f, 7f, 7f);
                building.transform.localPosition = new Vector3(0, building.transform.localScale.y / 2, 0);
                smallWindow.transform.localPosition = new Vector3(building.transform.localScale.x / 2,
                    building.transform.localScale.y / 3, building.transform.localScale.z * 0.325f);
                Quaternion sWRot = new Quaternion(smallWindow.transform.rotation.x,
                    -smallWindow.transform.rotation.y,
                    smallWindow.transform.rotation.z,
                    smallWindow.transform.rotation.w);
                Vector3 sWPos = new Vector3(building.transform.localScale.x / 2,
                    building.transform.localScale.y / 3, 
                    -building.transform.localScale.z * 0.325f);
                GameObject smallWindow2 = Instantiate(smallWindow, sWPos, sWRot);
                //GameObject smallWindow2 = smallWindow;
                smallWindow2.transform.parent = this.gameObject.transform;
                smallWindow2.transform.localPosition = sWPos;
                smallWindow2.transform.localRotation = sWRot;
                largeWindow.transform.localPosition = new Vector3(-building.transform.localScale.x / 2,
                    building.transform.localScale.y / 3, 0f);

                smallWindow.transform.localScale = new Vector3(smallWindow.transform.localScale.x, 1.5f, 1.5f);
                smallWindow2.transform.localScale = new Vector3(smallWindow2.transform.localScale.x, 1.5f, 1.5f);
                largeWindow.transform.localScale = new Vector3(largeWindow.transform.localScale.x, 1.5f, 3f);
                door.transform.localPosition = new Vector3(building.transform.localScale.x / 2,
                    /*building.transform.parent.transform.position.y + */door.transform.localScale.y / 2, 0);
                
                //building.transform.localScale = new Vector3(8f, 8f, 8f);
                //building.transform.position = new Vector3(0, 4, 0);
                Debug.Log("Pointer --> FLAT --> ");
                break;
            case BuildingType.HOUSE:
                Debug.Log("Pointer --> HOUSE --> ");
                //building.transform.localScale = new Vector3(6f, 8f, 8f);
                //building.transform.position = new Vector3(0, 4, 0);
                break;
            case BuildingType.SHOP:
                Debug.Log("Pointer --> SHOP --> ");
                //building.transform.localScale = new Vector3(4f, 6f, 12f);
                //building.transform.position = new Vector3(0, 3, 0);
                break;
        }
    }
}
