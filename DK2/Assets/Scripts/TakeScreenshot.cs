using UnityEngine;
using System.Collections;
using System.IO;

public class TakeScreenshot : MonoBehaviour {


    public int resWidth = 1920; public int resHeight = 1080;

    [Range(1, 6)]
    public int enlarge = 1;

    public bool transparent = false;

    public KeyCode screenshotKey = KeyCode.F8;
    private bool takeHiResShot = false;
    private TextureFormat transp = TextureFormat.ARGB32;
    private TextureFormat nonTransp = TextureFormat.RGB24;


    public static string ScreenShotName(int width, int height)
    {
        return string.Format("{0}/../screenshots/screen_{1}x{2}_{3}.png",
                             Application.dataPath,
                             width, height,
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    public void TakeHiResShot()
    {
        takeHiResShot = true;
    }


    void LateUpdate()
    {
        takeHiResShot |= Input.GetKeyDown(KeyCode.F8);
        if (takeHiResShot)
        {
#if !UNITY_4_3
#if !UNITY_WEBPLAYER
            TextureFormat textForm = nonTransp;
            if (transparent)
                textForm = transp;
            RenderTexture rt = new RenderTexture(resWidth * enlarge, resHeight * enlarge, 24);
            GetComponent<Camera>().targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidth * enlarge, resHeight * enlarge, textForm, false);
            GetComponent<Camera>().Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth * enlarge, resHeight * enlarge), 0, 0);
            GetComponent<Camera>().targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName(resWidth * enlarge, resHeight * enlarge);
            if (Directory.Exists(Application.dataPath + "/../screenshots/") == false)
            {
                Directory.CreateDirectory(Application.dataPath + "/../screenshots/");
            }
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", filename));
#endif
#endif

            takeHiResShot = false;
        }
    }

}
