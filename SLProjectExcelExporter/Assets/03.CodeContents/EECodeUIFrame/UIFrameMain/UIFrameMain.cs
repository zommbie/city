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

    private const string c_ImportExcelFileDirectory     = "RootImportFile";                 // ���� ���ϵ� �о�� ���
    private const string c_ExcelFileExtension           = "*.xlsx";
    private const string c_ConfigFileName               = "ExcelExporterWorkList.json";     // �ؽ� �� ��Ƴ��� ����

    private const string c_ExportJsonClientDirectory    = "RootExportJsonClient";           // Json ���� ��� ���(Ŭ��)
    private const string c_ExportJsonServerDirectory    = "RootExportJsonServer";           // Json ���� ��� ���(����)
    private const string c_ExportLoaderClientDirectory  = "RootExportLoaderClient";
    private const string c_ExportLoaderServerDirectory  = "RootExportLoaderServer";

    [SerializeField] private UIScrollExportContents ScrollExportContents;
    [SerializeField] private Dictionary<string, string> m_mapFileNameHash = new Dictionary<string, string>(); // Hash ������ ���� �˻�

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
    private void PrivUIFrameMainCheckConfigFile() // ���� üũ�ϴ� json ���� Ȯ��
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

    private void PrivUIFrameMainReadHashFileData() // ���� üũ
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

            if (m_mapFileNameHash.ContainsKey(strFileName)) // Hashüũ ���Ͽ��� ������ ����� ������ �ִٸ�
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
            else // ù �����̶� ������ ����� ������ ���ٸ�
            {
                m_mapFileNameHash.Add(strFileName, strNewFileHash);
                pListScrollItemData.Add(new SFileData(strFileName, true));
            }
        }
        PrivUIFrameMainSetScrollItemData(pListScrollItemData);
    }

    private void PrivUIFrameMainSetScrollItemData(List<SFileData> pListScrollItemData) // ��ũ�ѹ� ���빰 �����ۿ� Data ����
    {
        List<SFileData>.Enumerator itFileData = pListScrollItemData.GetEnumerator();
        while (itFileData.MoveNext())
        {
            ScrollExportContents.DoScrollExportContentsSetItemData(itFileData.Current.FileName, itFileData.Current.HasChanged);
        }
    }

    private void PrivUIFrameMainPreLoadAllImportFiles() // ProcessorMaster ��ũ��Ʈ�� Import����� ��� Excel������ PreLoad
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

    private void PrivUIFrameMainWriteHashFileData(List<string> pListExportFile) // Export ��ư Ŭ�� ���Ŀ� �ؽ� ���� �ֽ�ȭ
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
    public void HandleUIFrameMainExcelExport() // UI�� Export ��ư�� ���� ȣ��
    {
        try
        {
            PrivUIFrameMainPreLoadAllImportFiles();

            List<string> pListExportFile = ScrollExportContents.ExtractScrollExportContentsSelectedFileList();
            PrivUIFrameMainErrorCheck(m_pProcessorMaster.DoMasterExcelLoad(pListExportFile));   // UI���� ����ڰ� üũ�� ���� ���� List�� Master�� Load
            PrivUIFrameMainErrorCheck(m_pProcessorMaster.DoMasterExcelCompile());
            PrivUIFrameMainErrorCheck(m_pProcessorMaster.DoMasterExcelExport(c_ExportJsonClientDirectory, c_ExportJsonServerDirectory, c_ExportLoaderClientDirectory, c_ExportLoaderServerDirectory));

            PrivUIFrameMainWriteHashFileData(pListExportFile);         // Export ���� MD5 �ؽ� ���� ����
        }
        catch (Exception pError)
        {
            m_pErrorMessage.DoUIFrameErrorMessageSetErrorMessage(pError.Message);
            m_pErrorMessage.DoUIFrameErrorMessageSetErrorMessageEnd();
            m_pErrorMessage.DoUIFrameSelfShow();
            Debug.LogError(pError);
        }
    }

    public void HandleUIFrameMainNewOnlyScrollItemToggleOn()    // UI�� New Only ��ư�� �Ѹ� ȣ��
    {
        if (SelectAll.IsToggleOn() == true) SelectAll.DoButtonToggleOff();
        ScrollExportContents.DoScrollExportContentsShowNewOnly();
    }
    public void HandleUIFrameMainAllScrollItemToggleOn()        // UI�� Select All ��ư�� �Ѹ� ȣ��
    {
        if (NewOnly.IsToggleOn() == true) NewOnly.DoButtonToggleOff();
        ScrollExportContents.DoScrollExportContentsShowAllOn();
    }
    public void HandleUIFrameMainNewOnlyScrollItemToggleOff()   // UI�� New Only ��ư�� ���� ȣ��
    {
        ScrollExportContents.DoScrollExportContentsShowAllOff();
    }
    public void HandleUIFrameMainAllScrollItemToggleOff()       // UI�� Select All ��ư�� ���� ȣ��
    {
        ScrollExportContents.DoScrollExportContentsShowAllOff();
    }

    public void HandleUIFrameMainErrorLogPopupOn()
    {
        m_pErrorMessage.DoUIFrameSelfShow();
    }
}
