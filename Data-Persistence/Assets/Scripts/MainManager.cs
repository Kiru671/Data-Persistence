using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public GameObject GameOverText;

    public int brickCount;
    
    private bool m_Started = false;
    public int m_Points;
    
    public bool m_GameOver = false;
    public static event Action OnGameOver;
    public static event Action OnHIScoreChanged;

    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        bool gameSaved = DataManager.getInstance.GetFlag(SceneID.sceneID, "BrickSaved");
        Debug.Log($"Game saved status: {gameSaved}");
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};

        if (gameSaved)
        {
            //Load saved score
            m_Points = DataManager.getInstance.GetInt(SceneID.sceneID, "CurrentScore");
            ScoreText.text = $"Score : {m_Points}";
            
            Debug.Log("Loading saved bricks");
            // Load saved line count
            LineCount = DataManager.getInstance.GetInt(SceneID.sceneID, "LineCount");
            Debug.Log($"Loaded LineCount: {LineCount}");
            
            // Load saved bricks
            int totalBricks = DataManager.getInstance.GetInt(SceneID.sceneID, "TotalBricks");
            Debug.Log($"Loading {totalBricks} bricks");
            
            for (int x = 0; x < totalBricks; ++x)
            {
                Vector3 savedPos = DataManager.getInstance.GetVector3(SceneID.sceneID, $"BrickPosition{x}");
                int savedPoints = DataManager.getInstance.GetInt(SceneID.sceneID, $"BrickPoints{x}");
                
                if (savedPos != Vector3.zero)
                {
                    var savedBrick = Instantiate(BrickPrefab, savedPos, Quaternion.identity);
                    brickCount++;
                    savedBrick.PointValue = savedPoints;
                    savedBrick.onDestroyed.AddListener(AddPoint);
                }
            }
        }
        else
        {
            Debug.Log("Creating new brick layout");
            // Create new brick layout
            for (int i = 0; i < LineCount; ++i)
            {
                for (int x = 0; x < perLine; ++x)
                {
                    Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                    var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                    brickCount++;
                    brick.PointValue = pointCountArray[i % pointCountArray.Length];
                    brick.onDestroyed.AddListener(AddPoint);
                }
            }
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();
                

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        if(!m_GameOver && brickCount == 0)
        {
            GameOver();
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
        if(m_Points > DataManager.getInstance.GetInt("0", "HiScore"))
        {
            DataManager.getInstance.SetInt("0", "HiScore", m_Points);
            OnHIScoreChanged?.Invoke();
        }
        brickCount--;
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        OnGameOver?.Invoke();
        DataManager.getInstance.SetFlag(SceneID.sceneID, "BrickSaved", false);
    }
}