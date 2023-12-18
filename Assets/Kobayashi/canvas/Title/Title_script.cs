using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title_script : MonoBehaviour
{
    [SerializeField] private Transform _fade;
    private Fade fade;
    private FadeImage fadeImage;
    [SerializeField] private float _fadeSpeed = 1.0f;

    [SerializeField] private GameObject _logo;
    [SerializeField] private GameObject _titleObject;

    private IEnumerator s;

    // Start is called before the first frame update
    void Start()
    {
        fade = _fade.GetComponent<Fade>();
        fadeImage = _fade.gameObject.GetComponent<FadeImage>();

        StartCoroutine(TitleCoroutine(_fadeSpeed));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator TitleCoroutine(float time)
    {
        _logo.SetActive(true);
        _titleObject.SetActive(false);

        fadeImage.MainColor = Color.black;
        fadeImage.SetMaskTexture("Assets/Kobayashi/Flame/mask01.png");

        fade.FadeOut(time);        
        yield return new WaitForSeconds(time);

        yield return new WaitForSeconds(1.0f);

        fade.FadeIn(time);
        yield return new WaitForSeconds(time);

        Debug.Log("in");

        _logo.SetActive(false);
        _titleObject.SetActive(true);

        fadeImage.SetMaskTexture("Assets/Kobayashi/Flame/Clip_cross_repetition.png");

        fade.FadeOut(time);
        yield return new WaitForSeconds(time);

        fadeImage.MainColor = Color.white;
        fadeImage.SetMaskTexture("Assets/Kobayashi/Flame/mask02.png");

        yield return new WaitForSeconds(time);

        fade.FadeIn(time);
        yield return new WaitForSeconds(time);

        fade.FadeOut(time);
        yield return new WaitForSeconds(time);
    }
}
