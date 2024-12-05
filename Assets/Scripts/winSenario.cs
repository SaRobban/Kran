using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class winSenario : MonoBehaviour
{
    public Image image;
    public TMPro.TMP_Text score;
    public int targetScore = 300;
    public void DidIWin(int admount)
    {
        score.SetText("Coal : " +  admount + "/"+targetScore);
        if(admount > targetScore)
        {
            image.gameObject.SetActive(true);
        }
    }
}
