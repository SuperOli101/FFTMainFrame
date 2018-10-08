//MainFrame
Var. 0.63.0.0	29.09.2016	OGE
		1. Nach der Ändurungen 0.61 die Funktion this.Test.GetTestStep( lieferte kein richtiges Ergebnis.
		   Fehler in der Test.CreateTestSteps korrigiert.
		2. TestStep.GetResultTest lieferte nicht das, wie beschrieben. Fehler korrigiert.
		3. Flag -> CTest.StateOfPictureResult eingeführt
		4. Mit Überschreiben der public override void CMyTest::PrintFromFFT() und override void CFFTester::PrintFromFFT()
		   schlisst mant das Drucken ans Drucken Button und menü-Eintrag
Var. 0.62.0.0	03.05.2016	AST
		1. Delegate hinzugefügt (SetStartStopButtonLightHandler, Test.cs), hier kann eine Funktion angehängt werden die die Lampen der Start- und Stoptasten kontrolliert.
		2. Event hinuzgefügt (BeforeTestStart, Test.cs), wird ausgelöst bevor ein Test gestartet wird.
		3. Event hinuzgefügt (AfterTestEnded, Test.cs), wird ausgelöst nachdem ein Test ausgeführt wurde.
		4. Event hinuzgefügt (BeforeTestStep, Test.cs), wird ausgelöst bevor ein Testschritt ausgeführt wird.
		5. Event hinuzgefügt (AfterTestStep, Test.cs), wird ausgelöst nachdem ein Testschritt ausgeführt wurde.

Ver. 0.61.0.0	14.04.2016	AST
		1.	Neue Funktion, in der Sys-File können den Testschritten in Sequencing nun Parameter mitgegeben werden.
			Dazu können verschiedene Parameter anhand Variante und OsNummer angelegt werden
			Die Parameter sind über die TestStepParameter in den Testschritten abgerufen werden
Ver. 0.60.0.0	17.03.2016	AST
		1.	Neue Klasse CDataCounter
		2.	Für Klasse CDataCounter neue Sys-Einträge
Ver. 0.59.0.0	01.03.2016	OGE
		1.	this.Data.DB.SetConfigurationFFT wurde in die Try-Schleife gesetzt, sonst Absturzt ohne Fehlermeldung beim Fehler
		2.	measurment.MeasUnitName wird mit "_" belegt, wenn die vorher leer war
Ver. 0.58.0.0	25.02.2016	OGE
		1.	this.MeasUnitDictionary enthält jetzt nur die Einheiten in kleinen Buchstaben
		2.	CreateDutNumberInTab_DutNumberForCreateDB statt CreateDutNumberFromDB (CreateDutNumberFromDB nur zum Kompatibilität geblieben)
Ver. 0.57.0.0	30.11.2015	OGE
		1.	Passwort und Print aus Sys angeschlossen
		2.	Fehler beim Anlegen der Tabelle (Tab_PartProduction) beseitigt
			(Zuerst PartNumber anlegen dann Tab_PartProduction ausfüllen)
Ver. 0.56.0.0	17.11.2015	OGE
		1.	Neue Funktion in CTestStep::SearchDutNumberInTab_DutNumberForCreateDB
Ver. 0.55.0.0	30.06.2015	OGE
		1.	Schreibt den Assembly Name vom FFT in die Report Datei
		2.	ProductionSystemId, OrderId, Mode  und PartNumber werden in die tab_Measurment geschrieben
		3.	Neue Funktion CreateDutNumberFromDB um eine DUT-Nummer in tab_DutNumberForCreate zuerstellen
		4.	tab_PartProduction wurde an FFT angeschlosssen
Ver. 0.54.0.0	30.06.2015	OGE
		1.	WriteToScroll gibt die Anzahl der eingeführten Zeilen
Ver. 0.53.0.0	19.02.2015	OGE
		1.	PartProductionDB.cs und DbSql.cs wurde als eigenes Projekt gemacht
Ver. 0.52.0.0	18.07.2014	OGE
		1.	Erlaubt Sys und Msg vom beliebegem Verzeichniss laden mit Sys.DirectoryOfFFTSystem	= C:\Mein\Projekte\ScanAndStart //Ohne Eintrag vom DirectoryOfFFTSystem im Sys File wird in FFTSystem gesucht
			Ini muss extra in MyTest-Project geladen werden
		2.	Mit CTest.SetIconToFormMenuSelect lässt sich Icon im Formular CFormMenuSelect ändern
		3.	Erlaubt Übergabe vom zweiten Argument (FFTMode 0-Production;1-Experten;2-Engineering) im Exe-File
Ver. 0.51.0.0	03.07.2014	OGE
		1.	Erlaubt Funktionen (z.B void __DoMovement()) ohne Parameter für Menü aus dem FFT-Gerüst
Ver. 0.50.0.0	25.06.2014	OGE
		1.	Funktionen	tab_Software.GetSoftwareArray(string SoftwareCategory = null, string SoftwarePathContain = null) und 
						tab_Software.GetSoftwareArray(int[] SoftwareIdArray, int[] ProductionCategoryIdArray, string[] PartNumberArray, int FlagComparePartNumber = 0, string SoftwarePathContain = null)
			haben den neuen Parameter string SoftwarePathContain um die SW in DataEvaluation nach Pfad zu filtren
Ver. 0.49.0.0	03.06.2014	OGE
		1.	In CTest.Reset wird currentTestSequencingLIST auf null überprüft sonst Absturz
Ver. 0.48.0.0	04.02.2014	OGE
		1.	In CTest.Reset werden die Testschritts aus currentTestSequencingLIST auch geresetet, sonst die Testschritte, die mehrmals ausgeführt werden, werden nicht zurückgesetzt
Ver. 0.47.0.0	19.11.2013	OGE
		1.	tab_ProductionSystemNumber.CreateProductionSystemNumber Abfrage liefert in BIGINT um richtig sortieren zu können (SELECT CAST([Number] AS BIGINT) AS Number FROM tab_ProductionSystemNumber WHERE [NumberTypeId] = '{0}' ORDER BY [Number] DESC)
		2. Erlaubt Einlesen der OS-Nummerm aus externer Datei
Ver. 0.46.0.0	05.07.2013	OGE
		1.	SQL-Server darf beliebige Spracheneinstellungen haben ( builder.CurrentLanguage = "German";)
		2.	toolStripProgressBarTestTime wird rot, wenn ein Fehler aufgetretten ist (//Application.EnableVisualStyles(); -> auskommentiert)
Ver. 0.45.0.0	17.06.2013	OGE
		1.	Kleine Korrekture von der 0.44.0.0 Version
Ver. 0.44.0.0	12.06.2013	OGE
		1. DatenBank-Klasse wurde in CPartProductionDB und CPartProductionDBforFFT aufgeteilt
Ver. 0.43.0.0	20.03.2013	OGE
		1.	Msg-File hat jetzt [DescriptionMeasurement], wo datenbankfelder beschrieben werden können
Ver. 0.42.0.0	12.03.2013	OGE
		1.	Kleine anpassung von der Datenbankbeschreibung (Multiplier = -1-> entfernt unf LSL, USL auf null setzen wenn nicht benötigt)
Ver. 0.41.0.0	28.02.2013	OGE
		1.	Klasse tab_DutNumberForCreate  eingeführt. Für Erstellung der Seriennummer MACs
		2.	Msg-File akzeptiert \r\n um Msgs auf dem Bildschirm zu teilen.
Ver. 0.40.0.0	14.11.2012	OGE
		1.	Variantenwechsel mit Scanner ist über Menüeintrag und ComboBox erlaubt
Ver. 0.39.0.0	23.10.2012	OGE
		1.	Application.Exit(); in static int Main(string[] args) eingeführt, wenn Error beim Geräteninit. entsteht.
Ver. 0.38.0.0	09.10.2012	OGE
		1.	CPartProductionDB Klasse jetzt in MainFrame Projekt
		2. Assembly WriteToSQL.exe wird jetzt nicht benötigt
Ver. 0.37.0.0	18.09.2012	OGE
		1.	LSL kann in die Datenbanl als null eingetragen werden
Ver. 0.36.0.0	07.08.2012	OGE
		1.	FFT-Mode Farbe hat sich nicht geändert, wenn mann Variantenwechsel, ohne Formularschließen gemacht wurde. 
Ver. 0.34.0.0	01.08.2012	OGE
		1.	Erste Version in der Fertigung
		

