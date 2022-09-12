@echo off
if exist .\Documentation (rd /S /Q .\Documentation)
::call InstallFont.bat
if not defined %%1 
if %%1 == -u (Tools\Doxygen\doxygen.exe -w html NewHeader.htm NewFooter.htm Style.css && Tools\Doxygen\doxygen.exe -w latex NewHeader.tex NewFooter.tex NewStylesheet.sty)
Tools\Doxygen\doxygen.exe doxyfile.config
pushd .\Documentation\LaTeX\
call make.bat
call make.bat pdf
popd
start .\Documentation\html\index.html
if exist .\Documentation\LaTeX\refman.pdf (move .\Documentation\LaTeX\refman.pdf .\Documentation\Documentation.pdf && start .\Documentation\Documentation.pdf) else (echo Build failed: PDF output not found!)
del *.bak