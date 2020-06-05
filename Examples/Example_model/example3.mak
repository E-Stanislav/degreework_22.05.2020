# Microsoft Developer Studio Generated NMAKE File, Format Version 4.00
# ** DO NOT EDIT **

# TARGTYPE "Win32 (x86) Console Application" 0x0103

!IF "$(CFG)" == ""
CFG=example3 - Win32 Debug
!MESSAGE No configuration specified.  Defaulting to example3 - Win32 Debug.
!ENDIF 

!IF "$(CFG)" != "example3 - Win32 Release" && "$(CFG)" != "example3 - Win32 Debug"
!MESSAGE Invalid configuration "$(CFG)" specified.
!MESSAGE You can specify a configuration when running NMAKE on this makefile
!MESSAGE by defining the macro CFG on the command line.  For example:
!MESSAGE 
!MESSAGE NMAKE /f "example3.mak" CFG="example3 - Win32 Debug"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "example3 - Win32 Release" (based on "Win32 (x86) Console Application")
!MESSAGE "example3 - Win32 Debug" (based on "Win32 (x86) Console Application")
!MESSAGE 
!ERROR An invalid configuration is specified.
!ENDIF 

!IF "$(OS)" == "Windows_NT"
NULL=
!ELSE 
NULL=nul
!ENDIF 
################################################################################
# Begin Project
F90=fl32.exe
RSC=rc.exe

!IF  "$(CFG)" == "example3 - Win32 Release"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "Release"
# PROP BASE Intermediate_Dir "Release"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "Release"
# PROP Intermediate_Dir "Release"
# PROP Target_Dir ""
OUTDIR=.\Release
INTDIR=.\Release

ALL : "$(OUTDIR)\example3.exe"

CLEAN : 
	-@erase ".\Release\example3.exe"
	-@erase ".\Release\BOUND.obj"
	-@erase ".\Release\FORCE.obj"
	-@erase ".\Release\FINDNODD.obj"
	-@erase ".\Release\MAIN.obj"
	-@erase ".\Release\DATA.obj"
	-@erase ".\Release\DGRIDD.obj"
	-@erase ".\Release\GRIDDM.obj"
	-@erase ".\Release\REGULA~1.obj"
	-@erase ".\Release\FIND_M~1.obj"
	-@erase ".\Release\GET_STAR.obj"
	-@erase ".\Release\RENMDD.obj"
	-@erase ".\Release\GENRCM.obj"
	-@erase ".\Release\STSM.obj"
	-@erase ".\Release\DEGREE.obj"
	-@erase ".\Release\FNROOT.obj"
	-@erase ".\Release\RCM.obj"
	-@erase ".\Release\ROOTLS.obj"
	-@erase ".\Release\FNENDD.obj"
	-@erase ".\Release\PRNTDD.obj"
	-@erase ".\Release\FORMDD.obj"
	-@erase ".\Release\MGSDTR.obj"
	-@erase ".\Release\RCMSLV.obj"
	-@erase ".\Release\ELSLV.obj"
	-@erase ".\Release\ESFCT.obj"
	-@erase ".\Release\EUSLV.obj"
	-@erase ".\Release\STRDD.obj"
	-@erase ".\Release\genqmd.obj"
	-@erase ".\Release\qmdmrg.obj"
	-@erase ".\Release\qmdqt.obj"
	-@erase ".\Release\qmdrch.obj"
	-@erase ".\Release\qmdupd.obj"
	-@erase ".\Release\GENAU.obj"
	-@erase ".\Release\TOSPW.obj"
	-@erase ".\Release\GENKNG.obj"
	-@erase ".\Release\GENRZN.obj"
	-@erase ".\Release\SMBFCT.obj"
	-@erase ".\Release\MEMCNT.obj"

"$(OUTDIR)" :
    if not exist "$(OUTDIR)/$(NULL)" mkdir "$(OUTDIR)"

# ADD BASE F90 /Ox /I "Release/" /c /nologo
# ADD F90 /Ox /I "Release/" /c /nologo
F90_PROJ=/Ox /I "Release/" /c /nologo /Fo"Release/" 
F90_OBJS=.\Release/
# ADD BASE RSC /l 0x419 /d "NDEBUG"
# ADD RSC /l 0x419 /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
BSC32_FLAGS=/nologo /o"$(OUTDIR)/example3.bsc" 
BSC32_SBRS=
LINK32=link.exe
# ADD BASE LINK32 kernel32.lib /nologo /subsystem:console /machine:I386
# ADD LINK32 kernel32.lib /nologo /subsystem:console /machine:I386
LINK32_FLAGS=kernel32.lib /nologo /subsystem:console /incremental:no\
 /pdb:"$(OUTDIR)/example3.pdb" /machine:I386 /out:"$(OUTDIR)/example3.exe" 
LINK32_OBJS= \
	"$(INTDIR)/BOUND.obj" \
	"$(INTDIR)/FORCE.obj" \
	"$(INTDIR)/FINDNODD.obj" \
	"$(INTDIR)/MAIN.obj" \
	"$(INTDIR)/DATA.obj" \
	"$(INTDIR)/DGRIDD.obj" \
	"$(INTDIR)/GRIDDM.obj" \
	"$(INTDIR)/REGULA~1.obj" \
	"$(INTDIR)/FIND_M~1.obj" \
	"$(INTDIR)/GET_STAR.obj" \
	"$(INTDIR)/RENMDD.obj" \
	"$(INTDIR)/GENRCM.obj" \
	"$(INTDIR)/STSM.obj" \
	"$(INTDIR)/DEGREE.obj" \
	"$(INTDIR)/FNROOT.obj" \
	"$(INTDIR)/RCM.obj" \
	"$(INTDIR)/ROOTLS.obj" \
	"$(INTDIR)/FNENDD.obj" \
	"$(INTDIR)/PRNTDD.obj" \
	"$(INTDIR)/FORMDD.obj" \
	"$(INTDIR)/MGSDTR.obj" \
	"$(INTDIR)/RCMSLV.obj" \
	"$(INTDIR)/ELSLV.obj" \
	"$(INTDIR)/ESFCT.obj" \
	"$(INTDIR)/EUSLV.obj" \
	"$(INTDIR)/STRDD.obj" \
	"$(INTDIR)/genqmd.obj" \
	"$(INTDIR)/qmdmrg.obj" \
	"$(INTDIR)/qmdqt.obj" \
	"$(INTDIR)/qmdrch.obj" \
	"$(INTDIR)/qmdupd.obj" \
	"$(INTDIR)/GENAU.obj" \
	"$(INTDIR)/TOSPW.obj" \
	"$(INTDIR)/GENKNG.obj" \
	"$(INTDIR)/GENRZN.obj" \
	"$(INTDIR)/SMBFCT.obj" \
	"$(INTDIR)/MEMCNT.obj"

"$(OUTDIR)\example3.exe" : "$(OUTDIR)" $(DEF_FILE) $(LINK32_OBJS)
    $(LINK32) @<<
  $(LINK32_FLAGS) $(LINK32_OBJS)
<<

!ELSEIF  "$(CFG)" == "example3 - Win32 Debug"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "C:\Users\Imp\Desktop\EXAMPL~3\Obj"
# PROP BASE Intermediate_Dir "C:\Users\Imp\Desktop\EXAMPL~3\Obj"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "C:\Users\Imp\Desktop\EXAMPL~3\Obj"
# PROP Intermediate_Dir "C:\Users\Imp\Desktop\EXAMPL~3\Obj"
# PROP Target_Dir ""
OUTDIR=C:\Users\Imp\Desktop\EXAMPL~3\Obj
INTDIR=C:\Users\Imp\Desktop\EXAMPL~3\Obj

ALL : "$(OUTDIR)\example3.exe"

CLEAN : 
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\example3.exe"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\BOUND.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\FORCE.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\FINDNODD.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\MAIN.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\DATA.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\DGRIDD.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\GRIDDM.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\REGULA~1.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\FIND_M~1.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\GET_STAR.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\RENMDD.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\GENRCM.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\STSM.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\DEGREE.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\FNROOT.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\RCM.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\ROOTLS.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\FNENDD.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\PRNTDD.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\FORMDD.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\MGSDTR.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\RCMSLV.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\ELSLV.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\ESFCT.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\EUSLV.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\STRDD.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\genqmd.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\qmdmrg.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\qmdqt.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\qmdrch.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\qmdupd.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\GENAU.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\TOSPW.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\GENKNG.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\GENRZN.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\SMBFCT.obj"
	-@erase "C:\Users\Imp\Desktop\EXAMPL~3\Obj\MEMCNT.obj"

"$(OUTDIR)" :
    if not exist "$(OUTDIR)/$(NULL)" mkdir "$(OUTDIR)"

# ADD BASE F90 /Zi /I "C:\Users\Imp\Desktop\EXAMPL~3\Obj/" /c /nologo
# ADD F90 /Zi /I "C:\Users\Imp\Desktop\EXAMPL~3\Obj/" /c /nologo
F90_PROJ=/Zi /I "C:\Users\Imp\Desktop\EXAMPL~3\Obj/" /c /nologo /Fo"C:\Users\Imp\Desktop\EXAMPL~3\Obj/" /Fd"C:\Users\Imp\Desktop\EXAMPL~3\Obj\example3.pdb" 
F90_OBJS=C:\Users\Imp\Desktop\EXAMPL~3\Obj/
# ADD BASE RSC /l 0x419 /d "_DEBUG"
# ADD RSC /l 0x419 /d "_DEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
BSC32_FLAGS=/nologo /o"$(OUTDIR)/example3.bsc" 
BSC32_SBRS=
LINK32=link.exe
# ADD BASE LINK32 kernel32.lib /nologo /subsystem:console /debug /machine:I386
# ADD LINK32 kernel32.lib /nologo /subsystem:console /debug /machine:I386
LINK32_FLAGS=kernel32.lib /nologo /subsystem:console /incremental:yes\
 /pdb:"$(OUTDIR)/example3.pdb" /debug /machine:I386 /out:"$(OUTDIR)/example3.exe" 
LINK32_OBJS= \
	"$(INTDIR)\BOUND.obj" \
	"$(INTDIR)\FORCE.obj" \
	"$(INTDIR)\FINDNODD.obj" \
	"$(INTDIR)\MAIN.obj" \
	"$(INTDIR)\DATA.obj" \
	"$(INTDIR)\DGRIDD.obj" \
	"$(INTDIR)\GRIDDM.obj" \
	"$(INTDIR)\REGULA~1.obj" \
	"$(INTDIR)\FIND_M~1.obj" \
	"$(INTDIR)\GET_STAR.obj" \
	"$(INTDIR)\RENMDD.obj" \
	"$(INTDIR)\GENRCM.obj" \
	"$(INTDIR)\STSM.obj" \
	"$(INTDIR)\DEGREE.obj" \
	"$(INTDIR)\FNROOT.obj" \
	"$(INTDIR)\RCM.obj" \
	"$(INTDIR)\ROOTLS.obj" \
	"$(INTDIR)\FNENDD.obj" \
	"$(INTDIR)\PRNTDD.obj" \
	"$(INTDIR)\FORMDD.obj" \
	"$(INTDIR)\MGSDTR.obj" \
	"$(INTDIR)\RCMSLV.obj" \
	"$(INTDIR)\ELSLV.obj" \
	"$(INTDIR)\ESFCT.obj" \
	"$(INTDIR)\EUSLV.obj" \
	"$(INTDIR)\STRDD.obj" \
	"$(INTDIR)\genqmd.obj" \
	"$(INTDIR)\qmdmrg.obj" \
	"$(INTDIR)\qmdqt.obj" \
	"$(INTDIR)\qmdrch.obj" \
	"$(INTDIR)\qmdupd.obj" \
	"$(INTDIR)\GENAU.obj" \
	"$(INTDIR)\TOSPW.obj" \
	"$(INTDIR)\GENKNG.obj" \
	"$(INTDIR)\GENRZN.obj" \
	"$(INTDIR)\SMBFCT.obj" \
	"$(INTDIR)\MEMCNT.obj"

"$(OUTDIR)\example3.exe" : "$(OUTDIR)" $(DEF_FILE) $(LINK32_OBJS)
    $(LINK32) @<<
  $(LINK32_FLAGS) $(LINK32_OBJS)
<<

!ENDIF 

.for{$(F90_OBJS)}.obj:
   $(F90) $(F90_PROJ) $<  

.f{$(F90_OBJS)}.obj:
   $(F90) $(F90_PROJ) $<  

.f90{$(F90_OBJS)}.obj:
   $(F90) $(F90_PROJ) $<  

################################################################################
# Begin Target

# Name "example3 - Win32 Release"
# Name "example3 - Win32 Debug"

!IF  "$(CFG)" == "example3 - Win32 Release"

!ELSEIF  "$(CFG)" == "example3 - Win32 Debug"

!ENDIF 

################################################################################
# Begin Source File

SOURCE="C:\Users\Imp\Desktop\EXAMPL~3\BOUND.for"

"$(INTDIR)\BOUND.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="C:\Users\Imp\Desktop\EXAMPL~3\FORCE.for"

"$(INTDIR)\FORCE.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="C:\Users\Imp\Desktop\EXAMPL~3\FINDNODD.for"

"$(INTDIR)\FINDNODD.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\MAIN.for"

"$(INTDIR)\MAIN.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\DATA.for"

"$(INTDIR)\DATA.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\DGRIDD.for"

"$(INTDIR)\DGRIDD.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\GRIDDM.for"

"$(INTDIR)\GRIDDM.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\REGULA~1.FOR"

"$(INTDIR)\REGULA~1.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\FIND_M~1.FOR"

"$(INTDIR)\FIND_M~1.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\GET_STAR.for"

"$(INTDIR)\GET_STAR.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\RENMDD.for"

"$(INTDIR)\RENMDD.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\GENRCM.for"

"$(INTDIR)\GENRCM.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\STSM.for"

"$(INTDIR)\STSM.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\DEGREE.for"

"$(INTDIR)\DEGREE.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\FNROOT.for"

"$(INTDIR)\FNROOT.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\RCM.for"

"$(INTDIR)\RCM.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\ROOTLS.for"

"$(INTDIR)\ROOTLS.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\FNENDD.for"

"$(INTDIR)\FNENDD.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\PRNTDD.for"

"$(INTDIR)\PRNTDD.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\FORMDD.for"

"$(INTDIR)\FORMDD.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\MGSDTR.for"

"$(INTDIR)\MGSDTR.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\RCMSLV.for"

"$(INTDIR)\RCMSLV.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\ELSLV.for"

"$(INTDIR)\ELSLV.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\ESFCT.for"

"$(INTDIR)\ESFCT.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\EUSLV.for"

"$(INTDIR)\EUSLV.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\STRDD.for"

"$(INTDIR)\STRDD.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\genqmd.for"

"$(INTDIR)\genqmd.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\qmdmrg.for"

"$(INTDIR)\qmdmrg.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\qmdqt.for"

"$(INTDIR)\qmdqt.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\qmdrch.for"

"$(INTDIR)\qmdrch.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\qmdupd.for"

"$(INTDIR)\qmdupd.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\GENAU.for"

"$(INTDIR)\GENAU.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\TOSPW.for"

"$(INTDIR)\TOSPW.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\GENKNG.for"

"$(INTDIR)\GENKNG.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\GENRZN.for"

"$(INTDIR)\GENRZN.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\SMBFCT.for"

"$(INTDIR)\SMBFCT.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE="c:\PROGRA~1\mai\SIGMA6~1.1B\Source\SOURCE~1\MEMCNT.for"

"$(INTDIR)\MEMCNT.obj" : $(SOURCE) "$(INTDIR)"
   $(F90) $(F90_PROJ) $(SOURCE)


# End Source File
# End Target
# End Project
################################################################################
