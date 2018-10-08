//***************************************************************************
//*
//*                          Honeywell AG, Department PT
//*
//***************************************************************************
//*
//*	Date        : 03.08.2010
//*	Author      : O. Gebert (OGE)
//*	Filename    : MainFrame_FFT.sys
//*	Language    : Visual C++ for Windows 2005
//*	Description : 
//*
//***************************************************************************
//*
//*	Version		Date:			History:
//*	1.00		26.06.2009		Neu Erstellung (OGE)
//*
//***************************************************************************
[FFT_Template_Start]
Name	= "Installiert"
1		= "Model                    : %s (%s)"
2		= "Bootloader Versionsnummer: %s (%s)"
3		= "Model: %s"
4		= "Firmware Versionsnummer  : %s"
5		= "Bootloader Versionsnummer: %s"
6		= "Firmware Versionsnummer  : %s (%s)"
:3		= "Fehler beim ersten Kommunizieren zum DUT"
:4		= "Falsche Variante"
:5		= "Falsche Fimware Versionsnummer"
:6		= "Falsche Bootloader Versionsnummer"
:7		= "Fehler beim Beschreibendes Fertigungsdatums"
:8		= "Fehler beim Beschreibendes Fertigungslocation"

[FFT_Template_Test1]
Name	= "Power ON"
1		= "Seriennummer wird neuerstellt"
2		= "Seriennummer: %s"
:3		= "Fehler beim Erstellen der Seriennummer"
:4		= "Fehler beim Erstellen der Seriennummer (Überlauf)"
:5		= "Fehler beim Schreiben der Seriennummer zur ID-Datenbank"
:6		= "Fehler beim Lesen der ID-Datenbank"

[FFT_Template_Test2]
Name	= "DUT wird registriert"
1		= "Seriennummer wird neuerstellt"
2		= "Seriennummer: %s"
:3		= "Fehler beim Erstellen der Seriennummer"
:4		= "Fehler beim Erstellen der Seriennummer (Überlauf)"
:5		= "Fehler beim Schreiben der Seriennummer zur ID-Datenbank"
:6		= "Fehler beim Lesen der ID-Datenbank"

[FFT_Template_Finish]
Name	= "DUT wird registriert"
1		= "Seriennummer wird neuerstellt"
2		= "Seriennummer: %s"
:3		= "Fehler beim Erstellen der Seriennummer"
:4		= "Fehler beim Erstellen der Seriennummer (Überlauf)"
:5		= "Fehler beim Schreiben der Seriennummer zur ID-Datenbank"
:6		= "Fehler beim Lesen der ID-Datenbank"

[General]
1		= "--- {0} --- [ {1} ] ({2})" //strNameTestStep,this->m_iID.ToString(),strID_TestStep
2		= "--- O.K. ---"		
3		= "!!! Error: [{0}:{1}] !!!" //this.ID,strErrorCode
4		= "Der Testschritt {1} aus [{0}] ist nicht in [TestSteps] beschrieben" //strNameTestSequencing,strNameTestStep
5		= "Der Testschritt {1} aus [{0}] ist nicht in FFT-SW beschrieben" //strNameTestSequencing,strNameTestStep
6		= "Wollen Sie von der Variante {0} zu der Variante{1} wechseln" // strVariantOld, strVariant
7		= "Fehler: [{0}] - {1}"  //strErrorCode, strMsgError
8		= "(Weiter mit F10. Beenden mit F5 oder Esc)"
9		= "wird initialisiert ..."
10		= "Device Manager"
11		= "Testläufe gesamt: {0}   Testläufe fehlerhaft: {1}  Yield: {2} %" //this.CountTestRun, this.CountTestFail, dYield
12		= "Der Deckel ist offen. Machen Sie den Deckel zu um den Test weiterzumachen."
13		= "!!! Achtung !!!"
14		= "Testschrittzeit: {0} Sec ({1})"
15		= "Testzeit       : {0} Sec"
16		= "O.K."
17		= "Der FFT-Adapter ist falsch. Wollen Sie den anderen FFT-Adapter anschließen und weiterprobieren."
18		= "Keine Aktionen für diese Routine sind festgelegt"
:997	= "DB-Access error"
:998	= "System Error"
:999	= "Benutzerbedingt abgebrochen"
#999	= "Test canceled"
