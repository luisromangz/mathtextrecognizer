
EXTRA_DIST = 

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -debug -d:DEBUG
ASSEMBLY = ./bin/Debug/MathTextRecognizer.exe
COMPILE_TARGET = exe
PROJECT_REFERENCES =  \
	../MathTextLibrary/bin/Debug/MathTextLibrary.dll \
	../CustomGtkWidgets/bin/Debug/CustomGtkWidgets.dll
BUILD_DIR = ./bin/Debug
endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4
ASSEMBLY = ./bin/Release/MathTextRecognizer.exe
COMPILE_TARGET = exe
PROJECT_REFERENCES =  \
	../MathTextLibrary/bin/Release/MathTextLibrary.dll \
	../CustomGtkWidgets/bin/Release/CustomGtkWidgets.dll
BUILD_DIR = ./bin/Release
endif


BINARIES = \
	$(MATHTEXTRECOGNIZER)  
PROGRAMFILES = \
	$(MATHTEXTLIBRARY_DLL) \
	$(MATHTEXTLIBRARY_DLL_MDB) \
	$(CUSTOMGTKWIDGETS_DLL) \
	$(CUSTOMGTKWIDGETS_DLL_MDB)  

	
all: $(ASSEMBLY) $(BINARIES) $(PROGRAMFILES) 

FILES = \
	AssemblyInfo.cs \
	MTBNode.cs \
	MainWindow.cs \
	OutputDialog.cs 

DATA_FILES = 

RESOURCES = \
	mathtextrecognizer.glade 

EXTRAS = \
	./mathtextrecognizer.in 

REFERENCES =  \
	$(GTK_SHARP_20_LIBS) \
	$(GLADE_SHARP_20_LIBS) \
	-r:System

DLL_REFERENCES = 

MATHTEXTLIBRARY_DLL = $(BUILD_DIR)/MathTextLibrary.dll
$(MATHTEXTLIBRARY_DLL): ../MathTextLibrary/bin/Debug/MathTextLibrary.dll
	mkdir -p $(BUILD_DIR)
	cp '$<' '$@'

MATHTEXTLIBRARY_DLL_MDB = $(BUILD_DIR)/MathTextLibrary.dll.mdb
$(MATHTEXTLIBRARY_DLL_MDB): ../MathTextLibrary/bin/Debug/MathTextLibrary.dll.mdb
	mkdir -p $(BUILD_DIR)
	cp '$<' '$@'

CUSTOMGTKWIDGETS_DLL = $(BUILD_DIR)/CustomGtkWidgets.dll
$(CUSTOMGTKWIDGETS_DLL): ../CustomGtkWidgets/bin/Debug/CustomGtkWidgets.dll
	mkdir -p $(BUILD_DIR)
	cp '$<' '$@'

CUSTOMGTKWIDGETS_DLL_MDB = $(BUILD_DIR)/CustomGtkWidgets.dll.mdb
$(CUSTOMGTKWIDGETS_DLL_MDB): ../CustomGtkWidgets/bin/Debug/CustomGtkWidgets.dll.mdb
	mkdir -p $(BUILD_DIR)
	cp '$<' '$@'

MATHTEXTRECOGNIZER = $(BUILD_DIR)/mathtextrecognizer
$(MATHTEXTRECOGNIZER): mathtextrecognizer
	mkdir -p $(BUILD_DIR)
	cp '$<' '$@'



$(build_resx_resources) : %.resources: %.resx
	resgen2 '$<' '$@'

$(ASSEMBLY) $(ASSEMBLY).mdb: $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES)
	mkdir -p $(dir $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$@ -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)

include $(top_srcdir)/Makefile.include