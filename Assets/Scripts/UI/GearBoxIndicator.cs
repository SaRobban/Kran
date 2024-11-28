using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class GearBoxIndicator : MonoBehaviour
{
    [Header("Dependencies")]
    public CraneMaster craneMaster;

    [Header("UI Objects")]
    public RectTransform centerParent;
    public RectTransform indicator;
    public float textDistanceFromIndicator;
    public TMP_Text wheels;
    public TMP_Text towerRotation;
    public TMP_Text boomTilt;
    public TMP_Text wireLength;
    float m_angleStep;

    // Start is called before the first frame update
    void Start()
    {
        TMP_Text[] gearText = { wheels, towerRotation, boomTilt, wireLength };

        m_angleStep = -90 / (gearText.Length - 1);
        for (int i = 0; i < gearText.Length; i++)
        {
            float distToFot = gearText[i].rectTransform.rect.width / 2;

            Vector3 dir = Quaternion.Euler(0, 0, m_angleStep * i) * centerParent.up;
            gearText[i].rectTransform.position = centerParent.position + dir * textDistanceFromIndicator;
            gearText[i].rectTransform.right = dir;
        }

        craneMaster.input.A_GearSwitch += OnGearSwitch;
    }
    private void OnDestroy()
    {
        craneMaster.input.A_GearSwitch -= OnGearSwitch;
    }
    void OnGearSwitch(CraneContols.Gears gear)
    {
        switch (gear)
        {
            case CraneContols.Gears.Move:
                indicator.up = Vector3.up;
                break;
            case CraneContols.Gears.Rotate:
                indicator.up = Quaternion.Euler(0, 0, m_angleStep) * Vector3.up;
                break;
            case CraneContols.Gears.Rilt:
                indicator.up = Quaternion.Euler(0, 0, m_angleStep * 2) * Vector3.up;
                break;
            case CraneContols.Gears.Wire:
                indicator.up = Quaternion.Euler(0, 0, m_angleStep * 3) * Vector3.up;
                break;
            default:
                return;
        }
    }
}
