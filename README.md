[![] (http://www.compass.ie/wordpress/wp-content/uploads/Compass-logo-Final-300x104.png)](http://www.compass.ie)

# Compass FilePicker


Compass FilePicker is a simple file picker component for Xamarin Android.
It can be used as a full screen activity or as a fragment in a dialog.

![Activity](/../screenshots/Screenshots/activity.png?raw=true "Activity")

![Dialog](/../screenshots/Screenshots/dialog.png?raw=true "Activity")

## Usage example

```
var intent = new Intent(this, typeof(FilePickerActivity));
intent.PutExtra(FilePickerActivity.ExtraNewDirName, "NewFolder");
intent.PutExtra(FilePickerActivity.ExtraInitialDirectory, SelectedDirectory);
intent.PutExtra(FilePickerActivity.ExtraMode, (int)FilePickerMode.Directory);
StartActivityForResult(intent, FilePickerActivity.ResultCodeDirSelected);
```

and then in the OnActivityResult method

```
data.GetStringExtra(FilePickerActivity.ResultSelectedDir)
```

## Supported Platforms

* Xamarin.Android

## Build

Compass FilePicker is built using

* Visual Studio 2013 or Xamarin Studio
* The Xamarin framework

## Sample Applications
This project contains one sample Xamarin.Android application.

## TODO
- Suite of unit tests

## Credits
* [Androd-DirectoryChooser](passy/Android-DirectoryChooser) for the inspiration

## Feedback
All bugs, feature requests, pull requests, feedback, etc., are welcome. [Create an issue](https://github.com/compassinformatics/FilePicker/issues). 

## License
Copyright 2014, [Compass Informatics Ltd](http://www.compass.ie/).

Licensed under the [MIT License](http://opensource.org/licenses/MIT) or see the [`LICENSE`](https://github.com/compassinformatics/FilePicker/blob/master/LICENSE) file.

## Author
- Michele Scandura - 
[gitHub](https://github.com/mikescandy) or  [Twitter](https://twitter.com/mikescandy)
- Compass Informatics - [gitHub](https://github.com/compassinformatics), [Twitter](https://twitter.com/CompassInfo) or [web](https://www.compass.ie) 

## Copyright
Copyright 2014, [Compass Informatics Ltd](http://www.compass.ie/).
