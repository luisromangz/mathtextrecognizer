
EXTRA_DIST = 

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -debug -d:DEBUG
ASSEMBLY = ./bin/Debug/MathTextLearner.exe
COMPILE_TARGET = exe
PROJECT_REFERENCES =  \
	../CustomGtkWidgets/bin/Debug/CustomGtkWidgets.dll \
	../MathTextLibrary/bin/Debug/MathTextLibrary.dll
BUILD_DIR = ./bin/Debug
endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4
ASSEMBLY = ./bin/Release/MathTextLearner.exe
COMPILE_TARGET = exe
PROJECT_REFERENCES =  \
	../CustomGtkWidgets/bin/Release/CustomGtkWidgets.dll \
	../MathTextLibrary/bin/Release/MathTextLibrary.dll
BUILD_DIR = ./bin/Release
endif


BINARIES = \
	$(MATHTEXTLEARNER)  
PROGRAMFILES = \
	$(CUSTOMGTKWIDGETS_DLL) \
	$(CUSTOMGTKWIDGETS_DLL_MDB) \
	$(MATHTEXTLIBRARY_DLL) \
	$(MATHTEXTLIBRARY_DLL_MDB)  

	
all: $(ASSEMBLY) $(BINARIES) $(PROGRAMFILES) 

FILES = \
	AssemblyInfo.cs \
	MainWindow.cs \
	LearningFailedArgs.cs \
	Assistant/BitmapProcessesStep.cs \
	Assistant/DatabaseTypeStep.cs \
	Assistant/FileSelectionStep.cs \
	Assistant/NewDatabaseAssistant.cs \
	Assistant/BitmapProcessHelpers/BitmapProcessNode.cs \
	Assistant/BitmapProcessHelpers/ProcessEditorDialog.cs \
	Assistant/BitmapProcessHelpers/ProcessEditorWidget.cs \
	Assistant/BitmapProcessHelpers/ProcessSelectorDialog.cs 

DATA_FILES = 

RESOURCES = \
	mathtextlearner.glade \
	Assistant/databaseAssistant.glade 

EXTRAS = \
	./mathtextlearner.in 

REFERENCES =  \
	$(GLADE_SHARP_20_LIBS) \
	$(GTK_SHARP_20_LIBS) \
	-r:System.Drawing

DLL_REFERENCES = 

CUSTOMGTKWIDGETS_DLL = $(BUILD_DIR)/CustomGtkWidgets.dll
$(CUSTOMGTKWIDGETS_DLL): ../CustomGtkWidgets/bin/Debug/CustomGtkWidgets.dll
	mkdir -p $(BUILD_DIR)
	cp '$<' '$@'

CUSTOMGTKWIDGETS_DLL_MDB = $(BUILD_DIR)/CustomGtkWidgets.dll.mdb
$(CUSTOMGTKWIDGETS_DLL_MDB): ../CustomGtkWidgets/bin/Debug/CustomGtkWidgets.dll.mdb
	mkdir -p $(BUILD_DIR)
	cp '$<' '$@'

MATHTEXTLIBRARY_DLL = $(BUILD_DIR)/MathTextLibrary.dll
$(MATHTEXTLIBRARY_DLL): ../MathTextLibrary/bin/Debug/MathTextLibrary.dll
	mkdir -p $(BUILD_DIR)
	cp '$<' '$@'

MATHTEXTLIBRARY_DLL_MDB = $(BUILD_DIR)/MathTextLibrary.dll.mdb
$(MATHTEXTLIBRARY_DLL_MDB): ../MathTextLibrary/bin/Debug/MathTextLibrary.dll.mdb
	mkdir -p $(BUILD_DIR)
	cp '$<' '$@'

MATHTEXTLEARNER = $(BUILD_DIR)/mathtextlearner
$(MATHTEXTLEARNER): mathtextlearner
	mkdir -p $(BUILD_DIR)
	cp '$<' '$@'



$(build_resx_resources) : %.resources: %.resx
	resgen2 '$<' '$@'

$(ASSEMBLY) $(ASSEMBLY).mdb: $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES)
	mkdir -p $(dir $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$@ -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)

include $(top_srcdir)/Makefile.include