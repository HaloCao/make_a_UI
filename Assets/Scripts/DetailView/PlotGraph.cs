﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlotGraph : MonoBehaviour
{

    [SerializeField] private Sprite circleSprite;

    public GameObject bar1;
    public GameObject bar2;
    public float yMaximum;
    

    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;

    string xLabel = "Hour";
    string yLabel = "Liter";

    private int xMaximum = 24;
    float value1;
    float value2;

    // Update is called once per frame
    float time_sum = 0;
    void Update()
    {
        value1 = bar1.GetComponent<ValueBar>().currentvalue;
        value2 = bar2.GetComponent<ValueBar>().currentvalue;
        float dt = Time.deltaTime;
        time_sum += dt;
        if (time_sum >= 0.5f)
        {
            AddDataAndShow(value1, value2);
            time_sum = 0;
        }
    }

    // Use this for initialization
    void Start()
    {
        graphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();

        labelTemplateX = graphContainer.Find("LabelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("LabelTemplateY").GetComponent<RectTransform>();
        dashTemplateX = graphContainer.Find("DashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = graphContainer.Find("DashTemplateY").GetComponent<RectTransform>();

        // create x grid and axis labels
        for (int i = 0; i < xMaximum; i++)
        {
            float xSize = graphContainer.sizeDelta.x / xMaximum;
            float xPos = i * xSize;
            if (i % 2 == 0)
            {
                RectTransform labelX = Instantiate(labelTemplateX);
                labelX.SetParent(graphContainer, false);
                labelX.anchoredPosition = new Vector2(xPos, -5);
                labelX.gameObject.SetActive(true);
                labelX.GetComponent<Text>().text = i.ToString();
            }

            RectTransform dashX = Instantiate(dashTemplateX);
            dashX.SetParent(graphContainer, false);
            dashX.anchoredPosition = new Vector2(xPos, 0);
            dashX.gameObject.SetActive(true);
        }
        RectTransform labelXEnd = Instantiate(labelTemplateX);
        labelXEnd.SetParent(graphContainer, false);
        labelXEnd.anchoredPosition = new Vector2(graphContainer.sizeDelta.x, -5);
        labelXEnd.gameObject.SetActive(true);
        labelXEnd.GetComponent<Text>().text = xLabel;

        // create y grid and axis labels
        int ySeparatorCount = 5;
        for (int i = 1; i < ySeparatorCount; i++)
        {
            float graphHeight = graphContainer.sizeDelta.y;
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer, false);
            float normalizedValue = i * 1.0f / ySeparatorCount;
            labelY.anchoredPosition = new Vector2(-7f, normalizedValue * graphHeight);
            labelY.gameObject.SetActive(true);
            labelY.GetComponent<Text>().text = Mathf.RoundToInt(normalizedValue * yMaximum).ToString();

            RectTransform dashY = Instantiate(dashTemplateY);
            dashY.SetParent(graphContainer, false);
            dashY.anchoredPosition = new Vector2(0, normalizedValue * graphHeight);
            dashY.gameObject.SetActive(true);
        }
        RectTransform labelYEnd = Instantiate(labelTemplateY);
        labelYEnd.SetParent(graphContainer, false);
        labelYEnd.anchoredPosition = new Vector2(-5f, graphContainer.sizeDelta.y);
        labelYEnd.gameObject.SetActive(true);
        labelYEnd.GetComponent<Text>().text = yLabel;
    }


    int dataXIdx = 0;
    Vector2 lastData1Pos = new Vector2(-1,-1);
    Vector2 lastData2Pos = new Vector2(-1, -1);

    private void AddDataAndShow(float value1, float value2)
    {
        if (dataXIdx == xMaximum) {
            for (int i = 0; i < xMaximum; i++) {
                GameObject temp;
                temp = GameObject.Find("circle_1_" + i.ToString()); if (temp) Destroy(temp);
                temp = GameObject.Find("circle_2_" + i.ToString()); if (temp) Destroy(temp);
                temp = GameObject.Find("dotConnection_1_" + i.ToString()); if (temp) Destroy(temp);
                temp = GameObject.Find("dotConnection_2_" + i.ToString()); if (temp) Destroy(temp);
            }

            dataXIdx = 0;
            lastData1Pos = new Vector2(-1, -1);
            lastData2Pos = new Vector2(-1, -1);
        }

        float graphHeight = graphContainer.sizeDelta.y;
        float graphWidth = graphContainer.sizeDelta.x;
        float xSize = graphWidth / xMaximum;
        float xPosition = dataXIdx * xSize;

        // channel 1
        float yPosition = value1 / yMaximum * graphHeight;
        Vector2 dataPos = new Vector2(xPosition, yPosition);
        
        if (lastData1Pos != new Vector2(-1,-1))
        {
            CreateDotConnetion(lastData1Pos, dataPos, dataXIdx,1);
        }
        CreateCircle(dataPos, dataXIdx, Color.white, 1);
        lastData1Pos = new Vector2(xPosition, yPosition);

        // channel 2
        yPosition = value2 / yMaximum * graphHeight;
        dataPos = new Vector2(xPosition, yPosition);
       
        if (lastData2Pos != new Vector2(-1, -1))
        {
            CreateDotConnetion(lastData2Pos, dataPos, dataXIdx, 2);
        }
        CreateCircle(dataPos, dataXIdx, Color.blue, 2);
        lastData2Pos = new Vector2(xPosition, yPosition);

        dataXIdx++;
    }

    private void CreateDotConnetion(Vector2 dotPositionA, Vector2 dotPositionB, int idx, int channel)
    {
        GameObject gameObject = new GameObject("dotConnection_"+channel.ToString()+"_"+idx.ToString(), typeof(Image));
        gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
        gameObject.transform.SetParent(graphContainer, false);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 1);
        rectTransform.anchoredPosition = dotPositionA + 0.5f * dir * distance;
        rectTransform.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
    }

    private GameObject CreateCircle(Vector2 anchoredPosition, int idx, Color color, int channel)
    {
        GameObject gameObject = new GameObject("circle_"+ channel.ToString()+"_"+ idx.ToString(), typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        gameObject.GetComponent<Image>().color = color;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(15, 15);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        return gameObject;
    }

    private float GetAngleFromVectorFloat(Vector2 vec)
    {
        float angle = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
        return angle;
    }
}
