w32tm /config /update
@ECHO off
chcp 1251
::chcp 65001
title Lab_2
cls

SET cmdName=lab2.cmd
SET file=labLog.log
SET directory=D:\ZSTU\SNP\lab2
SET nameKillProcess=mspaint.exe
SET arhiveFolder=D:\ZSTU\SNP\lab2\archives
SET computerIP=192.168.0.15
SET /a fileSize=500

ECHO.
ECHO ------------------- LOG START ------------------- >> %file%

IF EXIST %directory%\%file% (
	ECHO.
    ECHO file: %file% find
	ECHO LOG [%date% %time%] - File with name [%file%] was opened>> %file%
)
IF NOT EXIST %directory%\%file% (
	ECHO.
	ECHO file: %file% NOT find
	ECHO LOG [%date% %time%] - File with name [%file%] was created> %file%
	ECHO File was created
)


ECHO.
ECHO.
ECHO ----------------- Time -----------------
ECHO. >> %file%
ECHO ----------------- Time ----------------- >> %file%
ECHO.
ECHO Local date and time: %date% %time%

net time \\DESKTOP-C9I6VLD

ECHO.

SET testTime=14:40:00
time 14:40:00
IF errorlevel 1 (
	ECHO Time to [14:40:00] NOT edit;
	ECHO Time to [14:40:00] NOT edit; >> %file%
) else (
	ECHO Time to [14:40:00] edit;
	ECHO Time to [14:40:00] edit; >> %file%
)


ECHO.
ECHO Synchronizing time...
ECHO.
w32tm /resync
ECHO.
w32tm /stripchart /computer:ntp.time.in.ua /samples:1

ECHO.
ECHO.
ECHO ------------- Process -------------
ECHO.
::tasklist

::IF EXIST %directory%\%file% (
::	ECHO.
::	tasklist >> %file%
::)

taskkill /IM %nameKillProcess%
ECHO.
ECHO Search %nameKillProcess% ...
IF errorlevel 1 goto NoRecord
goto Done
ECHO Result: Process %nameKillProcess% was found
:NoRecord
ECHO Result: Process %nameKillProcess% was not found
:Done
tasklist | find "%nameKillProcess%"

ECHO.
ECHO.
ECHO ------------- Delete files -------------
ECHO. >> %file%
ECHO ------------- Delete files ------------- >> %file%
ECHO.

::erase %directory%\*.tmp

SET /a num = 0
FOR %%i in (*.tmp) do ( 
	SET /a num += 1
	ECHO %%i
	ECHO %%i >> %file%
	del %%i
)
FOR %%i in (temp.*) do (
	SET /a num += 1
	ECHO %%i
	ECHO %%i >> %file%
	del %%i
)

ECHO Deleted [%num%] tmp files
ECHO LOG [%date% %time%] - Deleted [%num%] files >> %file%
ECHO. >> %file%

ECHO.
ECHO.
ECHO ---------------- Arhive ----------------
ECHO. >> %file%
ECHO ---------------- Arhive ---------------- >> %file%
ECHO.


ECHO RAR_[%date% %time%]
For /f "tokens=2-4 delims=/ " %%a in ('date /t') do (set mydate=%%c-%%a-%%b)
For /f "tokens=1-2 delims=/:" %%a in ('time /t') do (set mytime=%%a%%b)
IF NOT EXIST %arhiveFolder% (
	mkdir arhives
)
IF EXIST %arhiveFolder% (
	"C:\Program Files\7-Zip\7z.exe" u -mx1 %arhiveFolder%\%mydate%_%mytime%.7z *.*

	ECHO.

	FOR %%i in (*.*) do ECHO LOG [%date% %time%] - File was added to arhive with name %mydate%_%mytime%.7z
	FOR %%i in (*.*) do ECHO LOG [%date% %time%] - File was added to arhive with name %mydate%_%mytime%.7z >> %file%
)


::SET /a day=%DATE:~,2%-1
::SET lastDay=%day%%DATE:~2,8%


::FOR /r %%i in (*.7z) do (
::	IF [%lastDay%]==%%~ni ECHO LOG [%date% %time%] - Detected arhive for the past day
::	IF [%lastDay%]==%%~ni ECHO LOG [%date% %time%] - Detected arhive for the past day >> %file%
::)


ECHO. >> %file%
forfiles -p %arhiveFolder% -m *.7z -s -d -1 -c "cmd /c DEL @path" 
IF errorlevel 1 (
	ECHO LOG [%date% %time%] - Arhive for the past day was NOT deleted
	ECHO LOG [%date% %time%] - Arhive for the past day was NOT deleted >> %file%
) else (
	ECHO LOG [%date% %time%] - Arhive for the past day was deleted
	ECHO LOG [%date% %time%] - Arhive for the past day was deleted >> %file%
)
ECHO.


forfiles -p %arhiveFolder% -m *.7z -s -d -30 -c "cmd /c DEL @path"
IF errorlevel 1 (
	ECHO LOG [%date% %time%] - 30 day old file was NOT detected
	ECHO LOG [%date% %time%] - 30 day old file was NOT detected >> %file%
) else (
	ECHO LOG [%date% %time%] - 30 day old file was deleted
	ECHO LOG [%date% %time%] - 30 day old file was deleted >> %file%
)
ECHO.


ECHO.
ECHO.
ECHO --------------- Internet ---------------
ECHO. >> %file%
ECHO --------------- Internet --------------- >> %file%
ECHO.

ping google.com
IF errorlevel 1 (
	ECHO.
	ECHO LOG [%date% %time%] - NO internet connection
	ECHO LOG [%date% %time%] - NO internet connection >> %file%
) else (
	ECHO.
	ECHO LOG [%date% %time%] - Internet connection is available
	ECHO LOG [%date% %time%] - Internet connection is available >> %file%
)

ping %computerIP%
IF errorlevel 1 (
	ECHO.
	ECHO LOG [%date% %time%] - %computerIP% was not detected
	ECHO LOG [%date% %time%] - %computerIP% was not detected >> %file%
) else (
	ECHO.
	ECHO LOG [%date% %time%] - %computerIP% was detected and shutdown after 60s...
	ECHO LOG [%date% %time%] - %computerIP% was detected and shutdown after 60s >> %file%
)


For /F "Delims=" %%I In ('TYPE %directory%\ipon.txt') do SET ipon=%%~I
ECHO %ipon%


ping %ipon%
IF errorlevel 1 (
	ECHO.
	ECHO LOG [%date% %time%] - %ipon% from file was not detected
	ECHO LOG [%date% %time%] - %ipon% from file was not detected >> %file%
) else (
	ECHO.
	ECHO LOG [%date% %time%] - %ipon% from file was detected and shutdown after 60s...
	ECHO LOG [%date% %time%] - %ipon% from file was detected and shutdown after 60s >> %file%
)

arp -a >> %file%


ECHO.
ECHO.
ECHO -------------- Files size --------------
ECHO. >> %file%
ECHO -------------- Files size -------------- >> %file%
ECHO.


for %%i in (%directory%\%file%) do (set /a size=%%~Zi)
::echo %size%

IF NOT %fileSize%==%size% (
	ECHO LOG [%date% %time%] - %file% is larger than %fileSize% [%size% - %fileSize%]
	ECHO LOG [%date% %time%] - %file% is larger than %fileSize% [%size% - %fileSize%] >> %file%
)

systeminfo > systeminfo.txt

ECHO.
ECHO -------------------- LOG  END ------------------- >> %file%
ECHO.
ECHO. >> %file%
ECHO. >> %file%
SET /p var=End