using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakSolid : MonoBehaviour
{
    private int breakCount = 0; //counter to keep track of hits
    [SerializeField] private Material stoneB; //new material
    [SerializeField] private Material stoneC; //new material

    void OnCollisionEnter (Collision other) {
        if (other.gameObject.CompareTag("Projectile")){ //if collision is with a projectile
            breakCount++; //increase break count
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (breakCount == 1){
          GetComponent<Renderer>().material = stoneB;  //change material once
        }else if(breakCount == 2){
          GetComponent<Renderer>().material = stoneC; //change material once more
        }else if(breakCount == 3){
            Destroy(gameObject); //destory the object
        }
    }
}
