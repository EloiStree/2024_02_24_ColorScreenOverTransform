using UnityEngine;
using UnityEngine.Events;

public class CST_GoInCameraViewUsingAngle : MonoBehaviour
{

    public Transform m_whatToMove;
    public Camera m_cameraToUse;
    public LayerMask m_allowToHit = ~1;
    public UnityEvent m_noRaycastHitFound;

    public float m_horizontalAngle = 10;
    public float m_verticalAngle = 10;
    [ContextMenu("Move at random position")]
    public void MoveAtRandomPosition()
    {

        if (m_cameraToUse == null)
            m_cameraToUse = Camera.main;
        if (m_cameraToUse == null)
            return;

        m_whatToMove.position = m_cameraToUse.transform.position+m_cameraToUse.transform.forward*0.1f ;
        m_whatToMove.rotation = m_cameraToUse.transform.rotation;
        m_whatToMove.Rotate(Random.Range(-m_verticalAngle, m_verticalAngle), Random.Range(-m_horizontalAngle, m_horizontalAngle), 0,Space.Self);


        if (Physics.Raycast(m_whatToMove.position, m_whatToMove.forward , out RaycastHit hit, float.MaxValue * 0.5f, m_allowToHit))
        {
            Debug.DrawRay(m_whatToMove.position, hit.point, Color.green, 5);
            m_whatToMove.position = hit.point;
        }
        else {

            Debug.DrawRay(m_whatToMove.position, m_whatToMove.forward * 20, Color.red, 5);
            m_noRaycastHitFound.Invoke();
        }
    }

}

