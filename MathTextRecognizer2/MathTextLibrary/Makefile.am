
EXTRA_DIST = 

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -unsafe -warn:4 -debug -d:DEBUG
ASSEMBLY = ./bin/Debug/MathTextLibrary.dll
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = ./bin/Debug
endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -unsafe -warn:4
ASSEMBLY = ./bin/Release/MathTextLibrary.dll
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = ./bin/Release
endif


LINUX_PKGCONFIG = \
	$(MATHTEXTLIBRARY_PC)  

	
all: $(ASSEMBLY) $(LINUX_PKGCONFIG) 

FILES = \
	AssemblyInfo.cs \
	MathSymbol.cs \
	MathTextBitmap.cs \
	MathTextBitmapEvents.cs \
	BitmapProcesses/BitmapFixedThresholder.cs \
	BitmapProcesses/BitmapFramer.cs \
	BitmapProcesses/BitmapScaler.cs \
	BitmapProcesses/BitmapStentifordThinner.cs \
	BitmapProcesses/BitmapTwoPeaksThresholder.cs \
	BitmapProcesses/BitmapZhangSuenHoltThinner.cs \
	BitmapProcesses/BitmapZhangSuenStentifordHoltThinner.cs \
	BitmapSegmenters/AllHolesProjectionBitmapSegmenter.cs \
	BitmapSegmenters/DifferentialProjectionBitmapSegmenter.cs \
	BitmapSegmenters/FractionVerticalBitmapSegmenter.cs \
	BitmapSegmenters/IBitmapSegmenter.cs \
	BitmapSegmenters/MostFrequentHoleProjectionBitmapSegmenter.cs \
	BitmapSegmenters/ProjectionBitmapSegmenter.cs \
	BitmapSegmenters/TwoHolesBitmapSegmenter.cs \
	Controllers/ControllerEvents.cs \
	Controllers/MathTextOutputController.cs \
	Controllers/MathTextRecognizerController.cs \
	Output/LaTeXGenerator.cs \
	Output/MathMLGenerator.cs \
	Projection/Hole.cs \
	Projection/HorizontalBitmapProjection.cs \
	Projection/ProjectionMode.cs \
	Projection/VerticalBitmapProjection.cs \
	TreeMaker/RecognizedTreeNode.cs \
	TreeMaker/Steps/ITreeMakerStep.cs \
	TreeMaker/Steps/Step0MakeInitialTree.cs \
	TreeMaker/Steps/Step1SearchFractions.cs \
	TreeMaker/Steps/Step2SearchSuperAndSub.cs \
	Projection/BitmapProjection.cs \
	Utils/ImageUtils.cs \
	BitmapProcesses/BitmapProcess.cs \
	Databases/Caracteristic/CaracteristicDatabase.cs \
	Databases/Caracteristic/Caracteristics/ColorChangesXAboveFourCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/ColorChangesXBelowTwoCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/ColorChangesXEqualsFourCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/ColorChangesXEqualsTwoCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/ColorChangesYAboveFourCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/ColorChangesYBelowTwoCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/ColorChangesYEqualsFourCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/ColorChangesYEqualsTwoCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/EndingPointsEqualsOneCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/EndingPointsEqualsThreeCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/EndingPointsEqualsTwoCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/EndingPointsEqualsZeroCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/IBinaryCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/NumberBigHolesEqualsOneCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/NumberBigHolesEqualsTwoCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/NumberBigHolesEqualsZeroCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/PixelsLeftHalfCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/PixelsNortheastQuadrantCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/PixelsNorthwestQuadrantCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/PixelsSoutheastQuadrantCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/PixelsSouthwestQuadrantCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/PixelsTopHalfCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/ThriceTallerThanWiderCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/TreePointsEqualsOneCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/TreePointsEqualsThreeCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/TreePointsEqualsTwoCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/TreePointsEqualsZeroCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/TwiceTallerThanWiderCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/WiderThanTallerCaracteristic.cs \
	Databases/Caracteristic/Caracteristics/Helpers/CountBlackNeighboursHelper.cs \
	Databases/Caracteristic/Caracteristics/Helpers/CountColorChangesHelper.cs \
	Databases/Caracteristic/Caracteristics/Helpers/CountNumberOfBlackPixelsHelper.cs \
	Databases/Caracteristic/Caracteristics/Helpers/CountNumberOfHolesHelper.cs \
	Databases/Caracteristic/Caracteristics/Helpers/CountPixelsHelper.cs \
	Databases/Caracteristic/Caracteristics/Helpers/CountPixelsWithXNeighboursHelper.cs \
	Databases/Caracteristic/Caracteristics/Helpers/ImageBoxerHelper.cs \
	Databases/MathTextDatabase.cs \
	Databases/Momentum/MomentumDatabase.cs \
	Databases/DatabaseManager.cs \
	Databases/Caracteristic/BinaryCaracteristicNode.cs \
	Databases/Caracteristic/CaracteristicFactory.cs \
	Databases/DatabaseEvents.cs \
	Databases/ISymbolProcess.cs \
	Databases/DatabaseBase.cs \
	Databases/DatabaseInfo.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	./mathtextlibrary.pc.in 

REFERENCES =  \
	$(GTK_SHARP_20_LIBS) \
	-r:System \
	-r:System.Xml

DLL_REFERENCES = 

MATHTEXTLIBRARY_PC = $(BUILD_DIR)/mathtextlibrary.pc
$(MATHTEXTLIBRARY_PC): mathtextlibrary.pc
	mkdir -p $(BUILD_DIR)
	cp '$<' '$@'



$(build_resx_resources) : %.resources: %.resx
	resgen2 '$<' '$@'

$(ASSEMBLY) $(ASSEMBLY).mdb: $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES)
	mkdir -p $(dir $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$@ -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)

include $(top_srcdir)/Makefile.include