using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class winSenario : MonoBehaviour
{
    public Image image;
    public int targetScore = 300;
    public void DidIWin(int admount)
    {
        if(admount > targetScore)
        {
            image.gameObject.SetActive(true);
        }
    }
}
