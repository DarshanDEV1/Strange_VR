using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Manager : MonoBehaviour
{
    [SerializeField] internal TMP_Text m_Controller_Debug_Text;
    [SerializeField] internal TMP_Text m_Game_Debug_Text;
    [SerializeField] internal TMP_Text m_Power_Debug_Text;

    [SerializeField] internal OVRCameraRig m_CameraRig;

    [SerializeField] internal bool[] m_Power_Steps = new bool[2];

    [SerializeField] internal GameObject m_Gola_Prefab;
    [SerializeField] internal GameObject m_Hand_Gola_Prefab;

    [SerializeField] internal GameObject m_Portal;

    private bool m_Shooted = false;

    private GameObject m_Portal_Object;

    private void Update()
    {
        ControllerManagement();
        GameManagement();
        CheckPower();
    }

    private void ControllerManagement()
    {
        if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger)) { DebugText("Right Index Button Press"); } //Right Hand Index Button Press
        if (OVRInput.Get(OVRInput.RawButton.LIndexTrigger)) { DebugText("Left Index Button Press"); } //Left Hand Index Button Press
        if (OVRInput.Get(OVRInput.RawButton.RHandTrigger)) { DebugText("Right Hold Button Press"); } //Right Hand Hold Button Press
        if (OVRInput.Get(OVRInput.RawButton.LHandTrigger)) { DebugText("Left Hold Button Press"); } //Left Hand Hold Button Press
        if (OVRInput.Get(OVRInput.RawButton.A)) { DebugText("A Button Pressed"); } //Controller A Button Pressed
        if (OVRInput.Get(OVRInput.RawButton.B)) { DebugText("B Button Pressed"); } //Controller B Button Pressed
        if (OVRInput.Get(OVRInput.RawButton.X)) { DebugText("X Button Pressed"); } //Controller X Button Pressed
        if (OVRInput.Get(OVRInput.RawButton.Y)) { DebugText("Y Button Pressed"); } //Controller Y Button Pressed
    }

    private void GameManagement()
    {
        Vector3 m_left_hand_position = m_CameraRig.leftControllerInHandAnchor.position;
        Vector3 m_right_hand_position = m_CameraRig.rightControllerInHandAnchor.position;

        Vector3 m_resultant_vector = m_left_hand_position - m_right_hand_position;
        GameDebugText((m_resultant_vector.x < 0 ? m_resultant_vector * -1 : m_resultant_vector).ToString());
    }

    private Vector3 GetCTRLAnchor(CTRL_Anchor m_anchor)
    {
        switch (m_anchor)
        {
            case CTRL_Anchor.Left:
                return m_CameraRig.leftControllerInHandAnchor.position;
            case CTRL_Anchor.Right:
                return m_CameraRig.rightControllerInHandAnchor.position;
            case CTRL_Anchor.Resultant:
                Vector3 m_resultant_vector = (m_CameraRig.leftControllerInHandAnchor.position) - (m_CameraRig.rightControllerInHandAnchor.position);
                return (m_resultant_vector.x < 0 || m_resultant_vector.z < 0 ? m_resultant_vector * -1 : m_resultant_vector);
            case CTRL_Anchor.HandResultant:
                Vector3 m_hand_resultant_vector = (m_CameraRig.centerEyeAnchor.position) - (m_CameraRig.rightControllerInHandAnchor.position);
                return (m_hand_resultant_vector.x < 0 || m_hand_resultant_vector.z < 0 ? m_hand_resultant_vector * -1 : m_hand_resultant_vector);
            default:
                return Vector3.zero;
        }
    }

    private void CheckPower()
    {
        if (GetCTRLAnchor(CTRL_Anchor.Resultant).x > 0.5 && (OVRInput.Get(OVRInput.RawButton.RHandTrigger) && OVRInput.Get(OVRInput.RawButton.LHandTrigger)))
        {
            for (int i = 0; i < m_Power_Steps.Length; i++)
            {
                m_Power_Steps[i] = false;
            }
            m_Shooted = false;

            m_Portal_Object = Instantiate(m_Portal, m_CameraRig.centerEyeAnchor.position, Quaternion.identity);
            m_Portal_Object.transform.position = new Vector3(m_Portal_Object.transform.position.x, m_Portal_Object.transform.position.y, m_Portal_Object.transform.position.z + 2);
            m_Portal_Object.transform.localScale = new Vector3(1, 0, 0);

            Destroy(m_Portal_Object, 1f);

            StartCoroutine(Power());
        }
        else if (GetCTRLAnchor(CTRL_Anchor.Resultant).z > 0.5 && (OVRInput.Get(OVRInput.RawButton.RHandTrigger) && OVRInput.Get(OVRInput.RawButton.LHandTrigger)))
        {
            for (int i = 0; i < m_Power_Steps.Length; i++)
            {
                m_Power_Steps[i] = false;
            }
            m_Shooted = false;

            m_Portal_Object = Instantiate(m_Portal, m_CameraRig.centerEyeAnchor.position, Quaternion.identity);
            m_Portal_Object.transform.position = new Vector3(m_Portal_Object.transform.position.x, m_Portal_Object.transform.position.y, m_Portal_Object.transform.position.z + 2);
            m_Portal_Object.transform.localScale = new Vector3(1, 0, 0);

            Destroy(m_Portal_Object, 1f);

            StartCoroutine(Power());
        }

        if (GetCTRLAnchor(CTRL_Anchor.HandResultant).x > 0.5 && OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
        {
            for (int i = 0; i < m_Power_Steps.Length; i++)
            {
                m_Power_Steps[i] = false;
            }
            m_Shooted = false;

            StartCoroutine(HandPower());
        }
        else if (GetCTRLAnchor(CTRL_Anchor.HandResultant).z > 0.5 && OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
        {
            for (int i = 0; i < m_Power_Steps.Length; i++)
            {
                m_Power_Steps[i] = false;
            }
            m_Shooted = false;

            StartCoroutine(HandPower());
        }

    }

    private IEnumerator HandPower()
    {
        switch (PowerStep())
        {
            case -1:
                m_Power_Steps[0] = true;
                yield return new WaitForSeconds(.5f);
                PowerDebugText("First Step Complete");
                StartCoroutine(HandPower());
                break;
            case 0:
                if (GetCTRLAnchor(CTRL_Anchor.HandResultant).y > 0.5)
                {
                    m_Power_Steps[1] = true;
                    yield return new WaitForSeconds(.5f);
                    PowerDebugText("Second Step Complete");
                    if ((GetCTRLAnchor(CTRL_Anchor.HandResultant).x < 1 && GetCTRLAnchor(CTRL_Anchor.HandResultant).y < 1) || 
                        (GetCTRLAnchor(CTRL_Anchor.HandResultant).z < 1 && GetCTRLAnchor(CTRL_Anchor.HandResultant).y < 1))
                    {
                        yield return new WaitForSeconds(.5f);
                        PowerDebugText("Final Step Completed");
                        yield return new WaitForSeconds(.2f);
                        if (!m_Shooted)
                        {
                            InstantiateHandGola();
                        }
                    }
                    StartCoroutine(HandPower());
                }
                break;
            case 1:
                PowerDebugText("Final Step To Be Completed");
                break;
            default:
                StopAllCoroutines();
                yield break;
        }
    }

    private IEnumerator Power()
    {
        switch (PowerStep())
        {
            case -1:
                m_Power_Steps[0] = true;
                yield return new WaitForSeconds(.5f);
                PowerDebugText("First Step Complete");
                StartCoroutine(Power());
                break;
            case 0:
                if (GetCTRLAnchor(CTRL_Anchor.Resultant).y > 0.5)
                {
                    m_Power_Steps[1] = true;
                    m_Portal_Object.transform.localScale = new Vector3(1, .8f, 1);
                    yield return new WaitForSeconds(.5f);
                    PowerDebugText("Second Step Complete");
                    if ((GetCTRLAnchor(CTRL_Anchor.Resultant).x < 1 && GetCTRLAnchor(CTRL_Anchor.Resultant).y < 1) || 
                        (GetCTRLAnchor(CTRL_Anchor.Resultant).z < 1 && GetCTRLAnchor(CTRL_Anchor.Resultant).y < 1))
                    {
                        yield return new WaitForSeconds(.5f);
                        PowerDebugText("Final Step Completed");
                        yield return new WaitForSeconds(.2f);
                        if (!m_Shooted)
                        {
                            Destroy(m_Portal_Object);
                            InstantiateGola();
                        }
                    }
                    StartCoroutine(Power());
                }
                break;
            case 1:
                PowerDebugText("Final Step To Be Completed");
                break;
            default:
                StopAllCoroutines();
                yield break;
        }
    }

    private int PowerStep()
    {
        for (int i = 0; i < m_Power_Steps.Length; i++)
        {
            if (m_Power_Steps[i] == true)
            {
                return i;
            }
        }
        return -1;
    }

    private void InstantiateGola()
    {
        m_Shooted = true;
        GameObject m_gola = Instantiate(m_Gola_Prefab, m_CameraRig.centerEyeAnchor.position, Quaternion.identity);
        Destroy(m_gola, 3f);
    }

    private void InstantiateHandGola()
    {
        m_Shooted = true;
        GameObject m_gola = Instantiate(m_Hand_Gola_Prefab, m_CameraRig.centerEyeAnchor.position, Quaternion.identity);
        Destroy(m_gola, 3f);
    }

    private void DebugText(string m_text)
    {
        m_Controller_Debug_Text.text = m_text;
    }

    private void GameDebugText(string m_text)
    {
        m_Game_Debug_Text.text = m_text;
    }

    private void PowerDebugText(string m_text)
    {
        m_Power_Debug_Text.text = m_text;
    }

    enum CTRL_Anchor
    {
        Left,
        Right,
        Resultant,
        HandResultant
    }
}
