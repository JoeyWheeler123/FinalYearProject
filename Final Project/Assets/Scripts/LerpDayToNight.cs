﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class LerpDayToNight : MonoBehaviour
{
    [SerializeField] private VolumeProfile _dayTimeSkyboxProfile;
    [SerializeField] private VolumeProfile _nightTimeSkyboxProfile;
    [SerializeField] private VolumeProfile _changingSkyboxProfile;

    private GradientSky _daySkybox;
    private GradientSky _nightSkybox;
    private GradientSky _changingSkybox;
    
    public static LerpDayToNight instance;

    [SerializeField] private float debugLerpTime;

    [SerializeField] private GameObject _directionalLight;

    [SerializeField] private Quaternion _lightRotation;
    [SerializeField] private Quaternion _lightStartRotation;
    [SerializeField] private Color _lightColor;
    [SerializeField] private Color _lightStartColor;

    private Light _dirLight;

    public bool debug;

    // Start is called before the first frame update
    void Start()
    {
        
        
#if UNITY_EDITOR
        EditorApplication.playmodeStateChanged += ModeChanged;
#endif
        _dayTimeSkyboxProfile.TryGet(out _daySkybox);
        _nightTimeSkyboxProfile.TryGet(out _nightSkybox);


        _changingSkyboxProfile.TryGet(out _changingSkybox);


        _changingSkybox.bottom.value = _daySkybox.bottom.value;
        _changingSkybox.middle.value = _daySkybox.middle.value;
        _changingSkybox.top.value = _daySkybox.top.value;

        instance = this;

        _dirLight = _directionalLight.GetComponent<Light>();
        
        _lightStartRotation = _dirLight.transform.rotation;
        _lightStartColor = _dirLight.color;
        
        if (debug)
        {
            StartCoroutine(ChangeDayToNight(debugLerpTime));
        }
    }

    public static void LerpToNight(float secondsToLerpFor)
    {
        instance.StartCoroutine(instance.ChangeDayToNight(secondsToLerpFor));
    }

    public static void SetToNight()
    {
        instance._changingSkybox.bottom.value = instance._nightSkybox.bottom.value;
        instance._changingSkybox.middle.value = instance._nightSkybox.middle.value;
        instance._changingSkybox.top.value = instance._nightSkybox.top.value;

        instance._dirLight.color = instance._lightColor;
        instance._dirLight.transform.rotation = instance._lightRotation;
    }
    public static void SetToDay()
    {
        instance._changingSkybox.bottom.value = instance._daySkybox.bottom.value;
        instance._changingSkybox.middle.value = instance._daySkybox.middle.value;
        instance._changingSkybox.top.value = instance._daySkybox.top.value;
        
        instance._dirLight.color = instance._lightStartColor;
        instance._dirLight.transform.rotation = instance._lightStartRotation;
    }


    private IEnumerator ChangeDayToNight(float timeToLerpFor)
    {
        float timeLeft = timeToLerpFor;


        while (timeLeft > 0)
        {
            print("gas");
            _changingSkybox.gradientDiffusion.value = Mathf.Lerp(_changingSkybox.gradientDiffusion.value,
                _nightSkybox.gradientDiffusion.value, Time.deltaTime / timeLeft);


            _changingSkybox.bottom.value = Color.Lerp(_changingSkybox.bottom.value, _nightSkybox.bottom.value,
                Time.deltaTime / timeLeft);
            _changingSkybox.middle.value = Color.Lerp(_changingSkybox.middle.value, _nightSkybox.middle.value,
                Time.deltaTime / timeLeft);
            _changingSkybox.top.value =
                Color.Lerp(_changingSkybox.top.value, _nightSkybox.top.value, Time.deltaTime / timeLeft);
            
            _directionalLight.transform.rotation = Quaternion.Slerp(_directionalLight.transform.rotation, _lightRotation, Time.deltaTime / timeLeft);
            _dirLight.color = Color.Lerp(_dirLight.color, _lightColor, Time.deltaTime / timeLeft);
            
            timeLeft -= Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
    }

#if UNITY_EDITOR
    

    void ModeChanged()
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode &&
            EditorApplication.isPlaying)
        {
            _changingSkybox.bottom.value = _daySkybox.bottom.value;
            _changingSkybox.middle.value = _daySkybox.middle.value;
            _changingSkybox.top.value = _daySkybox.top.value;
        }
    }
#endif
}