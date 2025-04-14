using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

public class EEBuildProcessorMaster 
{
    private const string c_ClientExportSuffix = "Client";
    private const string c_ServerExportSuffix = "Server";

    private EEExcelFileAssist				m_pFileAssist		= new EEExcelFileAssist();
	private EEExcelParsingSheetContainer	m_pSheetContainer	= new EEExcelParsingSheetContainer();
	private EEJsonExporter					m_pJsonExport		= new EEJsonExporter();
	private EEJsonLoadCodeGeneratorCSharp   m_pCodeGeneratorC   = new EEJsonLoadCodeGeneratorCSharp();

	//---------------------------------------------------------------------
	public SErrorInfo DoMasterInitialize(string strExcelDirectory)
	{
		SErrorInfo rError = new SErrorInfo();
		m_pFileAssist.DoFileAssistDirectorySpecify(strExcelDirectory, ref rError);
		return rError;
	}
	// 사전에 Hash를 비교해서 갱신된 파일만 입력해야 한다.
	public SErrorInfo DoMasterExcelLoad(List<string> pListExcelFileName) 
	{
		SErrorInfo rError = new SErrorInfo();
		PrivMasterExcelOpenTableData(pListExcelFileName, ref rError);
		return rError;  
	}

	public SErrorInfo DoMasterExcelPreLoad(List<string> pListExcelFileName, bool bReset = true) // 전달된 모든 파일에서  Enum이나 TextDialog등 공용 파일만 로드한다. 
	{
		SErrorInfo rError = new SErrorInfo();
		if (bReset)
		{
			m_pSheetContainer.DoSheetContainerReset();
		}

		PrivMasterExcelOpenShared(pListExcelFileName, ref rError);

		return rError;
	}

	public SErrorContainer DoMasterExcelCompile()
	{
		SErrorContainer rErrorContainer = new SErrorContainer();
		m_pSheetContainer.DoSheetContainerCompile(rErrorContainer);
		return rErrorContainer;
	}

	public SErrorInfo DoMasterExcelExport(string strExportPathJsonClient, string strExportPathJsonServer, string strExportPathLoaderClient, string strExportPathLoaderServer)
	{
        SErrorInfo rErrorInfo = new SErrorInfo();
		List<EEExcelParsingSheetTable> pListSheetTable = null;
		List<SJsonExportData> pListJsonExportData = new List<SJsonExportData>();
		m_pSheetContainer.ExtractContainerTableList(out pListSheetTable);

        rErrorInfo = PrivMasterJsonExport(pListSheetTable, ref pListJsonExportData, strExportPathJsonClient, strExportPathJsonServer);
		if (rErrorInfo.ErrorType != EEEErrorCategory.None) return rErrorInfo;
        rErrorInfo = PrivMasterJsonLoaderGenerate(pListJsonExportData, m_pSheetContainer, strExportPathLoaderClient, strExportPathLoaderServer);
        if (rErrorInfo.ErrorType != EEEErrorCategory.None) return rErrorInfo;

        return rErrorInfo;
	}

	//----------------------------------------------------------------------
	private SErrorInfo PrivMasterJsonExport(List<EEExcelParsingSheetTable> pListSheetTable, ref List<SJsonExportData> pListJsonExportData, string strExportPathJsonClient, string strExportPathJsonServer)
	{
        SErrorInfo rErrorInfo = new SErrorInfo();

        for (int i = 0; i < pListSheetTable.Count; i++)
		{
			SJsonExportData pJsonExportData = new SJsonExportData();
			pListJsonExportData.Add(pJsonExportData);

			pJsonExportData.DataName = pListSheetTable[i].GetParsingSheetName();
			pJsonExportData.DataJsonContentsClient = m_pJsonExport.DoJsonExport(pListSheetTable[i], EExportType.Client);
			pJsonExportData.DataJsonContentsServer = m_pJsonExport.DoJsonExport(pListSheetTable[i], EExportType.Server);

            rErrorInfo = m_pFileAssist.DoFileAssistCreateJsonFile(pJsonExportData.DataJsonContentsClient, strExportPathJsonClient, pJsonExportData.DataName + c_ClientExportSuffix);
			if (rErrorInfo.ErrorType != EEEErrorCategory.None) break;

            rErrorInfo = m_pFileAssist.DoFileAssistCreateJsonFile(pJsonExportData.DataJsonContentsServer, strExportPathJsonServer, pJsonExportData.DataName + c_ServerExportSuffix);
            if (rErrorInfo.ErrorType != EEEErrorCategory.None) break;
        }

		return rErrorInfo;
    }

	private SErrorInfo PrivMasterJsonLoaderGenerate(List<SJsonExportData> pListJsonExportData, EEExcelParsingSheetContainer pSheetContainer, string strExportPathLoaderClient, string strExportPathLoaderServer)
	{
        SErrorInfo rErrorInfo = new SErrorInfo();

        for (int i = 0; i < pListJsonExportData.Count; i++)         //Client, Server 둘 다 C#을 사용한다는 가정으로 구현 - 사용언어에 맞춰서 CodeGenerator, 파일 생성함수 변경해야함
        {
			string strLoaderCodeCSharpClient = m_pCodeGeneratorC.DoJsonLoadCodeGenerate(pListJsonExportData[i].DataJsonContentsClient, pSheetContainer, EExportType.Client);
			string strLoaderCodeCSharpServer = m_pCodeGeneratorC.DoJsonLoadCodeGenerate(pListJsonExportData[i].DataJsonContentsServer, pSheetContainer, EExportType.Server);

            rErrorInfo = m_pFileAssist.DoFileAssistCreateCSharpFile(strLoaderCodeCSharpClient, strExportPathLoaderClient, pListJsonExportData[i].DataName + c_ClientExportSuffix);
			if (rErrorInfo.ErrorType != EEEErrorCategory.None) break;

            rErrorInfo = m_pFileAssist.DoFileAssistCreateCSharpFile(strLoaderCodeCSharpServer, strExportPathLoaderServer, pListJsonExportData[i].DataName + c_ServerExportSuffix);
            if (rErrorInfo.ErrorType != EEEErrorCategory.None) break;
        }

		return rErrorInfo;
    }

	//------------------------------------------------------------------------------
	private List<DataTableCollection> PrivMasterExcelFileExtract(List<string> listExcelFileName, ref SErrorInfo rError)
	{
		List<DataTableCollection> pListExcelFileContents = new List<DataTableCollection>();
		for (int i = 0; i < listExcelFileName.Count; i++)
		{
			m_pFileAssist.DoFileAssistFileOpen(listExcelFileName[i], out DataTableCollection pExcelSheetList, ref rError);
            if (rError.ErrorType != EEEErrorCategory.None)
            {
				break;
            }
            pListExcelFileContents.Add(pExcelSheetList);
		}

		return pListExcelFileContents;
	}

	private void PrivMasterExcelOpenAll(List<string> pListExcelFileName, ref SErrorInfo rError)
	{	
		List<DataTableCollection> pListExcelFileContents = PrivMasterExcelFileExtract(pListExcelFileName, ref rError);
        if (rError.ErrorType != EEEErrorCategory.None) return;
        PrivMasterExcelOpenSheet(pListExcelFileContents, EEExcelParsingSheetContainer.EExcelSheetType.Enumeration, ref rError);
        if (rError.ErrorType != EEEErrorCategory.None) return;
        PrivMasterExcelOpenSheet(pListExcelFileContents, EEExcelParsingSheetContainer.EExcelSheetType.LocalizeText, ref rError);
        if (rError.ErrorType != EEEErrorCategory.None) return;
        PrivMasterExcelOpenSheet(pListExcelFileContents, EEExcelParsingSheetContainer.EExcelSheetType.TableData, ref rError);
    }


	private void PrivMasterExcelOpenShared(List<string> pListExcelFileName, ref SErrorInfo rError)
	{
		List<DataTableCollection> pListExcelFileContents = PrivMasterExcelFileExtract(pListExcelFileName, ref rError);
		if (rError.ErrorType != EEEErrorCategory.None) return;
		PrivMasterExcelOpenSheet(pListExcelFileContents, EEExcelParsingSheetContainer.EExcelSheetType.Enumeration, ref rError);
        if (rError.ErrorType != EEEErrorCategory.None) return;
        PrivMasterExcelOpenSheet(pListExcelFileContents, EEExcelParsingSheetContainer.EExcelSheetType.LocalizeText, ref rError);
    }

	private void PrivMasterExcelOpenTableData(List<string> pListExcelFileName, ref SErrorInfo rError)
	{
		List<DataTableCollection> pListExcelFileContents = PrivMasterExcelFileExtract(pListExcelFileName, ref rError);
        if (rError.ErrorType != EEEErrorCategory.None) return;
        PrivMasterExcelOpenSheet(pListExcelFileContents, EEExcelParsingSheetContainer.EExcelSheetType.TableData, ref rError);
    }
	//-----------------------------------------------------------------------
	private void PrivMasterExcelOpenSheet(List<DataTableCollection> pExcelFileContents, EEExcelParsingSheetContainer.EExcelSheetType eSheetType, ref SErrorInfo rError)
	{
		for (int i = 0; i < pExcelFileContents.Count; i++)
		{
			DataTableCollection pDataTableCollection = pExcelFileContents[i];
			m_pSheetContainer.DoSheetContainerLoad(pDataTableCollection, eSheetType, ref rError);

			if(rError.ErrorType != EEEErrorCategory.None)
			{
				break;
			}
		}
	}

}
