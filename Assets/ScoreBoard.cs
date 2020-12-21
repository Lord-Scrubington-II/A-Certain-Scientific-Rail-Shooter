using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    private int score;
    private Text scoreText;
    
    [SerializeField] int pointsPerTick = 10; //points for living
    [SerializeField] float scoreTickLength = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        scoreText = gameObject.GetComponent<Text>();
        ScoreUpdate(0);
        StartCoroutine(ScoreTickStart());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator ScoreTickStart()
    {
        while (true)
        {
            //print("Score Updated");
            yield return new WaitForSeconds(scoreTickLength);
            ScoreUpdate(pointsPerTick);
        }
    }

    public void ScoreUpdate(int howMuch)
    {
        score += howMuch;
        scoreText.text = score.ToString();
    }
}
