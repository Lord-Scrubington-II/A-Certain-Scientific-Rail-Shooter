using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private string thisName;

    [SerializeField] private GameObject deathFX;
    [SerializeField] private Transform parentOfFXObj;
    [SerializeField] private readonly int pointsPerKill = 50;
    [SerializeField] private readonly int pointsPerHit = 5;
    [SerializeField] private int hp = 5;


    private ScoreBoard scoreBoard;

    // Start is called before the first frame update
    void Start()
    {
        thisName = gameObject.name;
        AddNonTriggerMeshCollider();
        scoreBoard = GameObject.FindObjectOfType<ScoreBoard>();
    }

    /**
     * func: AddNonTriggerMeshCollider() 
     * Guarantee attachment of a mesh collider at runtime.
     */
    void AddNonTriggerMeshCollider()
    {
        Collider boxCollider = gameObject.AddComponent<MeshCollider>();
        boxCollider.isTrigger = false;
    }

    void OnParticleCollision(GameObject other)
    {
        print("Particles Collided with " + thisName);

        scoreBoard.ScoreUpdate(pointsPerHit);

        hp--;
        if(hp <= 0){
            Kill();
        }
    }

    /**
     * func: Kill()
     * Kills the enemy by instantiating death fx and destroying itself in memory.
     */
    private void Kill()
    {
        //render explosion effects
        GameObject fx = Instantiate(deathFX, transform.position, Quaternion.identity);
        fx.transform.parent = parentOfFXObj;

        //make the scoreboard update points
        scoreBoard.ScoreUpdate(this.pointsPerKill);

        //bye bye
        Destroy(gameObject);
    }
}
