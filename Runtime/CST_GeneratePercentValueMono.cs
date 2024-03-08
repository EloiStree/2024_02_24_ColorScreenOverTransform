using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CST_GeneratePercentValueMono : MonoBehaviour
{
    public UnityEvent<float> m_onPercentGenerated;
    public float m_percentSpeed;
    public float m_currentPercentValue;


    public void Update()
    {
        m_currentPercentValue += m_percentSpeed * Time.deltaTime;
        m_currentPercentValue = Mathf.Clamp(m_currentPercentValue, -2f, 2f);
        if (m_currentPercentValue > 1f)
            m_currentPercentValue -= 1f;
        if (m_currentPercentValue <0f )
            m_currentPercentValue += 1f;
        m_onPercentGenerated.Invoke(m_currentPercentValue);
    }
}
