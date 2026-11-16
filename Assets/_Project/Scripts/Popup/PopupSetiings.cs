
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class PopupSetiings : PopupBase
{
    [Header("Sprites")]
    [SerializeField] private Sprite spriteOn;
    [SerializeField] private Sprite spriteOff;

    [Header("Buttons")]
    [SerializeField] private Button musicButton;
    [SerializeField] private Button soundButton;
    [SerializeField] private Button hapticButton;

    [Header("Texts")]
    [SerializeField] private TMP_Text musicText;
    [SerializeField] private TMP_Text soundText;
    [SerializeField] private TMP_Text hapticText;

    private Image musicImage;
    private Image soundImage;
    private Image hapticImage;

    protected  void Awake()
    {

        musicImage = musicButton.GetComponent<Image>();
        soundImage = soundButton.GetComponent<Image>();
        hapticImage = hapticButton.GetComponent<Image>();

        musicButton.onClick.AddListener(ToggleMusic);
        soundButton.onClick.AddListener(ToggleSound);
        hapticButton.onClick.AddListener(ToggleHaptic);


    }
    protected override void OnEnable()
    {
        base.OnEnable();

        GameManager.Instance.ChangeState(GameManager.GameState.Pause);
        UpdateUI();
        AudioManager.Instance.PauseAllSound();

        AudioManager.Instance.PlaySound("TapButton");
        //StartCoroutine(PauseSoundNextFrame());
    }

    private IEnumerator PauseSoundNextFrame()
    {
        yield return null;
        yield return null;
        AudioManager.Instance.PauseAllSound();
    }

    private void OnDestroy()
    {
        if (musicButton != null)
            musicButton.onClick.RemoveListener(ToggleMusic);

        if (soundButton != null)
            soundButton.onClick.RemoveListener(ToggleSound);

        if (hapticButton != null)
            hapticButton.onClick.RemoveListener(ToggleHaptic);

        GameManager.Instance.ChangeState(GameManager.GameState.Playing);
        AudioManager.Instance.ResumeAllSound();

    }

    private void ToggleMusic()
    {
        AudioManager.Instance.PlaySound("TapButton");

        AudioManager.Instance.SetMusicEnabled(!DataMusic.MusicEnabled);
        UpdateUI();
    }

    private void ToggleSound()
    {

        AudioManager.Instance.SetSoundEnabled(!DataMusic.SoundEnabled);
        UpdateUI();
        AudioManager.Instance.PlaySound("TapButton");

    }

    private void ToggleHaptic()
    {
        AudioManager.Instance.PlaySound("TapButton");

        DataMusic.HapticEnabled = !DataMusic.HapticEnabled;
        UpdateUI();
    }

    private void UpdateUI()
    {
        UpdateButton(musicImage, musicText, DataMusic.MusicEnabled);
        UpdateButton(soundImage, soundText, DataMusic.SoundEnabled);
        UpdateButton(hapticImage, hapticText, DataMusic.HapticEnabled);
    }

    private void UpdateButton(Image image, TMP_Text text, bool isOn)
    {
        if (image != null)
            image.sprite = isOn ? spriteOn : spriteOff;

        if (text != null)
            text.text = isOn ? "ON" : "OFF";
    }

    public override void DestroyPopup()
    {
        base.DestroyPopup();
        AudioManager.Instance.PlaySound("TapButton");

    }

}
