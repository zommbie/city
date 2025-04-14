using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEditor.PackageManager;
using UnityEngine;

public class UIFrameMain : CUIFrameWidgetBase
{
    [System.Serializable]
    public class SFileData
    {
        public string FileName;
        public bool HasChanged;
        public SFileData(string strFileName, bool bHasChanged)
        {
            this.FileName = strFileName;
            this.HasChanged = bHasChanged;
        }
    }

    private const string c_ImportExcelFileDirectory     = "RootImportFile";                 // 엑셀 파일들 읽어올 경로
    private const string c_ExcelFileExtension           = "*.xlsx";
    private const string c_ConfigFileName               = "ExcelExporterWorkList.json";     // 해시 값 모아놓은 파일

    private const string c_ExportJsonClientDirectory    = "RootExportJsonClient";           // Json 파일 출력 경로(클라)
    private const string c_ExportJsonServerDirectory    = "RootExportJsonServer";           // Json 파일 출력 경로(서버)
    private const string c_ExportLoaderClientDirectory  = "RootExportLoaderClient";
    private const string c_ExportLoaderServerDirectory  = "RootExportLoaderServer";

    [SerializeField] private UIScrollExportContents ScrollExportContents;
    [SerializeField] private Dictionary<string, string> m_mapFileNameHash = new Dictionary<string, string>(); // Hash 값으로 갱신 검사

    [Header("[Select Option]")]
    [SerializeField] UIToggleCheck NewOnly;
    [SerializeField] UIToggleCheck SelectAll;

    private EEBuildProcessorMaster  m_pProcessorMaster = new EEBuildProcessorMaster();
    private UIFrameErrorMessage m_pErrorMessage;
    //----------------------------------------------------------------------
    protected override void OnUIFrameInitializePost()
    {
        base.OnUIFrameInitializePost();
        m_pErrorMessage = UIManager.Instance.UIFind<UIFrameErrorMessage>();
        PrivUIFrameMainInitializePost();
    }

    //----------------------------------------------------------------------
    private void PrivUIFrameMainInitializePost()
    {
        try
        {
            PrivUIFrameMainErrorCheck(m_pProcessorMaster.DoMasterInitialize(c_ImportExcelFileDirectory));

            PrivUIFrameMainCheckConfigFile();

            PrivUIFrameMainReadHashFileData();

            PrivUIFrameMainPreLoadAllImportFiles();
        }
        catch(Exception pError)
        {
            m_pErrorMessage.DoUIFrameErrorMessageSetErrorMessage(pError.Message);
            m_pErrorMessage.DoUIFrameErrorMessageSetErrorMessageEnd();
            m_pErrorMessage.DoUIFrameSelfShow();
            Debug.LogError(pError);
        }
    }
    private void PrivUIFrameMainCheckConfigFile() // 갱신 체크하는 json 파일 확인
    {
        string strConfigFilePath = Path.Combine(c_ImportExcelFileDirectory, c_ConfigFileName);

        if (File.Exists(strConfigFilePath) == false)
        {
            using (StreamWriter pStreamWriter = File.CreateText(strConfigFilePath))
            {
                string strConfigFileData = JsonConvert.SerializeObject(m_mapFileNameHash);
                pStreamWriter.WriteLine(strConfigFileData);
            }
        }
        else
        {
            string strConfigFileData = File.ReadAllText(strConfigFilePath);
            m_mapFileNameHash = JsonConvert.DeserializeObject<Dictionary<string, string>>(strConfigFileData);
        }
    }

    private void PrivUIFrameMainReadHashFileData() // 갱신 체크
    {
        List<SFileData> pListScrollItemData = new List<SFileData>();
        DirectoryInfo pDirectoryInfo = new DirectoryInfo(c_ImportExcelFileDirectory);
        FileInfo[] aFileInfo = pDirectoryInfo.GetFiles(c_ExcelFileExtension);
        for (int i = 0; i < aFileInfo.Length; i++)
        {
            string strFileName = aFileInfo[i].Name;
            string strNewFileHash = string.Empty;
            using (FileStream pFileStream = aFileInfo[i].Open(FileMode.Open, FileAccess.Read))
            {
                using (MD5 pMD5 = MD5.Create())
                {
                    byte[] aHashByte = pMD5.ComputeHash(pFileStream);
                    strNewFileHash = EEStringUtility.ExtractHashByteToString(aHashByte);
                }
            }

            if (m_mapFileNameHash.ContainsKey(strFileName)) // Hash체크 파일에서 기존에 저장된 정보가 있다면
            {
                string strOldFileHash = m_mapFileNameHash[strFileName];

                if (strNewFileHash != strOldFileHash)
                {
                    m_mapFileNameHash[strFileName] = strNewFileHash;
                    pListScrollItemData.Add(new SFileData(strFileName, true));
                }
                else
                {
                    pListScrollItemData.Add(new SFileData(strFileName, false));
                }
            }
            else // 첫 실행이라서 기존에 저장된 정보가 없다면
            {
                m_mapFileNameHash.Add(strFileName, strNewFileHash);
                pListScrollItemData.Add(new SFileData(strFileName, true));
            }
        }
        PrivUIFrameMainSetScrollItemData(pListScrollItemData);
    }

    private void PrivUIFrameMainSetScrollItemData(List<SFileData> pListScrollItemData) // 스크롤바 내용물 아이템에 Data 전달
    {
        List<SFileData>.Enumerator itFileData = pListScrollItemData.GetEnumerator();
        while (itFileData.MoveNext())
        {
            ScrollExportContents.DoScrollExportContentsSetItemData(itFileData.Current.FileName, itFileData.Current.HasChanged);
        }
    }

    private void PrivUIFrameMainPreLoadAllImportFiles() // ProcessorMaster 스크립트에 Import경로의 모든 Excel파일을 PreLoad
    {
        List<string> pListExcelFilePath = new List<string>();
        DirectoryInfo pDirectoryInfo = new DirectoryInfo(c_ImportExcelFileDirectory);
        FileInfo[] aFileInfo = pDirectoryInfo.GetFiles(c_ExcelFileExtension);
        for (int i = 0; i < aFileInfo.Length; i++)
        {
            string strTempFileName = aFileInfo[i].Name;
            pListExcelFilePath.Add(strTempFileName);
        }
        PrivUIFrameMainErrorCheck(m_pProcessorMaster.DoMasterExcelPreLoad(pListExcelFilePath));
    }

    private void PrivUIFrameMainWriteHashFileData(List<string> pListExportFile) // Export 버튼 클릭 이후에 해시 파일 최신화
    {
        string strConfigFilePath = Path.Combine(c_ImportExcelFileDirectory, c_ConfigFileName);
        string strConfigFileData = File.ReadAllText(strConfigFilePath);
        Dictionary<string,string> mapCheckedFileNameHash = JsonConvert.DeserializeObject<Dictionary<string, string>>(strConfigFileData);

        for(int i = 0; i < pListExportFile.Count; i++)
        {
            mapCheckedFileNameHash[pListExportFile[i]] = new string(m_mapFileNameHash[pListExportFile[i]]);
        }

        using (StreamWriter pStreamWriter = File.CreateText(strConfigFilePath))
        {
            string strNewConfigData = JsonConvert.SerializeObject(mapCheckedFileNameHash, Formatting.Indented);
            pStreamWriter.WriteLine(strNewConfigData);
        }
    }

    private void PrivUIFrameMainErrorCheck(SErrorInfo rError)
    {
        if (rError.ErrorType != EEEErrorCategory.None)
        {
            throw new Exception(string.Format("[{0}][{1}] {2}", rError.ErrorType.ToString(), rError.TableKey, rError.ErrorMessage));
        }
    }

    private void PrivUIFrameMainErrorCheck(SErrorContainer rErrorContainer)
    {
        if (rErrorContainer.listError.Count > 0)
        {
            for (int i = 0; i < rErrorContainer.listError.Count; i++)
            {
                if (rErrorContainer.listError[i].ErrorType != EEEErrorCategory.None)
                {
                    string strErrorMessage = string.Format("[{0}][{1}] {2}", rErrorContainer.listError[i].ErrorType.ToString(), rErrorContainer.listError[i].TableKey, rErrorContainer.listError[i].ErrorMessage);
                    Debug.LogError(strErrorMessage);
                    m_pErrorMessage.DoUIFrameErrorMessageSetErrorMessage(strErrorMessage);
                }
            }
            m_pErrorMessage.DoUIFrameErrorMessageSetErrorMessageEnd();
            m_pErrorMessage.DoUIFrameSelfShow();
        }
    }

    //----------------------------------------------------------------------
    public void HandleUIFrameMainExcelExport() // UI의 Export 버튼을 통해 호출
    {
        try
        {
            PrivUIFrameMainPreLoadAllImportFiles();

            List<string> pListExportFile = ScrollExportContents.ExtractScrollExportContentsSelectedFileList();
            PrivUIFrameMainErrorCheck(m_pProcessorMaster.DoMasterExcelLoad(pListExportFile));   // UI에서 사용자가 체크한 엑셀 파일 List만 Master에 Load
            PrivUIFrameMainErrorCheck(m_pProcessorMaster.DoMasterExcelCompile());
            PrivUIFrameMainErrorCheck(m_pProcessorMaster.DoMasterExcelExport(c_ExportJsonClientDirectory, c_ExportJsonServerDirectory, c_ExportLoaderClientDirectory, c_ExportLoaderServerDirectory));

            PrivUIFrameMainWriteHashFileData(pListExportFile);         // Export 이후 MD5 해시 파일 갱신
        }
        catch (Exception pError)
        {
            m_pErrorMessage.DoUIFrameErrorMessageSetErrorMessage(pError.Message);
            m_pErrorMessage.DoUIFrameErrorMessageSetErrorMessageEnd();
            m_pErrorMessage.DoUIFrameSelfShow();
            Debug.LogError(pError);
        }
    }

    public void HandleUIFrameMainNewOnlyScrollItemToggleOn()    // UI의 New Only 버튼을 켜면 호출
    {
        if (SelectAll.IsToggleOn() == true) SelectAll.DoButtonToggleOff();
        ScrollExportContents.DoScrollExportContentsShowNewOnly();
    }
    public void HandleUIFrameMainAllScrollItemToggleOn()        // UI의 Select All 버튼을 켜면 호출
    {
        if (NewOnly.IsToggleOn() == true) NewOnly.DoButtonToggleOff();
        ScrollExportContents.DoScrollExportContentsShowAllOn();
    }
    public void HandleUIFrameMainNewOnlyScrollItemToggleOff()   // UI의 New Only 버튼을 끄면 호출
    {
        ScrollExportContents.DoScrollExportContentsShowAllOff();
    }
    public void HandleUIFrameMainAllScrollItemToggleOff()       // UI의 Select All 버튼을 끄면 호출
    {
        ScrollExportContents.DoScrollExportContentsShowAllOff();
    }

    public void HandleUIFrameMainErrorLogPopupOn()
    {
        m_pErrorMessage.DoUIFrameSelfShow();
    }
}
