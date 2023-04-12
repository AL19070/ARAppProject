using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menuselect : MonoBehaviour
{
    public static int scene = 0;
    public static int sortNum;
    public void StartGame(int buttonNum)
    {
        //B_sort
        if (buttonNum == 1 || buttonNum == 2 || buttonNum == 3)
        {
            SceneManager.LoadScene(buttonNum + 11);
            sortNum = buttonNum;
        }
        //A_sort
        else if (buttonNum == 8)
        {
            SceneManager.LoadScene(1);
        }
        //A_sort
        else if (buttonNum == 9 || buttonNum == 10 || buttonNum == 11)
        {
            SceneManager.LoadScene(buttonNum);
        }
        else if (buttonNum == 0)
        {
            SceneManager.LoadScene(0);
        }
        //A_cop
        else if (buttonNum == 12 || buttonNum == 13 || buttonNum == 14)
        {
            SceneManager.LoadScene(buttonNum - 6);
            scene = buttonNum - 6;
        }
        //B_cop
        else
        {
            SceneManager.LoadScene(buttonNum- 2);
            scene = buttonNum - 2;
        }
        
        scene = buttonNum;
    }
    public void EndGame()
    {
        Application.Quit();
    }

    public void ObjectTextChange()
    {
        ImageManager.bottonCount += 1;
        if (ImageManager.textState == 2 || ImageManager.textState == 1)
        {
            ImageManager.textState = 0;
        }
        else if (ImageManager.textState == 0)
        {
            ImageManager.textState = 2;
        }
    }
}
