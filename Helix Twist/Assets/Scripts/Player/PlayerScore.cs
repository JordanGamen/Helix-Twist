﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScore : MonoBehaviour
{
    [SerializeField] private int score = 0;
    private int highScore = 0;
    [SerializeField] private int comboCounter = 0;
    public int ComboCounter { get { return comboCounter; } }
    public int Score { get { return score; } }

    private PlayerSwipe playerSwipe;
    private PlayerSoundEffects playerSound;
    private HUD ui;    

    private float lastYposWhenEarningPoint;

    private List<GameObject> points = new List<GameObject>();
    public List<GameObject> PointParts { get { return points; } }

    private void Start()
    {
        highScore = PlayerPrefs.GetInt("highScore", 0);
        lastYposWhenEarningPoint = transform.position.y;
        playerSwipe = GetComponent<PlayerSwipe>();
        playerSound = GetComponent<PlayerSoundEffects>();
        ui = FindObjectOfType<HUD>();        
        score = GameManager.instance.SavedScore;
        ui.UpdateScore(score);
    }

    public void AddPoints()
    {
        points.AddRange(GameObject.FindGameObjectsWithTag("Point"));
    }

    public void ResetCombo()
    {
        comboCounter = 0;        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent == null || other.gameObject.layer != 0)
        {
            return;
        }

        GameObject potentialPoint = other.transform.parent.parent.parent.gameObject;

        if (potentialPoint.gameObject.CompareTag("Point") && points.Contains(potentialPoint) && transform.position.y < other.transform.position.y)
        {
            if (comboCounter >= 1 && lastYposWhenEarningPoint - transform.position.y < 1)
            {
                //Debug.Log("Points too close! My Y: " + transform.position.y + " last point Y: " + lastYposWhenEarningPoint);
                return;
            }

            //Debug.Log("Points far enough! my Y: " + transform.position.y + " last point Y: " + lastYposWhenEarningPoint + " calc: " + (lastYposWhenEarningPoint - transform.position.y));
            lastYposWhenEarningPoint = transform.position.y;
            points.Remove(potentialPoint);
            comboCounter++;            
            score += comboCounter;            
            playerSound.PlayPointSound();

            if (score > highScore)
            {
                highScore = score;
                PlayerPrefs.SetInt("highScore", highScore);
            }

            ui.UpdateScore(score);
            ui.ShowCombo(comboCounter);            
            playerSwipe.ResetSwipe();
        }
    }
}
