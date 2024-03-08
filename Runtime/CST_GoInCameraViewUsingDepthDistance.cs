using UnityEngine;
using UnityEngine.Events;

public class CST_GoInCameraViewUsingDepthDistance : MonoBehaviour {

    public Transform m_whatToMove;
    public float m_distanceOfCamera=10;
    public Camera m_cameraToUse;

    [ContextMenu("Move at random position")]
    public void MoveAtRandomPosition() {

        if(m_cameraToUse== null)
            m_cameraToUse = Camera.main;
        if (m_cameraToUse == null)
            return;
       Vector3 position = m_cameraToUse.ViewportToWorldPoint(new Vector3(Random.Range(0,1f), Random.Range(0,1f), m_distanceOfCamera),Camera.MonoOrStereoscopicEye.Mono);
       m_whatToMove.position = position;
    }

}
