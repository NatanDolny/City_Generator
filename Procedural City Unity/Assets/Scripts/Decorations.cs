using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decorations : MonoBehaviour
{
    public GameObject decoration; 
    public GameObject[] decorationList;

    public GameObject streetLamp;
    public GameObject bench;
    public GameObject bin;
    public GameObject tree;


    private void Awake()
    {
        decoration = gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        decoration = Instantiate(decorationList[(Random.Range(0, decorationList.Length))]);
        decoration.transform.parent = transform;
        decoration.transform.position = transform.position;
        decoration.transform.rotation = transform.rotation;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
