using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Score : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _text;

    private int _score;

    // Start is called before the first frame update
    void Start()
    {
        _score = 0;
        _text.text = _score.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddScore(int score)
    {
        _score += score;
        _text.text = _score.ToString();
    }
}
