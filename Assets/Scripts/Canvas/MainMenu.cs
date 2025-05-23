using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Scene mainGameScene;
    public GameObject cameraHolder;
    public RotateRoundAxis rotateRoundAxis;

    public CanvasGroup fadeOut;

    bool move = false;
    public float moveSpeed = 5;
    public float rotateSpeed = 1;
    public float fadeSpeed = 1;

    public float rotateDuration = 0.5f;
    float rotateTimer = 0;
    
    float moveTimer = 0;

    void Start()
    {
        
    }

    
    void Update()
    {
        if(move)
        {
            MoveTowardsHole();

        }
    }

    public void StartGame()
    {
        rotateRoundAxis.enabled = false;
        fadeOut.enabled = true;
        move = true;
        StartCoroutine(LoadMainScene());
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void MoveTowardsHole()
    {
        float targetX = 90f;
        float currentX = cameraHolder.transform.eulerAngles.x;

        if (currentX > 180f) currentX -= 360f;

        float newX = Mathf.MoveTowardsAngle(currentX, targetX, rotateSpeed * Time.deltaTime);
        Vector3 newEuler = cameraHolder.transform.eulerAngles;
        newEuler.x = newX;
        cameraHolder.transform.eulerAngles = newEuler;

        cameraHolder.transform.position += Vector3.down * moveSpeed * Time.deltaTime;

        if (Mathf.Approximately(newX, targetX))
        {
            fadeOut.alpha += Time.deltaTime * fadeSpeed;
            fadeOut.alpha = Mathf.Clamp01(fadeOut.alpha);
        }
    }

    IEnumerator LoadMainScene()
    {
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(1);
    }


    public void QuitGame()
    {
        Application.Quit();
    }

    

    

}
