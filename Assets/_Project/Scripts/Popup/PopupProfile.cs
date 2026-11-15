
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupProfile : PopupBase
{
    [SerializeField] private TMP_InputField inputName;
    [SerializeField] private Button buttonSave;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private DataUserName dataUserName;

    private Coroutine messageCoroutine;
    private bool isSaving;
    [SerializeField] private Image avatar;
    [SerializeField] private List<Sprite> avatarThemes;
    protected override void OnEnable()
    {
        base.OnEnable();

        if (messageText != null)
            messageText.text = "";

        if (buttonSave != null)

        isSaving = false;

        if (FirebaseManager.Instance == null)
            return;

        FirebaseManager.Instance.GetPlayerName(playerName =>
        {
            if (this == null || inputName == null)
                return;

            if (!string.IsNullOrEmpty(playerName))
                inputName.text = playerName;
        });
    }

    protected override void Start()
    {
        base.Start();
        UpdateAvatar();
        if (buttonSave != null)
            buttonSave.onClick.AddListener(SavePlayerName);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (buttonSave != null)
            buttonSave.onClick.RemoveListener(SavePlayerName);

        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
            messageCoroutine = null;
        }
    }

    private void UpdateAvatar()
    {
        if (avatar == null || ThemeManager.Instance == null)
            return;

        int index = ThemeManager.Instance.GetSelectedTheme(ThemeCategory.Dog);

        if (index < 0 || index >= avatarThemes.Count)
            return;

        avatar.sprite = avatarThemes[index];
    }

    private void SavePlayerName()
    {
        if (isSaving || inputName == null)
            return;

        string newName = inputName.text.Trim();
        string oldName = FirebaseManager.Instance.PlayerName;

        if (string.IsNullOrEmpty(newName))
        {
            ShowMessage("Please enter a name!");
            return;
        }

        if (string.Equals(newName, oldName, System.StringComparison.Ordinal))
        {
            DestroyPopup();
            return;
        }

        isSaving = true;

        FirebaseManager.Instance.ChangePlayerName(newName, success =>
        {
            if (this == null)
                return;

            isSaving = false;

            if (!success)
            {
                ShowMessage("This name is already taken!");
                return;
            }

            if (dataUserName != null)
                dataUserName.SetPlayerName(newName);

            ShowMessage("Name changed successfully!");
        });
    }

    private void ShowMessage(string message)
    {
        if (messageText == null)
            return;

        if (messageCoroutine != null)
            StopCoroutine(messageCoroutine);

        messageText.text = message;
        messageCoroutine = StartCoroutine(HideMessageAfterDelay());
    }

    private IEnumerator HideMessageAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        if (messageText != null)
            messageText.text = "";

        messageCoroutine = null;
    }
}
