namespace MetaMetrics.Api
{
    public enum MetaMetricsMeasurementType
    {
        Unknown,
        #region Login/Out
        [MetaMetricsMetaKIS]
        [MetaMetricsUsage]
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "Login")]
        metakis__login,
        [MetaMetricsMetaKIS]
        [MetaMetricsUsage]
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "LoginBySession")]
        metakis__loginbysessionid,
        [MetaMetricsMetaKIS]
        [MetaMetricsUsage]
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "LoginPing")]
        metakis__loginping,
        [MetaMetricsMetaKIS]
        [MetaMetricsUsage]
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "Logout")]
        metakis__logout, 
        #endregion
        
        #region Aufrufe
        [MetaMetricsMetaKIS]
        [MetaMetricsUsage]
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-PatientCall")]
        metakis__webcontext,
        [MetaMetricsMetaKIS]
        [MetaMetricsUsage]
        [MetaMetricsItem]
        [MetaMetricsTitle(Title = "MetaKIS-PatientCallType")]
        metakis__webcontext__items,
        #endregion
        
        #region Import
        [MetaMetricsMetaKIS]
        [MetaMetricsImport]
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Import")]
        metakis__workcontext,
        [MetaMetricsMetaKIS]
        [MetaMetricsImport]
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Import-Records")]
        metakis__workcontextrecords,
        
        [MetaMetricsMetaKIS]
        [MetaMetricsImport]
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Import-Extended-Records")]
        metakis__patientdata,
        [MetaMetricsMetaKIS]
        [MetaMetricsImport]
        [MetaMetricsItem]
        [MetaMetricsTitle(Title = "MetaKIS-Import-Extended-DataType")]
        metakis__patientdata__items,
        
        
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-DB-Import-DB")]
        metakisdbbatch__runimport,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-DB-Import-DBS")]
        metakisdbservice__runimport,
        
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-DB-Import-Records-DB")]
        metakisdbbatch__runimportrecord,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-DB-Import-Records-DBS")]
        metakisdbservice__runimportrecord,

        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Batch-Import-Records-BA")]
        metakisbatch__writemetakisdb,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Batch-Import-Records-BAS")]
        metakisbatchservice__writemetakisdb ,
        
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Batch-Process-Records-BA")]
        metakisbatch__batchrecordsprocessed,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Batch-Process-Records-BAS")]
        metakisbatchservice__batchrecordsprocessed,
        
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Stammdaten-Import")]
        metakis__importdata,
        #endregion
        
        #region MetaTEXT
        [MetaMetricsMetaTEXT]
        [MetaMetricsImport]
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaTEXT-Parse-Documents")]
        metatext__parsedocument_counter,
        [MetaMetricsMetaTEXT]
        [MetaMetricsImport]
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaTEXT-Analyse-Documents")]
        metatext__analyzedocument_counter,
        [MetaMetricsMetaTEXT]
        [MetaMetricsImport]
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaTEXT-Group-Documents")]
        metatext__groupdocumentsbyfall_counter,
        #endregion
        
        #region WPF Actions
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Wpf-Start")]
        metakis__wpf_clientstarted,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Wpf-OpenDS")]
        metakis__wpf_opends,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Wpf-OpenInEK")]
        metakis__wpf_openinek,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Wpf-OpenMetaKIS")]
        metakis__wpf_openmetakis,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Wpf-MerkeInEK")]
        metakis__wpf_mergeinek,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Wpf-MerkeMetaKIS")]
        metakis__wpf_mergemetakis,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Wpf-SaveMetaKIS")]
        metakis__wpf_savemetakis,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Wpf-SaveFilteredMetaKIS")]
        metakis__wpf_savemetakisfilter,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Wpf-ExportExcel")]
        metakis__wpf_exportexcel,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Wpf-E123")]
        metakis__wpf_e123,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Wpf-Abteilungsplan")]
        metakis__wpf_abteilungsplan,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Wpf-Auswertung")]
        metakis__wpf_auswertung,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Wpf-RulesManager")]
        metakis__wpf_rulesmanager,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Wpf-Neu-berechnen")]
        metakis__wpf_regrouprecords,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Wpf-Profiles-Compute")]
        metakis__wpf_computemetakisprofile,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Wpf-MoveToYear")]
        metakis__wpf_movetoyear,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Wpf-CutByDate")]
        metakis__wpf_cutbydate,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Wpf-Rules-Publish")]
        metakis__wpf_publishruleset,

        #endregion
        
        #region SL Actions
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-SL-Client")]
        metakis__sl_clientstarted,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-SL-Patient")]
        metakis__sl_patientstarted,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-SL-Patient-NewContext")]
        metakis__sl_newcontext,
        #endregion
        
        
        #region DBView Actions
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-View-Liste")]
        metakis__getdbview,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-View-Kachel")]
        metakis__getdbtiles,
        #endregion
        
        #region Daily Update Actions
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-DB-RecomputeAnwesende-DB")]
        metakisdbbatch__recomputeanwesende,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-DB-RecomputeAnwesende-DBS")]
        metakisdbservice__recomputeanwesende ,
        #endregion
        
        #region Batch Action
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Batch-Source")]
        metakisbatch__batchsource,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Batch-Source-BAS")]
        metakisbatchservice__batchsource ,
        
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Batch-Target")]
        metakisbatch__batchtarget,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Batch-Target-BAS")]
        metakisbatchservice__batchtarget ,
        
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Batch-Job-BA")]
        metakisbatch__runbatchjob,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Batch-Job-BAS")]
        metakisbatch__runbatchjobservice,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Batch-Job-BAS")]
        metakisbatchservice__runbatchjobservice ,
        #endregion

        #region Error Counter
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Error")]
        metakis__errorcounter,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Error-DB")]
        metakisdbbatch__errorcounter,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Error-DBS")]
        metakisdbservice__errorcounter ,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Error-BA")]
        metakisbatch__errorcounter,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "MetaKIS-Error-BAS")]
        metakisbatchservice__errorcounter ,
        #endregion
        
        #region FallUpdater Counter
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Record-DB")]
        metakisdbbatch__updatevisitfromrecord,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Record-DBS")]
        metakisdbservice__updatevisitfromrecord,
        
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Kodierung")]
        metakis__fallupdatercoding,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Kodierung")]
        metakisdbbatch__fallupdatercoding = metakis__fallupdatercoding,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Kodierung")]
        metakisdbservice__fallupdatercoding = metakis__fallupdatercoding,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Kodierung")]
        metakisbatch__fallupdatercoding = metakis__fallupdatercoding,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Kodierung")]
        metakisbatchservice__fallupdatercoding = metakis__fallupdatercoding,
        
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Chronic")]
        metakis__fallupdaterchronic,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Chronic")]
        metakisdbbatch__fallupdaterchronic = metakis__fallupdaterchronic,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Chronic")]
        metakisdbservice__fallupdaterchronic = metakis__fallupdaterchronic,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Chronic")]
        metakisbatch__fallupdaterchronic = metakis__fallupdaterchronic,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Chronic")]
        metakisbatchservice__fallupdaterchronic = metakis__fallupdaterchronic,
        
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Entgelte")]
        metakis__fallupdaterentgelt,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Entgelte")]
        metakisdbbatch__fallupdaterentgelt = metakis__fallupdaterentgelt,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Entgelte")]
        metakisdbservice__fallupdaterentgelt = metakis__fallupdaterentgelt,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Entgelte")]
        metakisbatch__fallupdaterentgelt = metakis__fallupdaterentgelt,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Entgelte")]
        metakisbatchservice__fallupdaterentgelt = metakis__fallupdaterentgelt,
        
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Beatmung")]
        metakis__fallupdaterbeatmung,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Beatmung")]
        metakisdbbatch__fallupdaterbeatmung = metakis__fallupdaterbeatmung,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Beatmung")]
        metakisdbservice__fallupdaterbeatmung = metakis__fallupdaterbeatmung,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Beatmung")]
        metakisbatch__fallupdaterbeatmung = metakis__fallupdaterbeatmung,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Beatmung")]
        metakisbatchservice__fallupdaterbeatmung = metakis__fallupdaterbeatmung,

        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Labor")]
        metakis__fallupdaterlabor,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Labor")]
        metakisdbbatch__fallupdaterlabor = metakis__fallupdaterlabor,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Labor")]
        metakisdbservice__fallupdaterlabor = metakis__fallupdaterlabor,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Labor")]
        metakisbatch__fallupdaterlabor = metakis__fallupdaterlabor,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Labor")]
        metakisbatchservice__fallupdaterlabor = metakis__fallupdaterlabor,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Labor")]
        metakis__writemetakisdb = metakis__fallupdaterlabor,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Labor")]
        metakisbatchservice__writemetakisdblab = metakis__fallupdaterlabor,

        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Medikation")]
        metakis__fallupdatermedikation,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Medikation")]
        metakisdbbatch__fallupdatermedikation = metakis__fallupdatermedikation,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Medikation")]
        metakisdbservice__fallupdatermedikation = metakis__fallupdatermedikation,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Medikation")]
        metakisbatch__fallupdatermedikation = metakis__fallupdatermedikation,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-Medikation")]
        metakisbatchservice__fallupdatermedikation = metakis__fallupdatermedikation,

        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-VerlaufDoku")]
        metakis__fallupdaterverlaufdoku,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-VerlaufDoku")]
        metakisdbbatch__fallupdaterverlaufdoku = metakis__fallupdaterverlaufdoku,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-VerlaufDoku")]
        metakisdbservice__fallupdaterverlaufdoku = metakis__fallupdaterverlaufdoku,
        
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "FallUpdater-MDK")]
        metakis__fallupdatermdk,
        
        #endregion

        #region Health Calls
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "Request-Version")]
        metakis__requestversion,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "Request-Version-Ext")]
        metakis__requestversionext,
        [MetaMetricsCounter]
        [MetaMetricsTitle(Title = "Request-Version-Date")]
        metakis__requestversiondate,
        #endregion
        
        [MetaMetricsMetaKIS]
        [MetaMetricsImport]
        [MetaMetricsItem]
        [MetaMetricsTitle(Title = "RestImportSuccess")]
        metakis__workcontext__items,
        [MetaMetricsMetaKIS]
        [MetaMetricsImport]
        [MetaMetricsItem]
        [MetaMetricsTitle(Title = "RestImportRecordsSuccess")]
        metakis__workcontextrecords__items,
        
        Ignore,       
        metakis__profile = Ignore,
        metakis__patientdata_error = Ignore,
        metakis__metrics_call_count = Ignore,
        metatext__metrics_call_count = Ignore,
        
        
        

    }
}