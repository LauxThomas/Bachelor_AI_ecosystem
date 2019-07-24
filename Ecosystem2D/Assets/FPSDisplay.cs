using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    
    float deltaTime = 0.0f;
    private int maxGen;
 
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        
        if (Input.GetKeyDown(KeyCode.Keypad1)||Input.GetKeyDown(KeyCode.Alpha1))
        {
            Time.timeScale = 1;
        }

        if (Input.GetKeyDown(KeyCode.Keypad2)||Input.GetKeyDown(KeyCode.Alpha2))
        {
            Time.timeScale += 1;
        }

        if (Input.GetKeyDown(KeyCode.Keypad3)||Input.GetKeyDown(KeyCode.Alpha3))
        {
            Time.timeScale = 10;
        }
        if (Input.GetKeyDown(KeyCode.Keypad4)||Input.GetKeyDown(KeyCode.Alpha4))
        {
            Time.timeScale = 25;
        }
        if (Input.GetKeyDown(KeyCode.Keypad8)||Input.GetKeyDown(KeyCode.Alpha8))
        {
            Time.timeScale = 100;
        }
        if (Input.GetKeyDown(KeyCode.Keypad9)||Input.GetKeyDown(KeyCode.Alpha9))
        {
            Time.timeScale /= 2;
        }
        
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Application.LoadLevel(Application.loadedLevel);
        }
        
    }
 
    void OnGUI()
    {
        
        int w = Screen.width, h = Screen.height;
 
        GUIStyle style = new GUIStyle();
 
        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 5 / 100;
        style.normal.textColor = Color.white;
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        text += BibitProducer.getStats();
        GUI.Label(rect, text, style);
    }
}