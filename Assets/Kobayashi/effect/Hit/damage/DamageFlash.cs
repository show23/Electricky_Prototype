using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [ColorUsage(true, true)]
    [SerializeField] private Color _flashColor = Color.white;
    [SerializeField] private float _flashTime = 0.25f;
    [SerializeField] private AnimationCurve _flashSpeedCurve;

    private MeshRenderer[] _sprieRenderers;
    private Material[] _materials;

    private Coroutine _damageFlashCoroutine;
    private void Awake()
    {
        _sprieRenderers = GetComponentsInChildren<MeshRenderer>();

        Debug.Log(_sprieRenderers.Length);

        Init();
    }

    private void Init()
    { 
        _materials = new Material[_sprieRenderers.Length];

        //
        for(int i = 0 ; i < _materials.Length; i++) 
        {
            _materials[i] = _sprieRenderers[i].material;
        }
    }

    public void CallDamageFlash()
    {
        _damageFlashCoroutine = StartCoroutine(DamageFlasher());
    }

    private IEnumerator DamageFlasher()
    {
        //Set the color
        SetFlashColor();

        //lerp the flash amount
        float currentFlashAmount = 0f;
        float elapsedTime = 0f;
        while(elapsedTime < _flashTime) 
        {
            // iterate elapsedTime
            elapsedTime += Time.deltaTime;

            // lerp the flash amount
            currentFlashAmount = Mathf.Lerp(1f, _flashSpeedCurve.Evaluate(elapsedTime), (elapsedTime / _flashTime));
            SetFlashAmount(currentFlashAmount);

            yield return null;
        }
    }

    private void SetFlashColor()
    {
        //Set the color
        for(int i = 0; i < _materials.Length; i++)
        {
            _materials[i].SetColor("_FlashColor", _flashColor);

        }
    }

    private void SetFlashAmount(float amount)
    {
        //Set the flash amount
        for(int i = 0; i < _materials.Length; i++) 
        {
            _materials[i].SetFloat("_FlashAmount", amount);
        }
    }
}
