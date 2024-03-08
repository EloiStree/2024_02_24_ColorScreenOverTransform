using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct UnityWindowPositionVector2
{
    public Vector2 m_unity;
    public POINTWINDOWPIXEL m_window;

    public void Set(Vector2 unity, POINTWINDOWPIXEL window)
    {
        m_unity = unity;
        m_window = window;
    }
}

[Serializable]
public struct POINTWINDOWPIXEL
{
    public int X;
    public int Y;
}

public class ReadPixelColorOverUnityScreenZoneMono : MonoBehaviour
{
    public static ReadPixelColorOverUnityScreenZoneMono InstanceInTheScene;
    public Transform m_target;
    public Vector3 m_targetViewPosition;
    public Vector3 m_targetUnityScreenPosition;
    public POINTWINDOWPIXEL m_estimatedPixel;
    public Color m_estimatedColor;
    public Color m_underMousePixel;

    public UnityEvent<Color> m_onPixelColorUpdated;
    public UnityEvent<string> m_onDebugMousePosition;
    public void TryToEstimateTheColorAndPositionOfTarget(Transform givenTarget,  out Color colorOnWindow)
    {
        TryToEstimateTheColorAndPositionOfTarget(givenTarget.position, out _ , out colorOnWindow);
    }
    public void TryToEstimateTheColorAndPositionOfTarget(Transform givenTarget, out UnityWindowPositionVector2 currentPosition, out Color colorOnWindow)
    {
        TryToEstimateTheColorAndPositionOfTarget(givenTarget.position, out  currentPosition, out  colorOnWindow);
    }
    public void TryToEstimateTheColorAndPositionOfTarget(Vector3 targetWorldPosition, out UnityWindowPositionVector2 currentPositoin, out Color colorOnWindow  )
    {
        currentPositoin = new UnityWindowPositionVector2();
        Vector3 targetUnityScreenPosition = Camera.main.WorldToScreenPoint(targetWorldPosition);
        currentPositoin.m_unity.x = targetUnityScreenPosition.x;
        currentPositoin.m_unity.y = targetUnityScreenPosition.y;
        double xDif = (targetUnityScreenPosition.x - m_leftDownKey.m_unity.x) / m_pixelScale.m_unityToWindowRationPixelHorizontal;
        double yDif = (targetUnityScreenPosition.y - m_leftDownKey.m_unity.y) / m_pixelScale.m_unityToWindowRationPixelVertical;
        currentPositoin.m_window.X = (int)(m_leftDownKey.m_window.X + xDif);
        currentPositoin.m_window.Y = (int)(m_leftDownKey.m_window.Y - yDif);
        colorOnWindow = GetPixelColorWithWindowCoordinate(currentPositoin.m_window.X, currentPositoin.m_window.Y);
    }

    [Header("Corner Position")]
    public UnityWindowPositionVector2 m_leftTopKey;
    public UnityWindowPositionVector2 m_rightDownKey;
    public UnityWindowPositionVector2 m_leftDownKey;
    [Header("Mouse Current Position")]
    public UnityWindowPositionVector2 m_mouseKeyPosition;
    public POINTWINDOWPIXEL m_windowNativePosition;
     RECT m_windowMainScreenRect;



    public Vector2 m_targetPixelPosition;
    public Vector2 m_pixelDifference;





    public WidthScaleRatio m_pixelScale;

    [System.Serializable]
    public class WidthScaleRatio {

        public int m_leftRightWindow;
        public int m_leftRightUnity;
        public int m_botTopWindow;
        public int m_botTopUnity;

        public double m_unityToWindowRationPixelHorizontal;
        public double m_unityToWindowRationPixelVertical;
        public double m_winToUnityRationPixelHorizontal;
        public double m_winToUnityRationPixelVertical;
        public void Reset() {
            m_leftRightWindow=0;
            m_leftRightUnity = 0;
            m_botTopWindow = 0;
            m_botTopUnity = 0;
            m_unityToWindowRationPixelHorizontal=0;
            m_unityToWindowRationPixelVertical = 0;
            m_winToUnityRationPixelHorizontal = 0;
            m_winToUnityRationPixelVertical = 0;

        }
    }




    public float m_timeBetweenCheckAndUpdate = 0.1f;

    private void Awake()
    {
        InstanceInTheScene = this;
        ResetCalibrationPointToCenterScreen();
        InvokeRepeating("RefreshInfo", m_timeBetweenCheckAndUpdate, m_timeBetweenCheckAndUpdate);
    }

    [ContextMenu("Reset Calibration point")]
    public  void ResetCalibrationPointToCenterScreen()
    {
        Vector3 p = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0.5f));
        m_leftTopKey.m_unity = new Vector2(p.x, p.y);
        m_rightDownKey.m_unity = new Vector2(p.x, p.y);
        m_pixelScale.Reset();
    }

   
    private void RefreshInfo()
    {
        //REFRESH IMPORTANT DATA
        RefreshCurrentMousePosition();
        RefreshCalibrationPoint();
        RefreshPixelScale();
        this.m_underMousePixel = GetPixelColorWithWindowCoordinate(m_mouseKeyPosition.m_window.X, m_mouseKeyPosition.m_window.Y);
        m_onPixelColorUpdated.Invoke(m_underMousePixel);
        m_onDebugMousePosition.Invoke($"NATIVE X{m_mouseKeyPosition.m_window.X} " +
            $"Y{m_mouseKeyPosition.m_window.Y}   \nUnity " +
            $"X{Math.Round(m_mouseKeyPosition.m_unity.x, 2)} " +
            $"Y{Math.Round(m_mouseKeyPosition.m_unity.y, 2)}");
    }


    private void RefreshCurrentMousePosition()
    {
        m_mouseKeyPosition.m_unity = Input.mousePosition;
        GetCursorPos(out m_mouseKeyPosition.m_window);
    }

    private void RefreshCalibrationPoint()
    {
        if (IsMouseWithinScreen())
        {

            if (m_mouseKeyPosition.m_unity.x < m_leftTopKey.m_unity.x && m_mouseKeyPosition.m_unity.y > m_leftTopKey.m_unity.y)
            {
                m_leftTopKey.Set(m_mouseKeyPosition.m_unity, m_mouseKeyPosition.m_window);

            }
            if (m_mouseKeyPosition.m_unity.x > m_rightDownKey.m_unity.x && m_mouseKeyPosition.m_unity.y < m_rightDownKey.m_unity.y)
            {
                m_rightDownKey.Set(m_mouseKeyPosition.m_unity, m_mouseKeyPosition.m_window);

            }
            if (m_mouseKeyPosition.m_unity.x < m_rightDownKey.m_unity.x && m_mouseKeyPosition.m_unity.y < m_rightDownKey.m_unity.y)
            {
                m_leftDownKey.Set(m_mouseKeyPosition.m_unity, m_mouseKeyPosition.m_window);

            }
        }
    }

    private void RefreshPixelScale()
    {

        //m_leftDownKey.m_unity.x = m_leftTopKey.m_unity.x;
        //m_leftDownKey.m_unity.y = m_rightDownKey.m_unity.y;
        //m_leftDownKey.m_window.X = m_leftTopKey.m_window.X;
        //m_leftDownKey.m_window.Y = m_rightDownKey.m_window.Y;
        m_pixelScale.m_leftRightUnity = (int)(m_rightDownKey.m_unity.x - m_leftTopKey.m_unity.x);
        m_pixelScale.m_leftRightWindow = (int)(m_rightDownKey.m_window.X - m_leftTopKey.m_window.X);
        m_pixelScale.m_botTopUnity = (int)(m_leftTopKey.m_unity.y - m_rightDownKey.m_unity.y);
        m_pixelScale.m_botTopWindow = (int)(m_leftTopKey.m_window.Y - m_rightDownKey.m_window.Y);
        
        m_pixelScale.m_unityToWindowRationPixelHorizontal = (double)Math.Abs(m_pixelScale.m_leftRightUnity) / (double)Math.Abs(m_pixelScale.m_leftRightWindow);
        m_pixelScale.m_unityToWindowRationPixelVertical = (double)Math.Abs(m_pixelScale.m_botTopUnity) / (double)Math.Abs(m_pixelScale.m_botTopWindow);
        m_pixelScale.m_winToUnityRationPixelHorizontal =   (double)Math.Abs(m_pixelScale.m_leftRightWindow) / (double)Math.Abs(m_pixelScale.m_leftRightUnity);
        m_pixelScale.m_winToUnityRationPixelVertical = (double)Math.Abs(m_pixelScale.m_botTopWindow) / (double)Math.Abs(m_pixelScale.m_botTopUnity);
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetCursorPos(out POINTWINDOWPIXEL lpPoint);
    public bool IsMouseWithinScreen()
    {
        // Get the screen dimensions
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Get the current mouse position
        Vector3 mousePosition = Input.mousePosition;

        // Check if the mouse coordinates are within the screen boundaries
        if (mousePosition.x >= 0 && mousePosition.x <= screenWidth &&
            mousePosition.y >= 0 && mousePosition.y <= screenHeight)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    [DllImport("user32.dll")]
    public static extern IntPtr GetDC(IntPtr hwnd);

    [DllImport("user32.dll")]
    public static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

    [DllImport("gdi32.dll")]
    public static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);



    public Color GetPixelColorWithWindowCoordinate(float x, float y) {
        return GetPixelColorWithWindowCoordinate((int)x, (int)y);
    }
    public  Color GetPixelColorWithWindowCoordinate(int x, int y)
    {
        IntPtr hdc = GetDC(IntPtr.Zero);
        uint pixel = GetPixel(hdc, x, y);
        ReleaseDC(IntPtr.Zero, hdc);

        Color color = new Color32(
            (byte)(pixel & 0x000000FF),
            (byte)((pixel & 0x0000FF00) >> 8),
            (byte)((pixel & 0x00FF0000) >> 16),
            255);
        return color;
    }



    [DllImport("user32.dll")]
    private static extern IntPtr GetDesktopWindow();

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

    [Serializable]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }







    public RECT windowRect;
    public RECT clientRect;


    void Start()
    {
        InvokeRepeating("RefreshWindowSizeInfo", 0, 1);
    }

    private void RefreshWindowSizeInfo()
    {
        IntPtr desktopHandle = GetDesktopWindow();

        GetWindowRect(desktopHandle, out windowRect);
        //Debug.Log("Window Position - X: " + windowRect.Left + ", Y: " + windowRect.Top);
        // Debug.Log("Window Size - Width: " + (windowRect.Right - windowRect.Left) + ", Height: " + (windowRect.Bottom - windowRect.Top));

        GetClientRect(desktopHandle, out clientRect);
        //Debug.Log("Client Area Size - Width: " + (clientRect.Right - clientRect.Left) + ", Height: " + (clientRect.Bottom - clientRect.Top));
    }

}
