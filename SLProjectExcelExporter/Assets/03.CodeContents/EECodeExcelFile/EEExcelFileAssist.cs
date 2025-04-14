using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ExcelDataReader;
using Newtonsoft.Json;


public class EEExcelFileAssist
{
    private string m_strDirectoryName = string.Empty;

    private const string c_JsonFileExtension = ".json";
    private const string c_ExcelFileExtension = ".xlsx";
    private const string c_CSharpFileExtension = ".cs";

    //-----------------------------------------------------------
    public void DoFileAssistDirectorySpecify(string strDirectory, ref SErrorInfo rError)
    {
        if (Directory.Exists(strDirectory) == false)
        {
            try
            {
                Directory.CreateDirectory(strDirectory);
            }
            catch (Exception pError)
            {
                rError.ErrorType = EEEErrorCategory.Directory;
                rError.ErrorMessage = "[FileOpen] " + pError.Message;
            }
        }
        m_strDirectoryName = strDirectory;
    }

    public void DoFileAssistFileOpen(string strFileName, out DataTableCollection pExcelPageList, ref SErrorInfo rError)
    {      
        pExcelPageList = null;

        if (m_strDirectoryName == string.Empty)
        {
            rError.ErrorType = EEEErrorCategory.FileOpen;
            rError.ErrorMessage = "[FileOpen]No Directory Specified. Call DoFileAssistDirectorySpecify";
        }
        else
        {
            string strFilePathName = m_strDirectoryName + Path.DirectorySeparatorChar + strFileName;
            if (File.Exists(strFilePathName))
            {
                using (FileStream pFileStream = File.Open(strFilePathName, FileMode.Open, FileAccess.Read))
                {
                    ExcelReaderConfiguration pExelConfig = new ExcelReaderConfiguration();
                    pExelConfig.FallbackEncoding = Encoding.UTF8;

                    IExcelDataReader pExcelReader = ExcelReaderFactory.CreateReader(pFileStream, pExelConfig);
                    pExcelPageList = pExcelReader.AsDataSet().Tables;

                    PrivExcelFileRemoveWhiteSpeaceAndConvertString(pExcelPageList);
                }
            }
            else
            {
                rError.ErrorType = EEEErrorCategory.FileOpen;
                rError.ErrorMessage = "[FileOpen] No Exist File";
            }
        }       
    }

    public SErrorInfo DoFileAssistCreateJsonFile(string strJsonData, string strFilePath, string strFileName)
    {
        SErrorInfo rError = new SErrorInfo();

        if (Directory.Exists(strFilePath) == false)
        {
            try
            {
                Directory.CreateDirectory(strFilePath);
            }
            catch
            {
                rError.ErrorType = EEEErrorCategory.Directory;
                rError.ErrorMessage = "[Directory] Failed to create export path";
            }
        }
        else
        {
            string strExportPath = Path.Combine(strFilePath, strFileName + c_JsonFileExtension);
            using (FileStream pFileStream = File.Open(strExportPath, FileMode.Create))
            {
                using (StreamWriter pStreamWriter = new StreamWriter(pFileStream))
                {
                    pStreamWriter.Write(strJsonData);
                }
            }
        }
        return rError;
    }

    public SErrorInfo DoFileAssistCreateCSharpFile(string strLoaderCodeCSharp, string strFilePath, string strFileName)
    {
        SErrorInfo rError = new SErrorInfo();

        if (Directory.Exists(strFilePath) == false)
        {
            try
            {
                Directory.CreateDirectory(strFilePath);
            }
            catch
            {
                rError.ErrorType = EEEErrorCategory.Directory;
                rError.ErrorMessage = "[Directory] Failed to create export path";
            }
        }
        else
        {
            string strExportPath = Path.Combine(strFilePath, strFileName + c_CSharpFileExtension);
            using (FileStream pFileStream = File.Open(strExportPath, FileMode.Create))
            {
                using (StreamWriter pStreamWriter = new StreamWriter(pFileStream))
                {
                    pStreamWriter.Write(strLoaderCodeCSharp);
                }
            }
        }
        return rError;
    }

    //--------------------------------------------------------------
    private void PrivExcelFileRemoveWhiteSpeaceAndConvertString(DataTableCollection pExcelPageList)
    {
        for (int i = 0; i < pExcelPageList.Count; i++)
        {
            DataRowCollection pRowDataCollection = pExcelPageList[i].Rows;
            for (int j = 0; j < pRowDataCollection.Count; j++)
            {
                DataRow pDataRow = pRowDataCollection[j];
                for (int h = 0; h < pDataRow.ItemArray.Length; h++)
                {
                    object pItem = pDataRow.ItemArray[h];
                    System.Type pType = pItem.GetType();
                    if (pType == typeof(string))
                    {
                        string strCell = pItem as string;
                        strCell = strCell.Trim();
                        pDataRow.SetField(h, strCell);
                    }
                }
            }
        }
    }
}
