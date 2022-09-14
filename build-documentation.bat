@echo off
if exist .\Documentation (rd /S /Q .\Documentation)
Tools\Doxygen\doxygen.exe doxyfile.config
pushd .\Documentation\LaTeX\
call make.bat pdf
popd
pushd .\Documentation\html
start index.html
start "C:\Program Files (x86)\HTML Help Workshop\hhw.exe" index.hhp
popd
if exist .\Documentation\LaTeX\refman.pdf (move .\Documentation\LaTeX\refman.pdf .\Documentation\Documentation.pdf && start .\Documentation\Documentation.pdf) else (echo Build failed: PDF output not found!)
if exist .\Documentation\rtf\refman.rtf (move .\Documentation\rtf\refman.rtf .\Documentation\Documentation.rtf && start .\Documentation\Documentation.rtf) else (echo Build failed: RTF output not found!)
del *.bak