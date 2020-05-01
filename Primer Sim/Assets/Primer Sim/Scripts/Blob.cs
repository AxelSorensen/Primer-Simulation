using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blob : MonoBehaviour
{
    //Declaring all the variables that will be used
    Rigidbody rb;
    public int speed = 10;
    public float senseRadius;
    public LayerMask foodLayer;
    public CreatureAction currentAction;
    public int eatenFood;
    public float energy = 100;
    public bool isHome = false;
    public float energyToGetBack;
    public bool goingHome = false;
    float blobHue = 0.575f; //Hue value for blue
    public bool startRot = false;

    public bool simStart;

    public GameObject[] walls;

    Vector3 closestPoint;
    GameObject nearestFood;
    Renderer blobRenderer;

    // Start is called before the first frame update
    void Start()
    {
        //senseRadius = (senseRadius / speed);
        blobRenderer = GetComponent<Renderer>();
        blobRenderer.material.color = Color.HSVToRGB((blobHue * (speed / 10f) / 3 + 0.40f), 1, 1);
        rb = GetComponent<Rigidbody>();
        // Starts the creature action on None
        currentAction = CreatureAction.None;
        // Decreases energy with 1 every 10th of a second
        InvokeRepeating("Energy", 0f, .1f);
    }

    void Update()
    {
        ClosestPoint();
        VisibleFood();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (simStart)
        {
            Act();
            EnergyCheck();
        }
    }

    // This function controls all the blob's actions with a switch statement.
    void Act()
    {
        switch (currentAction)
        {
            case CreatureAction.None:
                IsKinematic();
                break;
            case CreatureAction.Exploring:
                Explore();
                break;
            case CreatureAction.GoingToFood:
                MoveToFood();
                break;
            case CreatureAction.HitWall:
                HitWall();
                break;
            case CreatureAction.GoingHome:
                GoHome();
                break;
            case CreatureAction.IsHome:
                IsKinematic();
                break;
            case CreatureAction.OutOfEnergy:
                IsKinematic();
                break;

        }
    }

    // This function looks for visible food within the Blob's "sense radius"
    void VisibleFood()
    {
        // Creates an array with all colliders touching or inside the sphere. Only looks for objects on the "food" layer.
        Collider[] foodInRadius = Physics.OverlapSphere(transform.position, senseRadius, foodLayer);
        float nearestDist = Mathf.Infinity;
        nearestFood = null;

        // Loops through the foodInRadius array, finds the food object with the smallest distance to the blob and saves it to nearestFood
        foreach (var foodItem in foodInRadius)
        {
            if (Vector3.Distance(transform.position, foodItem.transform.position) < nearestDist)
            {
                nearestDist = Vector3.Distance(transform.position, foodItem.transform.position);
                nearestFood = foodItem.gameObject;
            }
        }
    }

    // This function controls the movement of the Blob
    void Explore()
    {
        if (rb.isKinematic)
        {
            rb.isKinematic = false;
        }

        // The blob moves foward (relative to its own rotation) at a constant speed
        rb.velocity = transform.forward * speed;
        // There is a 10% chance that the blob will change direction

        if (Random.value < .05f)
        {
            rb.transform.Rotate(new Vector3(0, Random.Range(-45, 45), 0));
        }
        // If there is food within senseRadius, change creature action
        if (nearestFood != null)
        {
            currentAction = CreatureAction.GoingToFood;
        }
    }

    // This function moves the blob to the position of the object stored in nearestFood
    void MoveToFood()
    {
        if (nearestFood != null)
        {
            // Sets the velocity equal to a direction vector pointing to nearestFood, multiplied by speed
            transform.LookAt(new Vector3(nearestFood.transform.position.x, transform.position.y, nearestFood.transform.position.z));
            rb.velocity = (nearestFood.transform.position - transform.position).normalized * speed;
        }
        else
        {
            currentAction = CreatureAction.Exploring;
        }
    }

    // This function turns the blob around 180 degrees and then going back to exploring
    void HitWall()
    {
        if (goingHome)
        {
            isHome = true;
            currentAction = CreatureAction.IsHome;
        }
        else
        {
            transform.Rotate(new Vector3(0, 180, 0));
            currentAction = CreatureAction.Exploring;
        }
    }

    // This function makes the blob take the shortest route home
    void GoHome()
    {
        goingHome = true;
        transform.LookAt(new Vector3(closestPoint.x, transform.position.y, closestPoint.z));
        rb.velocity = (closestPoint - transform.position).normalized * speed;
        Debug.DrawLine(closestPoint, transform.position, Color.red);
    }

    // This function calculates the closest point from the blob to "home" (the outer bounds)
    void ClosestPoint()
    {
        float nearestdist = Mathf.Infinity;
        GameObject nearestwall = null;

        // First the closest wall is found...
        foreach (var wall in walls)
        {
            if (Vector3.Distance(transform.position, wall.transform.position) < nearestdist)
            {
                nearestdist = Vector3.Distance(transform.position, wall.transform.position);
                nearestwall = wall.gameObject;
            }
        }
        // Than Collider.ClosestPoint is used to find the closest point from the blob to the wall
        closestPoint = nearestwall.GetComponent<Collider>().ClosestPoint(transform.position);
        energyToGetBack = ((Vector3.Distance(transform.position, closestPoint) * (speed / 10f) * (speed / 10f)) + 1);
    }


    // This function subtracts from energy and sets the blob's state to "Out of Energy" when energy runs out
    void Energy()
    {
        if (energy > 0)
        {
            energy -= (speed / 10f) * (speed / 10f);
        }
        else if (!isHome)
        {
            currentAction = CreatureAction.OutOfEnergy;
        }
    }

    // This function handles collisions with other objects
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("wall") && currentAction != CreatureAction.None)
        {
            currentAction = CreatureAction.HitWall;
        }

    }

    // This function sets all velocity to zero
    void IsKinematic()
    {
        rb.isKinematic = true;
    }

    // This if statement makes the blob go home if it already and just barely has enough energy to get back
    void EnergyCheck()
    {
        if (!goingHome && eatenFood > 0 && energy <= energyToGetBack)
        {
            currentAction = CreatureAction.GoingHome;
        }
    }
}
