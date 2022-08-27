using UnityEngine;
using UnityEngine.UI;

public class TesseractDemoScript : MonoBehaviour
{
    [SerializeField] private Texture2D[] imagesToRecognize;
    [SerializeField] private Text displayText;
    [SerializeField] private RawImage outputImage;

    private TesseractDriver _tesseractDriver;
    private string _text = "";
    private Texture2D _texture;

    private int currImgIndex = 0;

    private void Start()
    {
        _tesseractDriver = new TesseractDriver();
        _tesseractDriver.Setup(OnSetupCompleteRecognize);
    }

    public void NextRecognition()
    {
        Texture2D imageToRecognize = imagesToRecognize[currImgIndex];

        _texture = new Texture2D(imageToRecognize.width, imageToRecognize.height, TextureFormat.ARGB32, false);
        _texture.SetPixels32(imageToRecognize.GetPixels32());
        _texture.Apply();

        ClearTextDisplay();

        AddToTextDisplay(_tesseractDriver.Recognize(_texture));
        AddToTextDisplay(_tesseractDriver.GetErrorMessage(), true);

        SetImageDisplay(_texture);
        
        currImgIndex++;
        if (currImgIndex > imagesToRecognize.Length - 1)
            currImgIndex = 0;
    }

    private void OnSetupCompleteRecognize()
    {
        ClearTextDisplay();
        AddToTextDisplay(_tesseractDriver.CheckTessVersion());
    }

    private void ClearTextDisplay()
    {
        _text = "";
    }

    private void AddToTextDisplay(string text, bool isError = false)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        _text += (string.IsNullOrWhiteSpace(displayText.text) ? "" : "\n") + text;

        if (isError)
            Debug.LogError(text);
        else
            Debug.Log(text);
    }

    private void LateUpdate()
    {
        displayText.text = _text;
    }

    private void SetImageDisplay(Texture2D texture)
    {
        RectTransform rectTransform = outputImage.GetComponent<RectTransform>();

        rectTransform.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            rectTransform.rect.width * texture.height / texture.width);
        outputImage.texture = texture;
    }
}