This class is used to store and manage [MathTextLearner](MathTextLearner.md)'s configuration.

Basically it offers storage for a list of instances of [BitmapProcess](BitmapProcess.md), representing the different algorithms which can be used to process the image in order to prepare it before being recognized.

This class has also the responsibility of persisting those settings. For doing so, it uses the [ConfigFileUtils](ConfigFileUtils.md) class for a path for the [MathTextLearner](MathTextLearner.md) application, and then uses it to retrieve and store the settings (the later only when requested). If no config file is found, a embedded text file in the [MathTextLearner](MathTextLearner.md) assembly is used instead of it.