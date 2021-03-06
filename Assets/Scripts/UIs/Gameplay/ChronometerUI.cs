﻿using UnityEngine;
using UnityEngine.UI;

public class ChronometerUI : MonoBehaviour
{
    public float m_MaxTime = 300;
    public float m_ElapsedTime;
    public string m_Mask = "000";
    private Text m_Text;
    public bool m_PlayOnAwake = false;
    private bool m_Playing;

    public int RemainingTime => (int)Mathf.Clamp(Mathf.Ceil(m_MaxTime - m_ElapsedTime), 0, m_MaxTime);

    private void OnEnable()
    {
        m_Playing = m_PlayOnAwake;
        m_Text = GetComponent<Text>();
        UpdateUI();
    }

    public void SetMaxTime(float maxTime)
    {
        m_MaxTime = maxTime;
        m_ElapsedTime = 0.0f;
    }

    public void Stop()
    {
        m_Playing = false;
    }

    public void Play()
    {
        m_Playing = true;
        m_ElapsedTime = 0.0f;
        UpdateUI();
    }

    private void Update()
    {
        if (!m_Playing)
            return;

        m_ElapsedTime += Time.deltaTime;
        UpdateUI();
    }

    private void UpdateUI()
    {
        m_Text.text = RemainingTime.ToString(m_Mask);
    }
}