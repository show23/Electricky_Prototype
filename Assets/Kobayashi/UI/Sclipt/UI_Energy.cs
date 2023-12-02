using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Energy : MonoBehaviour
{
    [SerializeField] protected Image _gaugeImage;

    private PlayerControll _playerControl;

    private float _lastEnergy;

    // Start is called before the first frame update
    void Start()
    {
        _playerControl = GameObject.Find("Player").GetComponent<PlayerControll>();

        
    }

    // Update is called once per frame
    void Update()
    {
        /*電気ゲージは頻繁に変化するし常に更新する方針で良いか
          更新時に前フレームと変化あるか見る
        */

        
    }
}
