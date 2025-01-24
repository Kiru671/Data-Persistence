using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickSaver : MonoBehaviour
{
    private List<Brick> brickList = new List<Brick>();
    [SerializeField] private MainManager manager;

    private void OnApplicationQuit()
    {
        brickList.AddRange(FindObjectsOfType<Brick>());
        
        // Save total number of bricks and line count
        DataManager.getInstance.SetInt(SceneID.sceneID, "TotalBricks", brickList.Count);
        DataManager.getInstance.SetInt(SceneID.sceneID, "LineCount", manager.LineCount);
        DataManager.getInstance.SetInt(SceneID.sceneID, "CurrentScore", manager.m_Points);
        
        foreach (var brick in brickList)
        {
            int index = brickList.IndexOf(brick);
            Debug.Log($"Saving brick {index}: Position = {brick.transform.position}, Points = {brick.PointValue}");
            DataManager.getInstance.SetVector3(SceneID.sceneID, $"BrickPosition{index}", brick.transform.position);
            DataManager.getInstance.SetInt(SceneID.sceneID, $"BrickPoints{index}", brick.PointValue);
        }

        DataManager.getInstance.SetFlag(SceneID.sceneID, "BrickSaved", !manager.m_GameOver);
    }
}