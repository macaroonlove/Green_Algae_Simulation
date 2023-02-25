using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using StylizedWater;
using Cinemachine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public CinemachineVirtualCamera Sub;

    // 일조량
    public Volume volume;
    VolumeProfile volumeProfile;
    public Light DirectionalLight;
    public Slider LightAmount;

    // 풍속
    public WindZone WindData;
    public Slider WindAmount;
    
    // 강수량
    public ParticleSystem Rain_System;
    public GameObject River;
    public Slider RainAmount;
    public float RiverHeight = 0;
    ParticleSystem.EmissionModule emission;
    UnityEngine.Rendering.Universal.ColorAdjustments CA;

    // 물의 유속
    public TextMeshProUGUI WaterFlowText;
    public Material Water;
    private float WaterFlow;

    // 수온
    public TextMeshProUGUI WaterTemperatureText;
    private float WaterTemperature;

    // 영양분
    public TextMeshProUGUI NutrientPText;
    public TextMeshProUGUI NutrientNText;
    private float NutrientP;
    private int NutrientN;

    // 녹조의 양
    public TextMeshProUGUI AlgaeText;
    public int Algae;
    private float RWT;
    private int RWF;
    private int RS;
    private int RNN;

    void Start()
    {
        emission = Rain_System.emission;
        volumeProfile = volume.profile;
        volumeProfile.TryGet(out CA);
        Reset();
        StartCoroutine(EnviPrams());
    }

    public void Reset()
    {
        // 초기화
        SoundManager.Instance.SunBGM(0);
        SoundManager.Instance.WindBGM(0);
        SoundManager.Instance.RainBGM(0);
        Rain_System.gravityModifier = 0;
        CA.postExposure.value = 0;
        DirectionalLight.intensity = 0;
        emission.rateOverTime = 0;
        RiverHeight = 0;
        River.transform.position = new Vector3(River.transform.position.x, 37, River.transform.position.z);
        Water.SetVector("_NormalsMovement", new Vector3(0.1f, 0.01f));
        WindData.windMain = 0;
        LightAmount.value = 0;
        WindAmount.value = 0;
        RainAmount.value = 0;
        NutrientN = 0;
        NutrientP = 0;
        Algae = 0;
    }

    IEnumerator EnviPrams()
    {
        while (true)
        {
            // 강수량에 따른 물의 높이
            RiverHeight = (RainAmount.value * 0.0005f) - WaterFlow/120;
            RiverHeight = Mathf.Clamp(RiverHeight, -1, 1);
            if(River.transform.position.y < 44 && RiverHeight > 0)
            {
                River.transform.position += new Vector3(0, RiverHeight, 0);
            }
            else if(RiverHeight < 0 && River.transform.position.y > 30)
            {
                River.transform.position += new Vector3(0, RiverHeight, 0);
            }

            // 물의 유속: 강수량 + 바람의 세기
            WaterFlow = Mathf.Round(((WindAmount.value * 70) + (RainAmount.value / 10)) / 2) * 0.3f;
            WaterFlowText.text = $"물의 유속: {WaterFlow} cm/sec";
            Water.SetVector("_NormalsMovement", new Vector3(WaterFlow, 0.01f));

            // 온도: 일조량 + 강수량 + 바람의 세기
            WaterTemperature = Mathf.Round(((LightAmount.value * 100) - (WindAmount.value * 30) - (RainAmount.value / 5)) * 3) * 0.1f;
            WaterTemperature = Mathf.Clamp(WaterTemperature, 0, 35);
            WaterTemperatureText.text = "수온: " + WaterTemperature + " ˚C";

            // 영양분: 강수량
            NutrientN += Random.Range(50, (int)RainAmount.value) - 50;
            NutrientN = Mathf.Clamp(NutrientN, 0, 100000000);
            NutrientNText.text = "질소(N): " + NutrientN + " mg/L";

            NutrientP += float.Parse(string.Format("{0:N3}", Random.Range(0.001f, RainAmount.value / 50000)));
            NutrientP = Mathf.Clamp(NutrientP, 0, 2);
            NutrientPText.text = "인(P): " + NutrientP + " mg/L";

            // 개체수: 물의 유속 + 온도(20도~25도) + 영양분() + 클로로필(다다익선) (5000mL정도
            RWT = Random.Range(0, WaterTemperature > 30 ? 0.6f : WaterTemperature > 25 ? 1 : WaterTemperature > 20 ? 1 : WaterTemperature > 15 ? 0.5f : WaterTemperature > 10 ? 0.3f : WaterTemperature > 5 ? 0.1f : 0);
            RNN = Mathf.Clamp(Random.Range(0, (int)(NutrientN * RWT)), 0, 100);
            NutrientN -= RNN;
            NutrientP -= RNN * 0.0001f;
            RS = (int)(RNN * (LightAmount.value > 0.5f ? 1.5f : 0.5f));
            RWF = WaterFlow > 3 ? 100 - (int)(50 / WaterFlow) : 0;

            Algae += (RS - RWF);
            Water.SetFloat("_Green_Blend", Algae * 0.0001f);
            Algae = Mathf.Clamp(Algae, 0, 100000);

            AlgaeText.text = Algae + " <size=\"30\">cell/mL</size>";
            yield return new WaitForSeconds(1.2f);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Sub.Priority = 9;
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            Sub.Priority = 11;
        }
    }

    public void SunShine() // 화면 밝기 + 빛의 양
    {
        float _la = LightAmount.value;
        CA.postExposure.value = _la * 3;
        DirectionalLight.intensity = _la;
        if (_la > 0.6f)
            SoundManager.Instance.SunBGM(2);
        else if (_la > 0.2f)
            SoundManager.Instance.SunBGM(1);
        else
            SoundManager.Instance.SunBGM(0);
    }

    public void Wind() // 바람 세기
    {
        float _wa = WindAmount.value;
        WindData.windMain = _wa - 0.1f;
        if (_wa > 0.8f)
            SoundManager.Instance.WindBGM(3);
        else if (_wa > 0.5f)
            SoundManager.Instance.WindBGM(2);
        else if (_wa > 0.2f)
            SoundManager.Instance.WindBGM(1);
        else
            SoundManager.Instance.WindBGM(0);
    }

    public void Rain()
    {
        float _ra = RainAmount.value;
        emission.rateOverTime = _ra;
        Rain_System.gravityModifier = _ra / 15;
        if (_ra > 200f)
            SoundManager.Instance.RainBGM(3);
        else if (_ra > 100f)
            SoundManager.Instance.RainBGM(2);
        else if (_ra > 1f)
            SoundManager.Instance.RainBGM(1);
        else
            SoundManager.Instance.RainBGM(0);
    }
}
