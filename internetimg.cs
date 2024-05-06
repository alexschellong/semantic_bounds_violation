using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using UnityEngine.Rendering.HighDefinition;


public class internetimg : MonoBehaviour
{

    public Texture textureinternet;
    public Material Material1;
    public GameObject Object;
    public float gOpacity;
    public float newOpacity;
    public float tChange;
    public float tRemain;
    public float currentTime;
    public bool moving;
    public bool changingImg;
    public Object[] textures;
    // Start is called before the first frame update)



    void Start()
    {
        textures = Resources.LoadAll("images", typeof(Texture));

        gOpacity = Random.Range(0.0F, 1.0F);
        newOpacity = Random.Range(0.0F, 1.0F);
        tChange = Random.Range(0.0F, 5.0F);
        tRemain = Random.Range(0, 5);
        currentTime = 0f;
        moving = true;
        changingImg = true;

        Object.GetComponent<DecalProjector>().fadeFactor = gOpacity;

        //yGetComponent<DecalProjector>().material.EnableKeyword("_EMISSION");
        //StartCoroutine(DownloadImage("https://loremflickr.com/1920/1080"));

    }




    void Update()
    {
        StartCoroutine(dosomething());
        StartCoroutine(changeImage());

    }

    IEnumerator dosomething()
    {
        if (moving == true)
        {
            if (currentTime <= tChange)
            {


                currentTime += Time.deltaTime;
                Object.GetComponent<DecalProjector>().fadeFactor = Mathf.Lerp(gOpacity, newOpacity, currentTime / tChange);
            }
            else
            {
                moving = false;
                Object.GetComponent<DecalProjector>().fadeFactor = newOpacity;
                gOpacity = newOpacity;
                newOpacity = Random.Range(0.0F, 1.0F);
                tChange = Random.Range(0.0F, 5.0F);
                yield return wait();
                currentTime = 0f;

            }
        }
    }
    IEnumerator wait()
    {

        yield return new WaitForSeconds(tRemain);
        tRemain = Random.Range(0, 5);
        moving = true;
    }

    IEnumerator wait2()
    {

        yield return new WaitForSecondsRealtime(1800);
        changingImg = true;
    }

    IEnumerator changeImage()
    {

        if (changingImg == true)
        {
            GetComponent<DecalProjector>().material.EnableKeyword("_EMISSION");

            textureinternet = (Texture)textures[Random.Range(0, textures.Length)];

            GetComponent<DecalProjector>().material.EnableKeyword("_EMISSION");
            //GetComponent<DecalProjector>().material.SetTexture("_BaseColorMap", textureinternet);
            Material1.SetTexture("_EmissiveColorMap", textureinternet);
            Object.GetComponent<DecalProjector>().material = Material1;
            changingImg = false;
            yield return wait2();
        }
    }





    IEnumerator DownloadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
            textureinternet = ((DownloadHandlerTexture)request.downloadHandler).texture;

        //GetComponent<DecalProjector>().material.EnableKeyword("_EMISSION");
        //GetComponent<DecalProjector>().material.SetTexture("_BaseColorMap", textureinternet);
        Material1.SetTexture("_EmissiveColorMap", textureinternet);
        Object.GetComponent<DecalProjector>().material = Material1;

    }

    // Update is called once per frame

}
