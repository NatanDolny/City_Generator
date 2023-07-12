using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCustomisation : MonoBehaviour
{
    public BuildingCustomisation customisation;

    public Material[] materials;
    public Material chosenMat;

    public GameObject roof;
    public GameObject[] body;
    public GameObject bodyCopy;
    public int height = 6;
    public int width = 8;
    public int length = 8;
    public int storeys;


    private void Awake()
    {
        chosenMat = materials[Random.Range(0, materials.Length)];
        bodyCopy = transform.Find("Body").gameObject;
        
        roof = transform.Find("Roof").gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.Find("Body").childCount -1; i ++ )
        {
            transform.Find("Body").transform.Find("Painted").GetChild(i).GetComponent<Renderer>().material = chosenMat;
        }
        SetUp();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUp()
    {
        storeys = Random.Range(1, 5);
        body = new GameObject[storeys];
        bool balconies = false;
        if (Random.value >= 0.25f && storeys > 1)
        {
            balconies = true; 
        }

        for (int i = 0; i < storeys; i++)
        {
            if (i > 0)
            {
                GameObject temp = Instantiate(bodyCopy, transform);
                temp.name = " body " + i;
                temp.transform.position = new Vector3(transform.position.x, transform.position.y + height * i, transform.position.z);
                body[i] = temp;

                if (balconies)
                {
                    temp.transform.Find("Balcony").gameObject.SetActive(true);
                }
            }
        }
        roof.transform.position = new Vector3(roof.transform.position.x, roof.transform.position.y + height * (storeys - 1), roof.transform.position.z);
    }
}
