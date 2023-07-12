using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    public Slider widthSlider;
    public Slider lengthSlider;
    public Slider roadSlider;
    public Slider smallBigSlider;
    public Slider fillerSlider;
    
    public int width;
    public int length;
    public int road;
    public float smallBig;
    public float filler;
    
    public TextMeshProUGUI widthText;
    public TextMeshProUGUI lengthText;
    public TextMeshProUGUI roadText;
    public TextMeshProUGUI smallBigText;
    public TextMeshProUGUI fillerText;

    private void Awake()
    {
        widthSlider = transform.Find("Configuration").Find("City Width").GetComponent<Slider>();
        lengthSlider = transform.Find("Configuration").Find("City Length").GetComponent<Slider>();
        roadSlider = transform.Find("Configuration").Find("Road Percentage").GetComponent<Slider>();
        smallBigSlider = transform.Find("Configuration").Find("Small To Big Ratio").GetComponent<Slider>();
        fillerSlider = transform.Find("Configuration").Find("Filler Building Ratio").GetComponent<Slider>();
 
        widthText = widthSlider.transform.Find("Value").GetComponent<TextMeshProUGUI>();
        lengthText = lengthSlider.transform.Find("Value").GetComponent<TextMeshProUGUI>();
        roadText = roadSlider.transform.Find("Value").GetComponent<TextMeshProUGUI>();
        smallBigText = smallBigSlider.transform.Find("Value").GetComponent<TextMeshProUGUI>();
        fillerText = fillerSlider.transform.Find("Value").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        CityConfig.ResetValues(); 

        widthSlider.onValueChanged.AddListener(delegate { Width(); });
        lengthSlider.onValueChanged.AddListener(delegate { Length(); });
        roadSlider.onValueChanged.AddListener(delegate { RoadPercentage(); });
        smallBigSlider.onValueChanged.AddListener(delegate { SmallToBig(); });
        fillerSlider.onValueChanged.AddListener(delegate { FillerRatio(); });

        widthText.text = "" + widthSlider.value;
        lengthText.text = "" + lengthSlider.value;
        roadText.text = "" + roadSlider.value + "%";
        smallBigText.text = "" + smallBigSlider.value;
        fillerText.text = "" + fillerSlider.value;
    }
    public void Generate()
    {
        // incase the values dont get changed. We have to set them before scene transition.
        width = (int)widthSlider.value;
        length = (int)lengthSlider.value;
        road = (int)roadSlider.value;
        smallBig = smallBigSlider.value;
        filler = fillerSlider.value;

        CityConfig.cityWidth = width;
        CityConfig.cityLength = length;
        CityConfig.roadPercentage = road;
        CityConfig.smallToBigRatio = smallBig;
        CityConfig.extraFiller = filler;

        Time.timeScale = 1;

        SceneManager.LoadScene("Generator");
    }
    public void Controls()
    {
        SceneManager.LoadScene("Controls");
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void Width()
    {
        width = (int)widthSlider.value;
        widthText.text = "" + width;
    }
    public void Length()
    {
        length = (int)lengthSlider.value;
        lengthText.text = "" + length;
    }
    public void RoadPercentage()
    {
        road = (int)roadSlider.value;
        roadText.text = "" + road + "%";
    }
    public void SmallToBig()
    {
        smallBig = smallBigSlider.value;
        smallBigText.text = "" + smallBig;
    }
    public void FillerRatio()
    {
        filler = fillerSlider.value;
        fillerText.text = "" + filler;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
