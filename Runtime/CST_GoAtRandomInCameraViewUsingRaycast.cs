using UnityEngine;
using UnityEngine.Events;

public class CST_GoAtRandomInCameraViewUsingRaycast : MonoBehaviour
{

    public Transform m_whatToMove;
    public Camera m_cameraToUse;
    public LayerMask m_allowToHit = ~1;
    public UnityEvent m_noRaycastHitFound;
    [ContextMenu("Move at random position")]
    public void MoveAtRandomPosition()
    {

        if (m_cameraToUse == null)
            m_cameraToUse = Camera.main;
        if (m_cameraToUse == null)
            return;
        Ray position = m_cameraToUse.ViewportPointToRay(new Vector3(Random.Range(0, 1f), Random.Range(0, 1f), 0), Camera.MonoOrStereoscopicEye.Mono);
        Debug.DrawRay(position.origin, position.direction, Color.yellow, 5);
        if (Physics.Raycast(position, out RaycastHit hit, float.MaxValue, m_allowToHit))
        {
            m_whatToMove.position = hit.point;
        }
        else m_noRaycastHitFound.Invoke();
    }

}

