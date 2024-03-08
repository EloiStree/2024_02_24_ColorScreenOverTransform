using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CST_OnExactColorDoSomething : MonoBehaviour
{

    public Color m_colorToLookFor;
    public Color [] m_otherColors;
    public UnityEvent m_onColorFound;

    public void PushColorIn(Color color) {

        if (m_colorToLookFor.r == color.r 
            && m_colorToLookFor.g == color.g
            && m_colorToLookFor.b == color.b)
            m_onColorFound.Invoke();
        foreach (var item in m_otherColors)
        {
            if (item.r == color.r
          && item.g == color.g
          && item.b == color.b)
                m_onColorFound.Invoke();
        }
    }
}
