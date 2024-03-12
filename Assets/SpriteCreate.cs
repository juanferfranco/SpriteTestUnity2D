// Create a Sprite at startup.
// Assign a Texture to the Sprite when the button is pressed.

using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SpriteCreate : MonoBehaviour
{

    // Width and height of the texture in pixels.
    public int pixWidth;
    public int pixHeight;

    // The origin of the sampled area in the plane.
    public float xOrg;
    public float yOrg;

    // The number of cycles of the basic noise pattern that are repeated
    // over the width and height of the texture.
    public float scale = 1.0F;

    private Texture2D tex;
    private Color[] pix;
    private Renderer rend;


    private Sprite mySprite;
    public SpriteRenderer sr;

    public GameObject textPrefab; // Asigna tu prefab de texto aqu�
    private GameObject textObject = null;
    private int counter = 0;

    public Camera mainCamera;


    void Awake()
    {
        sr.color = new Color(0.9f, 0.9f, 0.9f, 1.0f);
        transform.position = new Vector3(0f,0f, 0.0f);
    }

    void Start()
    {

        // Set up the texture and a Color array to hold pixels during processing.
        tex = new Texture2D(pixWidth, pixHeight);
        pix = new Color[tex.width * tex.height];
        CalcNoise();
        mySprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 30), "Add sprite"))
        {
            sr.sprite = mySprite;
        }
    }

    void CalcNoise()
    {
        // For each pixel in the texture...
        float y = 0.0F;

        while (y < tex.height)
        {
            float x = 0.0F;
            while (x < tex.width)
            {
                float xCoord = xOrg + x / tex.width * scale;
                float yCoord = yOrg + y / tex.height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                pix[(int)y * tex.width + (int)x] = new Color(sample, sample, sample);
                x++;
            }
            y++;
        }

        // Copy the pixel data to the texture and load it into the GPU.
        tex.SetPixels(pix);
        tex.Apply();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0; // Aseg�rate de que la posici�n Z no afecte la visualizaci�n

            if(sr != null)
            {
                // Ajusta mouseWorldPos.z para asegurarte de que est� en el mismo plano que el objeto de la textura
                mousePos.z = sr.transform.position.z;

            }
            if (sr.bounds.Contains(mousePos))
            {
                if (textObject != null)
                {
                    Destroy(textObject);
                }
                // Crea y posiciona el cuadro de texto

                // Ajusta estos valores seg�n la porci�n de la pantalla que desees que el canvas cubra
                float desiredHeight = mainCamera.orthographicSize * 2;
                float desiredWidth = desiredHeight * mainCamera.aspect;

                textObject = Instantiate(textPrefab, mousePos, Quaternion.identity);

                RectTransform canvasRect = textObject.GetComponent<RectTransform>();
                canvasRect.sizeDelta = new Vector2(desiredWidth, desiredHeight);
                canvasRect.transform.localScale = Vector3.one * 2f; // Ajusta este valor seg�n sea necesario

                //textObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                textObject.GetComponentInChildren<TextMeshProUGUI>().text = counter.ToString(); // Aseg�rate de ajustar esto si usas otro tipo de componente de texto
                counter++;
            }
        }
    }
}
