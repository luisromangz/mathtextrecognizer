@echo off
mono bin\MathTextLearner.exe
IF errorlevel 1 GOTO fail
GOTO end
:fail
echo No se encontro el ejecutable de Mono, incorporelo al PATH del sistema
pause
:end