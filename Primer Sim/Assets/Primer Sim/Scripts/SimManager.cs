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

    [Header("Prefabs")]
    //Blue blobs have a speed of 10
    public GameObject blueblobPrefab;
    //Blue blobs have a speed of 20
    public GameObject redblobPrefab;
    public GameObject ground;
    public GameObject foodPrefab;

    public TextMeshPro count;
    public TextMeshPro simNum;

    int simNumber = 0;
    float bounds;

    List<int> counts = new List<int>();
    // This list is used to keep track of blobs being added and removed from the simulation
    List<GameObject> blobs = new List<GameObject>();
    Vector3 spawnPos;

    void Awake()
    {
        bounds = (ground.transform.lossyScale.x * 10) / 2 - blobBoundDecreaser;
        // For some reason I have to manually reference the 4 walls in script, otherwise the blobs get lost
        blueblobPrefab.GetComponent<Blob>().walls = GameObject.FindGameObjectsWithTag("wall");
        redblobPrefab.GetComponent<Blob>().walls = GameObject.FindGameObjectsWithTag("wall");
    }

    // Start is called before the first frame update
    void Start()
    {
        // I use a coroutine so I can execute functions with pauses inbetween
        StartCoroutine("Setup");
        Time.timeScale = timeScale;
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
            spawnPos.y = 1;
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
        GameObject redblob = (GameObject)Instantiate(redblobPrefab, new Vector3(-bounds,1,4), Quaternion.identity);
        blobs.Add(redblob);

        // Now the setup is done! This while loop will cause the following code to be repeated.
        while (true)
        {
            // This code displays the current amount of blobs and the sim number.
            count.text = "Blobs: " + blobs.Count.ToString();
            simNumber++;
            simNum.text = "Sim: " + simNumber;

            // This code instantiated the food on the plane
            for (int i = 0; i < foodAmount; i++)
            {
                spawnPos = Random.insideUnitSphere * (bounds - foodBoundDecreaser);
                spawnPos.y = .25f;
                GameObject food = (GameObject)Instantiate(foodPrefab, spawnPos, Quaternion.identity);
            }

            yield return new WaitForSeconds(2);

            // When everything is ready, all the blobs are reset and their current action is set to Exploring
            foreach (GameObject blob in blobs)
            {
                blob.GetComponent<Blob>().eatenFood = 0;
                blob.transform.LookAt(new Vector3(0, transform.position.y, 0));
                blob.GetComponent<Blob>().isHome = false;
                blob.GetComponent<Blob>().currentAction = CreatureAction.Exploring;
                blob.GetComponent<Blob>().energy = 100;
            }
            // It takes 10 seconds for the blobs energy to go from 100 to 0. So we wait for 10 seconds before proceeding.
            yield return new WaitForSeconds(10);
            yield return new WaitForSeconds(2);
            GameObject[] foods = GameObject.FindGameObjectsWithTag("food");

            // Destroy the leftover food
            foreach (GameObject food in foods)
            {
                Destroy(food);
            }

            // Remove the blobs that are out of energy
            foreach (GameObject blob in blobs.ToArray())
            {
                if (blob.GetComponent<Blob>().currentAction == CreatureAction.OutOfEnergy)
                {
                    blobs.Remove(blob);
                    Destroy(blob.gameObject);
                }
            }

            yield return new WaitForSeconds(2);
           
            // Instantiating a new blob for every blob with 2 eaten foods. The if statement is repetitive but decides whether to reproduce a blue or a red blob base on the blob that is reproducing.
            foreach (GameObject blob in blobs.ToArray())
            {
                if (blob.GetComponent<Blob>().eatenFood > 1)
                {
                    if (blob.name == "Blob(Clone)")
                    {
                        GameObject newblob = (GameObject)Instantiate(blueblobPrefab, blob.transform.position, Quaternion.identity);
                        newblob.GetComponent<Blob>().isHome = true;
                        blobs.Add(newblob);
                    }
                    else
                    {
                        GameObject newblob = (GameObject)Instantiate(redblobPrefab, blob.transform.position, Quaternion.identity);
                        newblob.GetComponent<Blob>().isHome = true;
                        blobs.Add(newblob);
                    }
                    
                }
            }

        }

    }
}
