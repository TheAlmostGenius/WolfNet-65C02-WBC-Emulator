@echo off
if exist .\docs (rd /S /Q .\docs)
Tools\Doxygen\doxygen.exe doxyfile.config
pushd .\docs\LaTeX\
call make.bat pdf
popd
pushd .\docs\html_docs
::start index.html
::start index.hhp
popd
::if exist .\docs\LaTeX\refman.pdf (move .\docs\LaTeX\refman.pdf .\docs\docs.pdf && start .\docs\docs.pdf) else (echo Build failed: PDF output not found!)
::if exist .\docs\rtf\refman.rtf (move .\docs\rtf\refman.rtf .\docs\docs.rtf && start .\docs\docs.rtf) else (echo Build failed: RTF output not found!)
del *.bak