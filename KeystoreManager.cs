#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class KeystoreManager : EditorWindow
{
    private const string KeystorePathKey = "KeystorePath";
    private const string KeystorePasswordKey = "KeystorePassword";
    private const string AliasNameKey = "AliasName";
    private const string AliasPasswordKey = "AliasPassword";
    private const string AutoFillKey = "AutoFill";

    private static string keystorePath;
    private static string keystorePassword;
    private static string aliasName;
    private static string aliasPassword;
    private static bool autoFill;

    static KeystoreManager()
    {
        LoadKeystoreInfo();
        EditorApplication.update += OnEditorUpdate;
    }

    [MenuItem("Tools/Keystore Manager")]
    public static void ShowWindow()
    {
        GetWindow(typeof(KeystoreManager), false, "Keystore Manager");
    }

    private void OnEnable()
    {
        LoadKeystoreInfo();
    }

    private void OnGUI()
    {
        GUILayout.Label("Keystore Manager", EditorStyles.boldLabel);

        keystorePath = EditorGUILayout.TextField("Keystore Path", keystorePath);
        keystorePassword = EditorGUILayout.PasswordField("Keystore Password", keystorePassword);
        aliasName = EditorGUILayout.TextField("Alias Name", aliasName);
        aliasPassword = EditorGUILayout.PasswordField("Alias Password", aliasPassword);
        autoFill = EditorGUILayout.Toggle("Auto Fill", autoFill);

        if (GUILayout.Button("Save"))
        {
            SaveKeystoreInfo();
            if (autoFill)
            {
                ApplyKeystoreInfoToPlayerSettings();
            }
        }
    }

    private static void LoadKeystoreInfo()
    {
        keystorePath = PlayerPrefs.GetString(KeystorePathKey, "");
        keystorePassword = PlayerPrefs.GetString(KeystorePasswordKey, "");
        aliasName = PlayerPrefs.GetString(AliasNameKey, "");
        aliasPassword = PlayerPrefs.GetString(AliasPasswordKey, "");
        autoFill = PlayerPrefs.GetInt(AutoFillKey, 0) == 1;
    }

    private static void SaveKeystoreInfo()
    {
        PlayerPrefs.SetString(KeystorePathKey, keystorePath);
        PlayerPrefs.SetString(KeystorePasswordKey, keystorePassword);
        PlayerPrefs.SetString(AliasNameKey, aliasName);
        PlayerPrefs.SetString(AliasPasswordKey, aliasPassword);
        PlayerPrefs.SetInt(AutoFillKey, autoFill ? 1 : 0);
        PlayerPrefs.Save();
    }

    private static void ApplyKeystoreInfoToPlayerSettings()
    {
        if (!string.IsNullOrEmpty(keystorePath) && !string.IsNullOrEmpty(keystorePassword))
        {
            PlayerSettings.Android.keystoreName = keystorePath;
            PlayerSettings.Android.keystorePass = keystorePassword;

            if (!string.IsNullOrEmpty(aliasName) && !string.IsNullOrEmpty(aliasPassword))
            {
                PlayerSettings.Android.keyaliasName = aliasName;
                PlayerSettings.Android.keyaliasPass = aliasPassword;
            }
            else
            {
                Debug.LogWarning("Alias name or password is empty. Using default values in Player Settings.");
            }
        }
        else
        {
            Debug.LogError("Keystore path or password is empty. Unable to apply to Player Settings.");
        }
    }

    private static void OnEditorUpdate()
    {
        if (autoFill)
        {
            ApplyKeystoreInfoToPlayerSettings();
            EditorApplication.update -= OnEditorUpdate;
        }
    }
}
#endif
