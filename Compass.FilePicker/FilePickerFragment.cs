// <copyright file="FilePickerFragment.cs" company="Compass Informatics Ltd.">
// Copyright (c) Compass Informatics 2014, All Right Reserved, http://compass.ie/
//
// This source is subject to the MIT License.
// Please see the License file for more information.
// All other rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// </copyright>
// <author>Michele Scandura</author>
// <email>mscandura@compass.ie</email>
// <date>30-04-2014</date>
// <summary>Contains a simple Point with floating coordinates.</summary>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;

using Java.Lang;

using Enum = System.Enum;
using Environment = Android.OS.Environment;
using Exception = System.Exception;
using File = Java.IO.File;

namespace Compass.FilePicker
{
    /// <summary>
    /// A DialogFragment that will show the files and subdirectories of a given directory.
    /// </summary>
    public class FilePickerFragment : DialogFragment
    {
        private const string ArgInitialDir = "ArgInitialDirectory";
        private const string ArgNewDirectoryName = "ArgNewDirectoryName";
        private const string ArgMode = "ArgMode";
        private const string LogTag = "Compass.FilePicker.FileListFragment";
        private const string KeyCurrentDirectory = "KeyCurrentDirectory";

        private Button btnCancel;
        private Button btnConfirm;
        private DirectoryInfo currentDirectory;
        private File selectedFile;
        private FileListAdapter adapter;
        private FilePickerMode filePickerMode;
        private ImageButton btnCreateFolder;
        private ImageButton btnLevelUp;
        private ListView listFiles;
        private FilePickerFileObserver fileObserver;
        private string initialDirectory;
        private string newDirectoryName;

        /// <summary>
        /// Occurs when the user press the Confirm button.
        /// </summary>
        public event FileSelectedEventHandler FileSelected;

        /// <summary>
        /// Occurs when the user press the Cancel button.
        /// </summary>
        public event CancelEventHandler Cancel;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilePickerFragment"/> class.
        /// </summary>
        public FilePickerFragment()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilePickerFragment"/> class.
        /// </summary>
        /// <param name="initialDir">The initial dirrectory.</param>
        /// <param name="mode">The filepicker mode.</param>
        public FilePickerFragment(string initialDir, FilePickerMode mode = FilePickerMode.File)
            : this(initialDir, null, mode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilePickerFragment"/> class.
        /// </summary>
        /// <param name="initialDir">The initial dirrectory.</param>
        /// <param name="newDirName">Default name for new folders.</param>
        /// <param name="mode">The filepicker mode.</param>
        public FilePickerFragment(string initialDir, string newDirName, FilePickerMode mode = FilePickerMode.File)
        {
            var args = new Bundle();
            args.PutString(ArgNewDirectoryName, newDirName ?? string.Empty);
            args.PutString(ArgInitialDir, initialDir ?? string.Empty);
            args.PutInt(ArgMode, (int)mode);
            Arguments = args;
        }

        /// <inheritdoc />
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (Arguments == null)
            {
                throw new IllegalArgumentException("You must create DirectoryChooserFragment via the FileListFragment(string, string, FilePickerMode) constructor.");
            }

            newDirectoryName = Arguments.GetString(ArgNewDirectoryName);
            initialDirectory = Arguments.GetString(ArgInitialDir);
            filePickerMode = (FilePickerMode)Arguments.GetInt(ArgMode);

            if (savedInstanceState != null)
            {
                initialDirectory = savedInstanceState.GetString(KeyCurrentDirectory);
            }

            if (ShowsDialog)
            {
                SetStyle(DialogFragmentStyle.NoTitle, 0);
            }
            else
            {
                SetHasOptionsMenu(true);
            }
        }

        /// <inheritdoc />
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.Assert(Activity != null, "Activity != null");
            var view = inflater.Inflate(Resource.Layout.filepicker_fragment, container, false);

            btnConfirm = view.FindViewById<Button>(Resource.Id.btnConfirm);
            btnCancel = view.FindViewById<Button>(Resource.Id.btnCancel);
            btnCreateFolder = view.FindViewById<ImageButton>(Resource.Id.btnCreateFolder2);
            btnLevelUp = view.FindViewById<ImageButton>(Resource.Id.btnNavUp);
            listFiles = view.FindViewById<ListView>(Resource.Id.directoryList);

            switch (filePickerMode)
            {
                case FilePickerMode.File:
                    view.FindViewById<TextView>(Resource.Id.txtvSelectedFolderLabel).Text = GetString(Resource.String.filepicker_selected_file_label);
                    break;
                case FilePickerMode.Directory:
                    view.FindViewById<TextView>(Resource.Id.txtvSelectedFolderLabel).Text = GetString(Resource.String.filepicker_selected_folder_label);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            btnConfirm.Click += (sender, args) => Confirm();
            btnCancel.Click += (sender, args) => CancelAction();
            btnCreateFolder.Click += (sender, args) => OpenNewFolderDialog();
            btnLevelUp.Click += (sender, args) => UpOneLevel();
            listFiles.ItemClick += ListFilesOnItemClick;

            if (!ShowsDialog || filePickerMode == FilePickerMode.File)
            {
                btnCreateFolder.Visibility = ViewStates.Gone;
            }

            adapter = new FileListAdapter(Activity, new FileSystemInfo[0]);
            listFiles.Adapter = adapter;

            if (!string.IsNullOrWhiteSpace(initialDirectory) && IsValidFile(initialDirectory))
            {
                selectedFile = new File(initialDirectory);
            }
            else
            {
                selectedFile = Environment.ExternalStorageDirectory;
            }

            return view;
        }

        /// <inheritdoc />
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.filepicker_menu, menu);
            var menuItem = menu.FindItem(Resource.Id.filepicker_new_folder_item);
            if (menuItem == null)
            {
                return;
            }
            menuItem.SetVisible(IsValidFile(selectedFile) && newDirectoryName != null);
        }

        /// <inheritdoc />
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId != Resource.Id.filepicker_new_folder_item)
            {
                return base.OnOptionsItemSelected(item);
            }
            OpenNewFolderDialog();
            return true;
        }

        /// <inheritdoc />
        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutString(KeyCurrentDirectory, selectedFile.AbsolutePath);
        }

        /// <inheritdoc />
        public override void OnPause()
        {
            base.OnPause();
            if (fileObserver != null)
            {
                fileObserver.StopWatching();
            }
        }

        /// <inheritdoc />
        public override void OnResume()
        {
            base.OnResume();
            if (fileObserver != null)
            {
                fileObserver.StartWatching();
            }
            RefreshFilesList();
        }

        private void ListFilesOnItemClick(object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
        {
            var fileSystemInfo = adapter.GetItem(itemClickEventArgs.Position);

            if (fileSystemInfo.IsFile())
            {
                selectedFile = new File(fileSystemInfo.FullName);
                UpdateSelectedFileText();
                RefreshButtonState();
            }
            else
            {
                // Dig into this directory, and display it's contents
                RefreshFilesList(fileSystemInfo.FullName);
            }
        }

        private void RefreshFilesList()
        {
            if (selectedFile != null && selectedFile.IsDirectory)
            {
                RefreshFilesList(selectedFile);
            }
        }

        private void RefreshFilesList(string path)
        {
            var file = new File(path);
            RefreshFilesList(file);
            file.Dispose();
        }

        private void RefreshFilesList(File targetDirectory)
        {
            if (targetDirectory == null)
            {
                LogDebug("Directory can't be null");
            }
            else if (!targetDirectory.IsDirectory)
            {
                LogDebug("Cant change to a file");
            }
            else
            {
                var visibleThings = new List<FileSystemInfo>();
                var dir = new DirectoryInfo(targetDirectory.Path);
                try
                {
                    switch (filePickerMode)
                    {
                        case FilePickerMode.File:
                            visibleThings.AddRange(dir.GetFileSystemInfos().Where(item => item.IsVisible()));
                            break;
                        case FilePickerMode.Directory:
                            foreach (var item in
                                dir.GetFileSystemInfos()
                                    .Where(
                                        item =>
                                        item.IsVisible() && item.IsDirectory()
                                        && (item.Attributes & FileAttributes.System) != FileAttributes.System
                                        && (item.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden))
                            {
                                try
                                {
                                    var directoryInfo = new DirectoryInfo(item.FullName);
                                    // Trying to access a subfolder. If it's ok I'll check if it's writable.
                                    // If everything is fine I'll add the folder to the list of visible things.
                                    directoryInfo.GetFileSystemInfos(); //Throws an exception if it can't access the folder
                                    var javaFile = new File(item.FullName);

                                    // native java method to check if a file or folder is writable
                                    if (javaFile.CanWrite())
                                    {
                                        visibleThings.Add(item);
                                    }
                                    javaFile.Dispose(); // remember to dispose to avoid keeping references to java objects.
                                }
                                catch (Exception ex)
                                {
                                    LogDebug("Directory " + item.FullName + "is not writable.");
                                    LogError(ex.Message);
                                }
                            }
                            break;
                        default:
                            // something went wrong
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch (Exception ex)
                {
                    LogError("Couldn't access the directory " + currentDirectory.FullName + "; " + ex);
                    Toast.MakeText(Activity, "Problem retrieving contents of " + targetDirectory, ToastLength.Long).Show();
                    return;
                }

                currentDirectory = dir;
                selectedFile = new File(currentDirectory.FullName);
                adapter.AddDirectoryContents(visibleThings);
                CreateFileObserver(currentDirectory.FullName);
                fileObserver.StartWatching();
                UpdateSelectedFileText();

                LogDebug(string.Format("Displaying the contents of directory {0}.", targetDirectory));
                RefreshButtonState();
            }
        }

        private void UpdateSelectedFileText()
        {
			if (View == null) 
			{
				return;
			}

            var textView = View.FindViewById<TextView>(Resource.Id.txtvSelectedFolder);
            if (textView == null)
            {
                return;
            }
            switch (filePickerMode)
            {
                case FilePickerMode.File:
                    if (selectedFile.IsFile)
                    {
                        textView.Text = selectedFile.Name;
                    }
                    break;
                case FilePickerMode.Directory:
                    if (selectedFile.IsDirectory)
                    {
                        textView.Text = selectedFile.AbsolutePath;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpOneLevel()
        {
            var path = currentDirectory.Parent;
            if (path != null && !string.IsNullOrWhiteSpace(path.FullName))
            {
                RefreshFilesList(path.FullName);
            }
        }

        private void OpenNewFolderDialog()
        {
            var builder = new AlertDialog.Builder(Activity);
            var customView = Activity.LayoutInflater.Inflate(Resource.Layout.filepicker_createfolder, null);
            var editText = customView.FindViewById<EditText>(Resource.Id.folderName);
            builder.SetView(customView);
            builder.SetMessage(GetString(Resource.String.filepicker_enter_folder_msg));
            builder.SetPositiveButton(
                GetString(Resource.String.filepicker_ok),
                (s, e) =>
                {
                    CreateFolder(currentDirectory.FullName, editText.Text);
                    ((AlertDialog)s).Dismiss();
                });
            builder.SetNegativeButton(GetString(Resource.String.filepicker_cancel_label), (s, e) => ((AlertDialog)s).Dismiss());
            builder.Create().Show();
        }

        private void CreateFolder(string baseFolder, string newFolder)
        {
            try
            {
                var path = Path.Combine(baseFolder, newFolder);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    RefreshFilesList(baseFolder);
                }
                else if (Directory.Exists(path))
                {
                    Toast.MakeText(Activity, GetString(Resource.String.filepicker_create_folder_error_already_exists), ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Activity, GetString(Resource.String.filepicker_create_folder_error), ToastLength.Short).Show();
                LogError(ex.Message);
            }
        }

        private void Confirm()
        {
            if (FileSelected != null)
            {
                FileSelected(this, selectedFile.AbsolutePath);
            }
        }

        private void CancelAction()
        {
            if (Cancel != null)
            {
                Cancel(this);
            }
        }

        private bool IsValidFile(string path)
        {
            var file = new File(path);
            var isValid = IsValidFile(file);
            file.Dispose();
            return isValid;
        }

        private bool IsValidFile(File file)
        {
            bool isValid;
            switch (filePickerMode)
            {
                case FilePickerMode.File:
                    isValid = file != null && file.IsFile && file.CanRead() && file.CanWrite();
                    break;
                case FilePickerMode.Directory:
                    isValid = file != null && file.IsDirectory && file.CanRead() && file.CanWrite();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return isValid;
        }

        private bool IsValidFile()
        {
            return IsValidFile(selectedFile);
        }

        private void RefreshButtonState()
        {
            if (Activity == null)
            {
                return;
            }
            btnConfirm.Enabled = IsValidFile();
            Activity.InvalidateOptionsMenu();
        }

        private static void LogDebug(string message)
        {
            Log.Debug(LogTag, message);
        }

        private static void LogError(string message)
        {
            Log.Error(LogTag, message);
        }

        private void CreateFileObserver(string path)
        {
            // FileObserverEvents.Create | FileObserverEvents.Delete | FileObserverEvents.MovedFrom | FileObserverEvents.MovedTo;
            const FileObserverEvents Mask = FileObserverEvents.Create | FileObserverEvents.Delete | FileObserverEvents.MovedFrom | FileObserverEvents.MovedTo | (FileObserverEvents)0x40000000;
            Console.WriteLine(Mask.ToString());
            fileObserver = new FilePickerFileObserver(path, Mask);
            fileObserver.OnFileEvent += (events, s) =>
                {
                    LogDebug(string.Format("FileObserver event received - {0}", events));
                    if ((events & (FileObserverEvents)0x40000000) == (FileObserverEvents)0x40000000)
                    {
                        Console.WriteLine("Folder event");
                    }
                    events &= FileObserverEvents.AllEvents;
                    var eventName = Enum.GetName(typeof(FileObserverEvents), events);
                    Console.WriteLine(eventName);
                    if ((events & Mask) == events)
                        if (Activity != null)
                        {
                            Activity.RunOnUiThread(RefreshFilesList);
                        }

                };
        }
    }
}