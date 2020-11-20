#region Header

/*	============================================
 *	Author 			    	: strix
 *	Initial Creation Date 	: 2020-11-20
 *	Summary 		        : 
 *  Template 		        : New Behaviour For ReSharper
   ============================================ */

#endregion Header

using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public interface IUnityPrefsWrapper
{
    PlayerPrefsWindowEditor.ECurrentZone eZone { get; }
    int GetInt(string strKey);
    float GetFloat(string strKey);
    string GetString(string strKey);

    void SetInt(string strKey, int iValue);
    void SetFloat(string strKey, float fValue);
    void SetString(string strKey, string strValue);
    void Save();

    void DeleteKey(string strKey);
    void DeleteAll();

    PlayerPrefsWindowEditor.SaveDataList GetSaveDataList();
}

public class PlayerPrefsWrapper : IUnityPrefsWrapper
{
    public PlayerPrefsWindowEditor.ECurrentZone eZone => PlayerPrefsWindowEditor.ECurrentZone.PlayerPrefs;

    public int GetInt(string strKey) => PlayerPrefs.GetInt(strKey);
    public float GetFloat(string strKey) => PlayerPrefs.GetFloat(strKey);
    public string GetString(string strKey) => PlayerPrefs.GetString(strKey);

    public void SetInt(string strKey, int iValue) => PlayerPrefs.SetInt(strKey, iValue);
    public void SetFloat(string strKey, float fValue) => PlayerPrefs.SetFloat(strKey, fValue);
    public void SetString(string strKey, string strValue) => PlayerPrefs.SetString(strKey, strValue);

    public void Save() => PlayerPrefs.Save();
    public void DeleteKey(string strKey) => PlayerPrefs.DeleteKey(strKey);
    public void DeleteAll() => PlayerPrefs.DeleteAll();

    public PlayerPrefsWindowEditor.SaveDataList GetSaveDataList()
    {
        return PlayerPrefsWindowEditor.GetPlayerPrefSaveDataList();
    }
}

public class EditorPrefsWrapper : IUnityPrefsWrapper
{
    public PlayerPrefsWindowEditor.ECurrentZone eZone => PlayerPrefsWindowEditor.ECurrentZone.EditorPrefs;

    public int GetInt(string strKey) => EditorPrefs.GetInt(strKey);
    public float GetFloat(string strKey) => EditorPrefs.GetFloat(strKey);
    public string GetString(string strKey) => EditorPrefs.GetString(strKey);

    public void SetInt(string strKey, int iValue) => EditorPrefs.SetInt(strKey, iValue);
    public void SetFloat(string strKey, float fValue) => EditorPrefs.SetFloat(strKey, fValue);
    public void SetString(string strKey, string strValue) => EditorPrefs.SetString(strKey, strValue);

    public void Save() { }
    public void DeleteKey(string strKey) => EditorPrefs.DeleteKey(strKey);
    public void DeleteAll() => EditorPrefs.DeleteAll();


    public PlayerPrefsWindowEditor.SaveDataList GetSaveDataList()
    {
        return PlayerPrefsWindowEditor.GetPlayerPrefSaveDataList();
    }
}
