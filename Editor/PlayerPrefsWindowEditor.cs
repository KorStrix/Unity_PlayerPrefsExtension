#region Header
/*	============================================
 *	Author   			    : Strix
 *	Initial Creation Date 	: 2020-03-15
 *	Summary 		        : 
 *
 * PlayerPrefs의 값을 Editor에서 변경할 수 있는 툴입니다.
 * Editor가 설치된 환경이 Window일 때만 정상동작합니다.
 *  
 * 원본 코드 : https://forum.unity.com/threads/editor-utility-player-prefs-editor-edit-player-prefs-inside-the-unity-editor.370292/
 * 추가작업
 * EditorPrefs도 수정할 수 있게끔 수정
 * 이미 Save된 Prefs값 리스트를 항상 노출
 *
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using System;
using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using Microsoft.Win32;

/// <summary>
/// 
/// </summary>
public class PlayerPrefsWindowEditor : EditorWindow
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum EFieldType
    {
        String,
        Integer,
        Float
    }

    public enum ECurrentZone
    {
        PlayerPrefs,
        EditorPrefs,
    }

    public class SaveDataList
    {
        public static SaveDataList Dummy => new SaveDataList(new List<SaveData>());

        public IReadOnlyList<SaveData> listSaveData { get; private set; }

        public SaveDataList(List<SaveData> listSaveData)
        {
            this.listSaveData = listSaveData;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

    public class SaveData
    {
        public string strKey { get; private set; }
        public EFieldType eFieldType { get; private set; }

        public int iValue { get; private set; }
        public float fValue { get; private set; }
        public string strValue { get; private set; }

        public object pValue_Origin { get; private set; }

        public SaveData(string strKey, int iValue, object pValue_Origin)
        {
            this.strKey = strKey; this.pValue_Origin = pValue_Origin;

            eFieldType = EFieldType.Integer;
            this.iValue = iValue;
        }

        public SaveData(string strKey, float fValue, object pValue_Origin)
        {
            this.strKey = strKey; this.pValue_Origin = pValue_Origin;

            eFieldType = EFieldType.Float;
            this.fValue = fValue;
        }

        public SaveData(string strKey, string strValue, object pValue_Origin)
        {
            this.strKey = strKey; this.pValue_Origin = pValue_Origin;

            eFieldType = EFieldType.String;
            this.strValue = strValue.Replace("\0", "");
        }
    }

    /* public - Field declaration               */


    /* protected & private - Field declaration  */

    private EFieldType _eFieldType = EFieldType.String;
    private ECurrentZone _eCurrentZone = ECurrentZone.PlayerPrefs;
    private IUnityPrefsWrapper _pWrapper;

    private string _strKey = "";
    private string _strSetValue = "";
    private string _strGetValue = "";

    private string _strError;
    private string _strLog;


    // ========================================================================== //

    /* public - [Do~Something] Function 	        */

    public static SaveDataList GetPlayerPrefSaveDataList()
    {
        var listResult = new List<SaveData>();

        using (RegistryKey pHiveKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default))
        {
            using (RegistryKey pCurrentKey = pHiveKey.OpenSubKey(GetRegistryPath()))
            {
                if (pCurrentKey == null)
                    return SaveDataList.Dummy;

                string[] arrValueNames = pCurrentKey.GetValueNames();
                for (int i = 0; i < arrValueNames.Length; i++)
                {
                    string strValueName = arrValueNames[i];
                    object pValue = pCurrentKey.GetValue(strValueName);

                    if (pValue is int iValue)
                        listResult.Add(new SaveData(strValueName, iValue, iValue));

                    else if (pValue is float fValue)
                        listResult.Add(new SaveData(strValueName, fValue, fValue));

                    else if (pValue is byte[] arrBytes)
                        listResult.Add(new SaveData(strValueName, System.Text.Encoding.Default.GetString(arrBytes), arrBytes));
                }
            }
        }

        return new SaveDataList(listResult);
    }

    [MenuItem("Tools/Strix/Player & Editor Prefs Editor")]
    static void Init()
    {
        PlayerPrefsWindowEditor pWindow = (PlayerPrefsWindowEditor)GetWindow(typeof(PlayerPrefsWindowEditor), false);

        pWindow.minSize = new Vector2(600, 300);
        pWindow.Show();
    }

    // ========================================================================== //

    /* protected - [Override & Unity API]       */

    private void OnGUI()
    {
        GUIStyle pLabelStyle_Header = new GUIStyle();
        pLabelStyle_Header.normal.textColor = Color.green;

        EditorGUILayout.LabelField("Player & Editor Prefs Editor", pLabelStyle_Header);
        EditorGUILayout.LabelField("Author: RomejanicDev // Editor: Strix");
        EditorGUILayout.Separator();

        _eCurrentZone = (ECurrentZone)EditorGUILayout.EnumPopup("Current Zone", _eCurrentZone);
        if (_pWrapper == null || _pWrapper.eZone != _eCurrentZone)
        {
            switch (_eCurrentZone)
            {
                case ECurrentZone.PlayerPrefs: _pWrapper = new PlayerPrefsWrapper(); break;
                case ECurrentZone.EditorPrefs: _pWrapper = new EditorPrefsWrapper(); break;
                default:
                    return;
            }
        }

        EditorGUILayout.HelpBox(_pWrapper.GetSaveDataList().ToString(), MessageType.None);
        EditorGUILayout.Separator();


        _eFieldType = (EFieldType)EditorGUILayout.EnumPopup("Key Type", _eFieldType);
        _strKey = EditorGUILayout.TextField("Pref Key", _strKey);
        _strSetValue = EditorGUILayout.TextField("Pref Save Value", _strSetValue);
        EditorGUILayout.LabelField("Pref Get Value", _strGetValue);


        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button($"Set Key {GetCurrentZone()}"))
            {
                if (_eFieldType == EFieldType.Integer)
                {
                    if (int.TryParse(_strSetValue, out int iResult) == false)
                    {
                        _strError = "Invalid input \"" + _strSetValue + "\"";
                        return;
                    }
                    _pWrapper.SetInt(_strKey, iResult);
                    _strLog = $"Set {_strKey} - {iResult}";
                }
                else if (_eFieldType == EFieldType.Float)
                {
                    if (float.TryParse(_strSetValue, out float fResult) == false)
                    {
                        _strError = "Invalid input \"" + _strSetValue + "\"";
                        return;
                    }
                    _pWrapper.SetFloat(_strKey, fResult);
                    _strLog = $"Set {_strKey} - {fResult}";
                }
                else
                {
                    _pWrapper.SetString(_strKey, _strSetValue);
                    _strLog = $"Set {_strKey} - {_strSetValue}";
                }

                _pWrapper.Save();
                _strError = null;
            }

            if (GUILayout.Button($"Get Key {GetCurrentZone()}"))
            {
                if (_eFieldType == EFieldType.Integer)
                {
                    _strGetValue = _pWrapper.GetInt(_strKey).ToString();
                }
                else if (_eFieldType == EFieldType.Float)
                {
                    _strGetValue = _pWrapper.GetFloat(_strKey).ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    _strGetValue = _pWrapper.GetString(_strKey);
                }

                _strLog = $"Get {_strKey} - {_strGetValue}";
            }
        }
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button($"Delete Key {GetCurrentZone()}"))
            {
                _pWrapper.DeleteKey(_strKey);
                _pWrapper.Save();
            }

            if (GUILayout.Button($"Delete All Keys {GetCurrentZone()}"))
            {
                _pWrapper.DeleteAll();
                _pWrapper.Save();
            }
        }
        GUILayout.EndHorizontal();


        PrintLog();
    }


    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    private void PrintLog()
    {
        if (string.IsNullOrEmpty(_strError) == false)
            EditorGUILayout.HelpBox(_strError, MessageType.Error);

        if (string.IsNullOrEmpty(_strLog) == false)
            EditorGUILayout.HelpBox(_strLog, MessageType.None);
    }

    private string GetCurrentZone()
    {
        return $" '{_eCurrentZone}'";
    }

    static string GetRegistryPath()
    {
        return $"Software\\Unity\\UnityEditor\\{PlayerSettings.companyName}\\{PlayerSettings.productName}";
    }

    #endregion Private
}