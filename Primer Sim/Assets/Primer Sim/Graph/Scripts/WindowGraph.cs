using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class WindowGraph : MonoBehaviour
{
    [SerializeField] private RectTransform circle;
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;
    private List<GameObject> gameObjectList;

    public GameObject fill;
    public List<Vector3[]> dotList = new List<Vector3[]>();

    [Header("Graph Settings")]
    public bool dynamicXAxis;

    private void Awake()
    {
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();
        dashTemplateX = graphContainer.Find("dashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = graphContainer.Find("dashTemplateY").GetComponent<RectTransform>();
        circle = graphContainer.Find("circle").GetComponent<RectTransform>();
        List<int> values = new List<int> {5, 40, 20};
        List<int> values2 = new List<int> { 10, 20, 5 };
        gameObjectList = new List<GameObject>();
        ShowGraph(values, values2);
    }

    public void ShowGraph(List<int> valueList, List<int> valueList2, Func<int, string> getAxisLabelX = null, Func<float, string> getAxisLabelY = null)
    {
        if (getAxisLabelX == null)
        {
            getAxisLabelX = delegate (int _i) { return _i.ToString(); };
        }
        if (getAxisLabelY == null)
        {
            getAxisLabelY = delegate (float _f) { return Mathf.RoundToInt(_f).ToString(); };
        }
        foreach(GameObject gameObject in gameObjectList)
        {
            Destroy(gameObject);
        }
        gameObjectList.Clear();

        fill.GetComponent<MeshFill>().DestroyChildren();

        float graphHeight = graphContainer.sizeDelta.y;
        float graphWidth = graphContainer.sizeDelta.x;
        float yMaximum = 100f;
        float xSize = 60f;
        if (dynamicXAxis)
        {
            xSize = graphWidth / valueList.Count;
        }
        GameObject lastCircleGameObject = null;

        for (int i = 0; i < valueList.Count; i++)
        {
            float xPosition = xSize + i * xSize;
            float yPosition = (valueList[i]/ yMaximum) * graphHeight;

            RectTransform circleGameObject = Instantiate(circle);
            circleGameObject.sizeDelta = new Vector2(0,0);
            circleGameObject.SetParent(graphContainer, false);
            circleGameObject.gameObject.SetActive(true);
            circleGameObject.anchoredPosition = new Vector2(xPosition, yPosition);

            gameObjectList.Add(circleGameObject.gameObject);
            if (lastCircleGameObject != null)
            {
                //GameObject dotConnectionGameObject = CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
                //gameObjectList.Add(dotConnectionGameObject);

                fill.GetComponent<MeshFill>().CreateShape(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition, 0, 0);
            }
            lastCircleGameObject = circleGameObject.gameObject;
        }

        lastCircleGameObject = null;

        for (int i = 0; i < valueList.Count; i++)
        {
            float xPosition = xSize + i * xSize;
            float yPosition = ((valueList[i] + valueList2[i]) / yMaximum) * graphHeight;

            RectTransform circleGameObject = Instantiate(circle);
            circleGameObject.sizeDelta = new Vector2(0, 0);
            circleGameObject.SetParent(graphContainer, false);
            circleGameObject.gameObject.SetActive(true);
            circleGameObject.anchoredPosition = new Vector2(xPosition, yPosition);

            gameObjectList.Add(circleGameObject.gameObject);
            if (lastCircleGameObject != null)
            {
                //GameObject dotConnectionGameObject = CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
                //gameObjectList.Add(dotConnectionGameObject);

                fill.GetComponent<MeshFill>().CreateShape(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition, 1, .1f);
            }
            lastCircleGameObject = circleGameObject.gameObject;

            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(graphContainer, false);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(xPosition, -5f);
            labelX.GetComponent<Text>().text = getAxisLabelX(i);
            gameObjectList.Add(labelX.gameObject);

            RectTransform dashX = Instantiate(dashTemplateX);
            dashX.SetParent(graphContainer, false);
            dashX.gameObject.SetActive(true);
            dashX.anchoredPosition = new Vector2(xPosition, -5f);
            gameObjectList.Add(dashX.gameObject);

        }

        int separatorCount = 10;
        for (int i = 0; i <= separatorCount; i++)
        {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer, false);
            labelY.gameObject.SetActive(true);
            float normalizedValue = i * 1f / separatorCount;
            labelY.anchoredPosition = new Vector2(-5f, normalizedValue * graphHeight);
            labelY.GetComponent<Text>().text = getAxisLabelY(normalizedValue * yMaximum);
            gameObjectList.Add(labelY.gameObject);

            RectTransform dashY = Instantiate(dashTemplateY);
            dashY.SetParent(graphContainer, false);
            dashY.gameObject.SetActive(true);
            dashY.anchoredPosition = new Vector2(-5f, normalizedValue * graphHeight);
            gameObjectList.Add(dashY.gameObject);
        }
    }

    private GameObject CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 1f);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.sizeDelta = new Vector2(distance, 2f);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

        return gameObject;

        
    }
}
