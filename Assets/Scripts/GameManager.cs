﻿using Cinemachine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public Pause m_PauseUI;
    public HintTextUI m_HintUI;
    public ChronometerUI m_ChronometerUI;
    public CounterUI m_MovesUI;
    public LevelUI m_LevelUI;
    public ScoreUI m_ScoreUI;

    [Header("Map")]
    public MapGenerator m_MapGenerator;
    private GameObject m_Player;
    private GameObject m_Portal;
    private List<IEnemy> m_EnemyScripts = new List<IEnemy>();
    private GameObject[] m_Enemies;

    [Header("Camera")]
    public CinemachineTargetGroup m_TargetGroup;
    public CinemachineVirtualCamera m_VirtualCamera;
    public float m_MinCamera = 4.0f;
    public float m_MaxCamera = 6.0f;
    
    [Header("Camera Player")]
    [Range(0.0f, 10.0f)]
    public float m_PlayerWeight = 2.0f;
    [Range(0.0f, 10.0f)]
    public float m_PlayerRadius = 1.0f;

    [Header("Camera Portal")]
    public bool m_UseCenterWithPortal = true;
    [Range(0.0f, 10.0f)]
    public float m_PortalWeight = 2.0f;
    [Range(0.0f, 10.0f)]
    public float m_PortalRadius = 2.0f;

    [Header("Next Level")]
    public float m_LoadTime = 2.0f;
    private Map m_Map;

    private void Start()
    {
        m_Map = m_MapGenerator.Build();
        
        Initialize();
        SetTargetGroup();
        DisablePlayer();
        FindAllEnemies();

        ShowStory(m_Map.Level, m_Map.Story);
    }

    private void FindAllEnemies()
    {
        var enemies = Helper.FindAll<IEnemy>();
        foreach (var enemy in enemies)
            m_EnemyScripts.Add(enemy);

        m_Enemies = GameObject.FindGameObjectsWithTag("Enemy");

        HideEnemies();
    }

    private void SetTargetGroup()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_Portal = GameObject.FindGameObjectWithTag("Portal");
        m_TargetGroup.AddMember(m_Player.transform, m_PlayerWeight, m_PlayerRadius);
        m_TargetGroup.AddMember(m_Portal.transform, m_PortalWeight, m_PortalRadius);

        m_VirtualCamera.GetCinemachineComponent<CinemachineGroupComposer>().m_MinimumOrthoSize = m_MinCamera;
        m_VirtualCamera.GetCinemachineComponent<CinemachineGroupComposer>().m_MaximumOrthoSize = m_MaxCamera;
    }

    private void Initialize()
    {
        m_ChronometerUI.SetMaxTime(m_Map.Time);
        m_MovesUI.SetValue(m_Map.Moves);
        m_LevelUI.SetValue(m_Map.Level);

        m_ChronometerUI.transform.parent.gameObject.SetActive(false);
        m_MovesUI.transform.parent.gameObject.SetActive(false);
        m_LevelUI.transform.parent.gameObject.SetActive(false);
        m_ScoreUI.transform.parent.gameObject.SetActive(false);
    }

    private void EnablePlayer()
    {
        m_Player.GetComponent<PlayerController>().enabled = true;
    }

    private void DisablePlayer()
    {
        m_PauseUI.enabled = false;
        m_Player.GetComponent<PlayerController>().enabled = false;
    }

    public void ShowStory(int level, string message)
    {
        m_HintUI.Show($"Level {level}", message);
    }

    public void DisableCameraPortal()
    {
        if (!m_UseCenterWithPortal)
        {
            m_TargetGroup.RemoveMember(m_Portal.transform);
        }
    }

    public void HideStory()
    {
        m_HintUI.Hide();

        m_ChronometerUI.transform.parent.gameObject.SetActive(true);
        m_MovesUI.transform.parent.gameObject.SetActive(true);
        m_LevelUI.transform.parent.gameObject.SetActive(true);
        m_ScoreUI.transform.parent.gameObject.SetActive(true);
        m_PauseUI.enabled = true;

        EnablePlayer();
        DisableCameraPortal();
    }

    public void GameOver()
    {
        DisablePlayer();
        ScreenManager.Instance.LoadLevel("Gameover");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            GameOver();

        if (Input.GetKeyDown(KeyCode.V))
            Victory();
    }

    public void Victory()
    {
        DisablePlayer();
        m_HintUI.Show("Congratulations!", $"You completed level {m_Map.Level}");
        m_HintUI.AddListener(delegate{ OnClickVictory(); });
    }

    private void OnClickVictory()
    {
        PlayerPrefs.SetInt("level", m_Map.Level + 1);
        ScreenManager.Instance.LoadLevelLoading("Gameplay");
    }

    public bool HasMovement =>  m_MovesUI.m_Value > 0;

    public void UseMovement()
    {
        m_MovesUI.Decrease(1);
    }

    public void Loop()
    {
        foreach (var enemy in m_EnemyScripts)
            enemy.Move();
    }

    public void ShowEnemies()
    {
        foreach (var enemy in m_Enemies)
            enemy.SetActive(true);
    }

    public void HideEnemies()
    {
        foreach (var enemy in m_Enemies)
            enemy.SetActive(false);
    }
}