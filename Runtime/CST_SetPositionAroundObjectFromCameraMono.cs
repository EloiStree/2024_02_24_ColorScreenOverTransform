using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CST_SetPositionAroundObjectFromCameraMono : MonoBehaviour
{
    public Transform m_whatToMove;
    public Transform m_aroundWhat;

    public float m_radiusDistance = 360;

    [Range(0,360)]
    public float m_currentAngle = 0;

    public void SetCurrentAngle(float angle)
    {
        m_currentAngle = angle;
        RefreshPosition();
    }
    public void SetCurrentAngleAsPercentClockwise(float percentClockwise)
    {
        m_currentAngle = percentClockwise*360f;
        RefreshPosition();
    }
    public void SetDistanceRadiusOfObject(float radius)
    {
        m_radiusDistance = radius;
        RefreshPosition();
    }

    void Update()
    {
        RefreshPosition();

    }

    private  void RefreshPosition()
    {
        Camera c = Camera.main;
        if (c == null)
            return;
        m_whatToMove.position = m_aroundWhat.position;
        m_whatToMove.position += c.transform.up * m_radiusDistance;
        m_whatToMove.RotateAround(m_aroundWhat.position, c.transform.forward, m_currentAngle);
    }

    private void OnValidate()
    {
        RefreshPosition();
    }
}
