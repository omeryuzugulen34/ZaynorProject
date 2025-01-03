using System.Threading;
using UnityEngine;
using System;
using System.Collections;
using TMPro;
using DG.Tweening;

public class LightingManager : MonoBehaviour
{
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;

    [SerializeField,Range(0,24)] private float TimeOfDay = 12f;

    public bool day;
    public bool night;

    public DateTime TimerStart;
    public DateTime TimerEnd;

    double timer;

    public int MinutesToTransition;
    public int SecondsToTransition;

   public GameObject TimerText;

void Awake()
{
    UpdateLighting(12/24f);
}

    private void Update(){
        if(Preset == null){
            return;
        }

        if(Input.GetKeyDown(KeyCode.G)){
            TimerStart = DateTime.Now;
            TimeSpan time = new TimeSpan(0,MinutesToTransition,SecondsToTransition);
            TimerEnd = TimerStart.Add(time);
            TimerText.SetActive(true);
            StartCoroutine(TransitionTime());

            float initialTime = TimeOfDay;
            float incrementValue = 12;
            DOTween.To(() => TimeOfDay, x => TimeOfDay = x, initialTime + incrementValue, 60f)
           .OnUpdate(() =>
           {
               TimeOfDay %= 24;
               UpdateLighting(TimeOfDay / 24f);
           })
           .OnComplete(() =>
           {
            if(day){
                day = false;
                night = true;
            }
            else{
                night = false;
                day = true;
            }
           });

        }
    }

    private void UpdateLighting(float timePercent){
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);

        if(DirectionalLight != null){
            DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);

            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent*360f)- 90f,170f,0));
        }
    }


    private void OnValidate()
    {
        if(DirectionalLight != null)
            return;

        if(RenderSettings.sun != null){
            DirectionalLight = RenderSettings.sun;
        }
        else{
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach(Light light in lights){
                if(light.type == LightType.Directional){
                    DirectionalLight = light;
                    return;
                }
            }

        }

    }

    private IEnumerator TransitionTime(){     
        while(DateTime.Now < TimerEnd){
            TimeSpan timeleft = TimerEnd - DateTime.Now;
            timer = timeleft.TotalSeconds;

            string text = "";
            if(timeleft.Minutes > 0){
                text += $"{timeleft.Minutes}m ";
            }
            if(timeleft.Seconds > 0){
                text += $"{timeleft.Seconds}s";
            }

            TimerText.GetComponent<TMP_Text>().text = text;
            yield return null;
        }
        timer = 0;
        
        TimerText.SetActive(false);
        
    }
}
