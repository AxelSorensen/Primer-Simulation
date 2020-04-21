using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blob : MonoBehaviour
{
    //Declaring all the variables that will be used
    Rigidbody rb;
    public int speed;
    public float senseRadius;
    public LayerMask foodLayer;
    public CreatureAction currentAction;
    public int eatenFood;
    public GameObject[] walls;
    public int energy = 100;
    public bool isHome = false;
    public int energyLoss;
    public float energyToGetBack;

    bool goingHome = false;
   
    Vector3 closestPoint;
    GameObject nearestFood;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Starts the creature action on None
        currentAction = CreatureAction.None;
        // Decreases energy with 1 every 10th of a second
        InvokeRepeating("Energy", 0f, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        ClosestPoint();
        VisibleFood();
        Act();
    }

    // This function controls all the blob's actions with a switch statement.
    void Act()
    {
        switch (currentAction)
        {
            case CreatureAction.None:
                rb.isKinematic = true;
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
                rb.isKinematic = true;
                break;
            case CreatureAction.OutOfEnergy:
                rb.isKinematic = true;
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
        // Sets kinematic to false
        if (rb.isKinematic)
        {
            rb.isKinematic = false;
        }
        // The blob moves foward (relative to its own rotation) at a constant speed
        rb.velocity = transform.forward * speed;

        // There is a 10% chance that the blob will change direction
        if (Random.value < .05f)
        {
            rb.transform.Rotate(new Vector3(0, Random.Range(-100, 100), 0));
        }
        // If there is food within senseRadius, change creature action
        if (nearestFood != null)
        {
            currentAction = CreatureAction.GoingToFood;
        }

        if (isHome)
        {
            currentAction = CreatureAction.IsHome;
        }

        // This if statement makes the blob go home if it already and just barely has enough energy to get back
        if (!goingHome && eatenFood > 0 && energy <= energyToGetBack)
        {
            goingHome = true;
            currentAction = CreatureAction.GoingHome;
        }
    }

    // This function moves the blob to the position of the object stored in nearestFood
    void MoveToFood()
    {
        if (nearestFood != null)
        {
            // Sets the velocity equal to a direction vector pointing to nearestFood, multiplied by speed
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
        rb.angularVelocity = Vector3.zero;
        transform.Rotate(new Vector3(0, 180, 0));
        currentAction = CreatureAction.Exploring;
    }

    // This function makes the blob take the shortest route home
    void GoHome()
    {
        if (energy > 0)
        {
            rb.velocity = (closestPoint - transform.position).normalized * speed;
            isHome = true;
            Debug.DrawLine(closestPoint, transform.position, Color.red);
        }
        else
        {
            currentAction = CreatureAction.OutOfEnergy;
        }

    }

    // This function calculates the closest point from the blob to "home" (the outer bounds)
    void ClosestPoint()
    {
        float nearestDist = Mathf.Infinity;
        GameObject nearestWall = null;

        // First the closest wall is found...
        foreach (var wall in walls)
        {
            if (Vector3.Distance(transform.position, wall.transform.position) < nearestDist)
            {
                nearestDist = Vector3.Distance(transform.position, wall.transform.position);
                nearestWall = wall.gameObject;
            }
        }
        // Then the closest point on that collider is found using .ClosestPoint(blobs position)
        closestPoint = nearestWall.GetComponent<Collider>().ClosestPoint(transform.position);
        // Then calculate the energy that would be needed to travel that distance
        energyToGetBack = (Vector3.Distance(transform.position, closestPoint) + 5);
    }

    // This function subtracts one from energy and sets the blob's state to "Out of Energy" if it doesn't make it home
    void Energy()
    {
        if (energy > 0)
        {
            energy -= energyLoss;
        }
        else if (!isHome)
        {
            currentAction = CreatureAction.OutOfEnergy;
        }
    }

    // This function handles collisions with other objects
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("wall"))
        {
            currentAction = CreatureAction.HitWall;
        }
        
    }
}
