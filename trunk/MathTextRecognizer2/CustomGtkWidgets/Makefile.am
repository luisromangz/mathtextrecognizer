
EXTRA_DIST = 

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -debug -d:DEBUG
ASSEMBLY = ./bin/Debug/CustomGtkWidgets.dll
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = ./bin/Debug
endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4
ASSEMBLY = ./bin/Release/CustomGtkWidgets.dll
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = ./bin/Release
endif


LINUX_PKGCONFIG = \
	$(CUSTOMGTKWIDGETS_PC)  

	
all: $(ASSEMBLY) $(LINUX_PKGCONFIG) 

FILES = \
	AssemblyInfo.cs \
	HandWriting/RasterHandWritingArea.cs \
	HandWriting/VectorHandWritingArea.cs \
	HandWriting/IHandWritingArea.cs \
	Logger/LogView.cs \
	Logger/LogSaveDialog.cs \
	ImageArea/ImageArea.cs \
	ImageArea/ImageAreaMode.cs \
	CommonDialogs/DatabaseOpenDialog.cs \
	CommonDialogs/AppInfoDialog.cs \
	CommonDialogs/ConfirmDialog.cs \
	CommonDialogs/OkDialog.cs \
	CommonDialogs/ImageLoadDialog.cs \
	CommonDialogs/DatabaseSaveDialog.cs \
	CommonDialogs/FolderOpenDialog.cs \
	CommonDialogs/PanelAssistant.cs \
	CommonDialogs/PanelAssistantStep.cs \
	ImageResources.cs 

DATA_FILES = 

RESOURCES = \
	gui.glade \
	../Common/Artwork/insert-image16.png \
	../Common/Artwork/insert-image22.png 

EXTRAS = \
	./customgtkwidgets.pc.in 

REFERENCES =  \
	$(GTK_SHARP_20_LIBS) \
	$(GLADE_SHARP_20_LIBS) \
	$(GTK_DOTNET_20_LIBS) \
	-r:System.Data \
	-r:System \
	-r:System.Drawing

DLL_REFERENCES = 

CUSTOMGTKWIDGETS_PC = $(BUILD_DIR)/customgtkwidgets.pc
$(CUSTOMGTKWIDGETS_PC): customgtkwidgets.pc
	mkdir -p $(BUILD_DIR)
	cp '$<' '$@'



$(build_resx_resources) : %.resources: %.resx
	resgen2 '$<' '$@'

$(ASSEMBLY) $(ASSEMBLY).mdb: $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES)
	mkdir -p $(dir $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$@ -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)

include $(top_srcdir)/Makefile.include