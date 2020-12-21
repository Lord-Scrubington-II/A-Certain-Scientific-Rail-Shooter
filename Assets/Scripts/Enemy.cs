using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private string thisName;
    [SerializeField] GameObject deathFX;
    [SerializeField] Transform parent;
    [SerializeField] int pointsPerHit = 50;

    private ScoreBoard scoreBoard;

    // Start is called before the first frame update
    void Start()
    {
        thisName = gameObject.name;
        AddNonTriggerBoxCollider();
        scoreBoard = GameObject.FindObjectOfType<ScoreBoard>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AddNonTriggerBoxCollider()
    {
        Collider boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = false;
    }

    void OnParticleCollision(GameObject other)
    {
        print("Particles Collided with " + thisName);

        //render explosion effects
        GameObject fx = Instantiate(deathFX, transform.position, Quaternion.identity);
        fx.transform.parent = parent;

        //make the scoreboard update points
        scoreBoard.ScoreUpdate(this.pointsPerHit);

        //bye bye
        Destroy(gameObject);
    }
}
