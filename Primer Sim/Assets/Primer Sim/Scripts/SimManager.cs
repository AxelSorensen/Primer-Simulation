using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SimManager : MonoBehaviour
{
    [Header("Sim Settings")]
    public int blobAmount;
    public int foodAmount;
    public int timeScale;
    // These variables control how far out on the plane, the blobs and foods are spawned
    public float foodBoundDecreaser;
    public float blobBoundDecreaser;

    public TextMeshPro oneEatenHomeText;
    public TextMeshPro twoEatenHomeText;
    public TextMeshPro zeroEatenText;
    public TextMeshPro oneEatenNotHomeText;
    public TextMeshPro twoEatenNotHomeText;
    public TextMeshPro endBlobs;
    public TextMeshPro blobsAdded;

    [Header("Prefabs")]
    //Blue blobs have a speed of 1
    public GameObject blueblobPrefab;
    public GameObject ground;
    public GameObject foodPrefab;

    public TextMeshPro count;
    public TextMeshPro simNum;

    int oneEatenHome;
    int twoEatenHome;
    int zeroEaten;
    int oneEatenNotHome;
    int twoEatenNotHome;

    float bounds;

    int simNumber = 0;

    public GameObject graph;

    public List<int> blueCount = new List<int>();
    public List<int> redCount = new List<int>();
    // This list is used to keep track of blobs being added and removed from the simulation
    List<GameObject> blobs = new List<GameObject>();
    List<GameObject> redBlobs = new List<GameObject>();
    List<int> histogram = new List<int>();
    Vector3 spawnPos;
    float circleRadius = 29;

    float fixedTime;

    void Awake()
    {

        // For some reason I have to manually reference the 4 walls in script, otherwise the blobs get lost
        blueblobPrefab.GetComponent<Blob>().walls = GameObject.FindGameObjectsWithTag("wall");
        //redblobPrefab.GetComponent<Blob>().ringWall = GameObject.FindGameObjectWithTag("wallsphere");
    }

    // Start is called before the first frame update
    void Start()
    {
        bounds = (ground.transform.lossyScale.x * 10) / 2 - blobBoundDecreaser;
        //this.fixedTime = Time.fixedDeltaTime;
        Time.timeScale = timeScale;
        //Time.fixedDeltaTime = this.fixedTime * Time.timeScale;
        // I use a coroutine so I can execute functions with pauses inbetween
        StartCoroutine("Setup");
    }

    void Update()
    {
        // Not used for now, everything is happening in the setup function
    }

    IEnumerator Setup()
    {
        // This whole for loop just determines a random vector position in the outer bounds of the plane
        for (int i = 0; i < blobAmount; i++)
        {
            spawnPos.y = 0;
            int randomNumber = Random.Range(1, 5);
            switch (randomNumber)
            {
                case 1:
                    spawnPos.x = bounds;
                    spawnPos.z = Random.Range(-bounds, bounds);
                    break;
                case 2:
                    spawnPos.x = -bounds;
                    spawnPos.z = Random.Range(-bounds, bounds);
                    break;
                case 3:
                    spawnPos.x = Random.Range(-bounds, bounds);
                    spawnPos.z = bounds;
                    break;
                case 4:
                    spawnPos.x = Random.Range(-bounds, bounds);
                    spawnPos.z = -bounds;
                    break;
            }
            // Instantiating a blob at the randomized position and adds it to the blob list

            GameObject blob = (GameObject)Instantiate(blueblobPrefab, spawnPos, Quaternion.identity);
            blobs.Add(blob);

        }
        // This code instantiates the red blob in a predefined position and adds it too blobs aswell
        //GameObject redblob = (GameObject)Instantiate(redblobPrefab, SpawnPos(), Quaternion.identity);
        //blobs.Add(redblob);
        //redBlobs.Add(redblob);

        // Now the setup is done! This while loop will cause the following code to be repeated.

        while (true)
        {
            endBlobs.text = "End blobs: " + "...";
            oneEatenHomeText.text = "Blobs home with 1 foods: " + "...";
            twoEatenHomeText.text = "Blobs home with 2 foods: " + "...";
            zeroEatenText.text = "Blobs with 0 foods: " + "...";
            oneEatenNotHomeText.text = "Blobs out with 1 foods: " + "...";
            twoEatenNotHomeText.text = "Blobs out with 2 foods: " + "...";
            blobsAdded.text = "Blobs added: " + "...";

            for (int i = 0; i <= 100; i++)
            {
                histogram.Add(0);
            }

            foreach (GameObject blob in blobs)
            {
                histogram[blob.GetComponent<Blob>().speed] += 1;
            }


            //if (!(blobs.Count - redBlobs.Count <= 0))
            //{
            //    blueCount.Add(blobs.Count - redBlobs.Count);
            //}
            //else
            //{
            //    blueCount.Add(0);
            //}
            //redCount.Add(redBlobs.Count);
            graph.GetComponent<WindowGraph>().ShowGraph(histogram);
            histogram.Clear();
            // This code displays the current amount of blobs and the sim number.
            count.text = "Start blobs: " + blobs.Count.ToString();
            simNumber++;
            simNum.text = "Sim: " + simNumber;

            // This code instantiated the food on the plane
            for (int i = 0; i < foodAmount; i++)
            {
                spawnPos = new Vector3(Random.Range(-bounds, bounds), .25f, Random.Range(-bounds, bounds));
                GameObject food = (GameObject)Instantiate(foodPrefab, spawnPos, Quaternion.identity);
            }
            //if(foodAmount > 10)
            //{
            //    foodAmount--;
            //}

            yield return new WaitForSeconds(2f);
            // When everything is ready, all the blobs are reset and their current action is set to Exploring
            foreach (GameObject blob in blobs)
            {
                blob.GetComponent<Blob>().simStart = true;
                blob.GetComponent<Blob>().eatenFood = 0;
                blob.transform.LookAt(new Vector3(0, transform.position.y, 0));
                blob.GetComponent<Blob>().isHome = false;
                blob.GetComponent<Blob>().goingHome = false;
                blob.GetComponent<Blob>().startRot = false;
                blob.GetComponent<Blob>().currentAction = CreatureAction.Exploring;
                blob.GetComponent<Blob>().energy = 100;

            }
            // It takes 10 seconds for the blobs energy to go from 100 to 0. So we wait for 10 seconds before proceeding.
            float slowestSpeed = Mathf.Infinity;
            foreach (GameObject blob in blobs)
            {
                if (blob.GetComponent<Blob>().speed / 10f < slowestSpeed)
                {
                    slowestSpeed = blob.GetComponent<Blob>().speed / 10f;
                }
            }
            Debug.Log(10f / (slowestSpeed * slowestSpeed));
            yield return new WaitForSeconds(10f / (slowestSpeed * slowestSpeed) + 1f);
            GameObject[] foods = GameObject.FindGameObjectsWithTag("food");

            // Destroy the leftover food
            foreach (GameObject food in foods)
            {
                Destroy(food);
            }

            oneEatenHome = 0;
            twoEatenHome = 0;
            zeroEaten = 0;
            oneEatenNotHome = 0;
            twoEatenNotHome = 0;


            foreach (GameObject blob in blobs)
            {
                if (blob.GetComponent<Blob>().eatenFood == 1 && blob.GetComponent<Blob>().currentAction == CreatureAction.IsHome)
                {
                    oneEatenHome += 1;
                }
                if (blob.GetComponent<Blob>().eatenFood == 2 && blob.GetComponent<Blob>().currentAction == CreatureAction.IsHome)
                {
                    twoEatenHome += 1;
                }
                if (blob.GetComponent<Blob>().eatenFood == 0 && blob.GetComponent<Blob>().currentAction != CreatureAction.IsHome)
                {
                    zeroEaten += 1;
                }
                if (blob.GetComponent<Blob>().eatenFood == 1 && blob.GetComponent<Blob>().isHome != true)
                {
                    oneEatenNotHome += 1;
                }
                if (blob.GetComponent<Blob>().eatenFood == 2 && blob.GetComponent<Blob>().isHome != true)
                {
                    twoEatenNotHome += 1;
                }
            }

            oneEatenHomeText.text = "Blobs home with 1 foods: " + oneEatenHome.ToString();
            twoEatenHomeText.text = "Blobs home with 2 foods: " + twoEatenHome.ToString();
            zeroEatenText.text = "Blobs with 0 foods: " + zeroEaten.ToString();
            oneEatenNotHomeText.text = "Blobs out with 1 foods: " + oneEatenNotHome.ToString();
            twoEatenNotHomeText.text = "Blobs out with 2 foods: " + twoEatenNotHome.ToString();
            blobsAdded.text = "Blobs added: " + (twoEatenHome + oneEatenHome + twoEatenNotHome + oneEatenNotHome + zeroEaten);
            yield return new WaitForSeconds(.1f);

            // Remove the blobs that are out of energy
            foreach (GameObject blob in blobs.ToArray())
            {
                if (!blob.GetComponent<Blob>().isHome)
                {
                    blobs.Remove(blob);
                    Destroy(blob.gameObject);
                }
            }

            yield return new WaitForSeconds(.1f);

            // Instantiating a new blob for every blob with 2 eaten foods.
            foreach (GameObject blob in blobs.ToArray())
            {
                if (blob.GetComponent<Blob>().eatenFood > 1 && blob.GetComponent<Blob>().currentAction == CreatureAction.IsHome)
                {
                    GameObject newblob = (GameObject)Instantiate(blueblobPrefab, blob.transform.position, Quaternion.identity);
                    newblob.GetComponent<Blob>().isHome = true;
                    newblob.GetComponent<Blob>().goingHome = true;
                    blobs.Add(newblob);

                    newblob.GetComponent<Blob>().speed = blob.GetComponent<Blob>().speed;
                    if (Random.value < 1f)
                    {
                        if (Random.value > 0.5f)
                        {
                            newblob.GetComponent<Blob>().speed -= 1;
                        }
                        else
                        {
                            newblob.GetComponent<Blob>().speed += 1;
                        }
                    }
                }
            }

            endBlobs.text = "End blobs: " + blobs.Count.ToString();
            yield return new WaitForSeconds(.1f);
        }

    }

    Vector3 SpawnPos()
    {
        float angle = Random.value * Mathf.PI * 2;

        spawnPos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * (circleRadius - blobBoundDecreaser);
        return spawnPos;
    }
}
