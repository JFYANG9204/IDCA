mrScriptCL Library\log1.mrs /a:BATCH_FILE=".\RunTables.bat" 

@rem mrScriptCL Toplines.mrs >> MasterLog.txt
@rem 
@rem mrScriptCL SpecHelp.mrs >> MasterLog.txt
@rem 
@rem mrScriptCL Run.mrs /d:TOPBREAK="\"bk01\"" /d:OUTPUTNAME="\"tab-01WT\"" /d:SIGTEST1="\"True\""  /d:GLOBALFILTER="\"Comp={Completed}\"" /d:GLOBALLABEL="\"Total\"">> MasterLog.txt
@rem mrScriptCL Run.mrs /d:TOPBREAK="\"bk01\"" /d:OUTPUTNAME="\"tab-01NT\"" /d:SIGTEST1="\"False\"" /d:GLOBALFILTER="\"Comp={Completed}\"" /d:GLOBALLABEL="\"Total\"" >> MasterLog.txt
