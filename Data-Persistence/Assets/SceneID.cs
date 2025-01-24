using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneID : MonoBehaviour
{
    public static string sceneID = "";
    // Start is called before the first frame update
    void Awake()
    {
        sceneID = SceneManager.GetActiveScene().name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
