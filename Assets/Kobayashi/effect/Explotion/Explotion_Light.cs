using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Rendering.HighDefinition;

public class Explotion_Light : MonoBehaviour
{

    public float size = 1;
    public bool is_flame_light = true;
    public bool is_explotion_light = true;
    //private GameObject _parent = GatGameObject;

    // GameObject
    private GameObject flame_light_gameObject;
    private Transform explosion_flash_light_transform;
    private GameObject explosion_flash_light_gameobject;
    private HDAdditionalLightData explosion_light; 

    // constant
    private float anim_start_time = 0.0f;
    private float anim_middle_time = 0.5f;
    private float anim_end_time = 1.0f;

    private float anim_start_pos_y = 0.0f;
    private float anim_middle_pos_y = 5.0f;
    private double anim_start_intensity = 40000000.0;
    private float anim_middle_intensity = 9000000.0f;
    private float anim_end_intensity = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        flame_light_gameObject = this.gameObject.transform.GetChild(0).gameObject;
        flame_light_gameObject.gameObject.SetActive(is_flame_light);

        explosion_flash_light_transform = this.gameObject.transform.GetChild(1);
        explosion_flash_light_gameobject = explosion_flash_light_transform.gameObject;
        explosion_flash_light_gameobject.gameObject.SetActive(is_explotion_light);

        explosion_light = explosion_flash_light_gameobject.GetComponent<HDAdditionalLightData>();
    

        SetExplotionAnim();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetExplotionAnim()
    {
        anim_middle_pos_y *= size;


        if (!explosion_flash_light_gameobject.GetComponent<Animation>())
        {
        	explosion_flash_light_gameobject.AddComponent<Animation>();
        }

        AnimationClip clip = new AnimationClip();
        AnimationCurve curve_pos;
        AnimationCurve curve_ints;

        Keyframe start_key_frame_pos;
        Keyframe middle_key_frame_pos;

        Keyframe start_key_frame_ints;
        Keyframe middle_key_frame_ints;
        Keyframe end_key_frame_ints;

        start_key_frame_pos = new Keyframe(anim_start_time, anim_start_pos_y);
        middle_key_frame_pos = new Keyframe(anim_middle_time, anim_middle_pos_y);
        start_key_frame_ints = new Keyframe(anim_start_time, (float)anim_start_intensity);
        middle_key_frame_ints = new Keyframe(anim_middle_time, anim_middle_intensity);
        end_key_frame_ints = new Keyframe(anim_end_time, anim_end_intensity);

        curve_pos = new AnimationCurve(start_key_frame_pos, middle_key_frame_pos);
        curve_ints = new AnimationCurve(start_key_frame_ints, middle_key_frame_ints, end_key_frame_ints);

        clip.SetCurve("", typeof(Transform), "localPosition.y", curve_pos);
        clip.SetCurve("", typeof(float), "HDAdditionalLightData.Intensity", curve_ints);

        explosion_flash_light_gameobject.GetComponent<Animation>().AddClip(clip, "ExplosionAnimation");
        explosion_flash_light_gameobject.GetComponent<Animation>().Play("ExplosionAnimation");
    }
}
