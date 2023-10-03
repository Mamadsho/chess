using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;

public class checkmateAnimation : MonoBehaviour
{
    TMP_Text text;
    int i = 0;
    string initText;
    // Start is called before the first frame update
    void Start()
    {
        OnEnable();
    }
    void OnEnable()
    {
        text = GetComponent<TMP_Text>();
        initText = text.text;
        //text.text = "";
        InvokeRepeating("AddLetterToEnd", 0, 0.1f);
    }
    void AddLetterToEnd()
    {
        text.text = initText.Substring(0,i);
        if (i > initText.Length - 1)
        {
            i = 0;
            CancelInvoke();
        }
        else
        {
            i++;
        }
    }
}
