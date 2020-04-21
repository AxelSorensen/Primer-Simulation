using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    bool wasEaten = false;

    // I had to detect the collision from the food gameobject, since multiple blobs were colliding with 1 food at once (and all got a point).
    // I am controlling the blobs current action and foodEaten from this script.
    // And using !wasEaten to only let a food collide with something once
    void OnCollisionEnter(Collision other)
    {
        if (!wasEaten)
        {
            if (other.gameObject.CompareTag("blob") && other.gameObject.GetComponent<Blob>().eatenFood < 2)
            {
                other.gameObject.GetComponent<Blob>().eatenFood += 1;
                wasEaten = true;
                Destroy(gameObject);
                // Keep exploring if only 1 food is eaten, else go home
                if (other.gameObject.GetComponent<Blob>().eatenFood < 2)
                {
                    other.gameObject.GetComponent<Blob>().currentAction = CreatureAction.Exploring;
                }
                else
                {
                    other.gameObject.GetComponent<Blob>().currentAction = CreatureAction.GoingHome;
                }
            }
        }
    }
}
