// <copyright file="Activity1.cs" company="Compass Informatics Ltd.">
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

using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Compass.FilePicker;

namespace SampleApplication
{
    [Activity(Label = "SampleApplication", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private const string SelectedDirectory = "/";

        private FilePickerFragment filePickerFragment;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            var buttonDirectoryActivity = FindViewById<Button>(Resource.Id.btnDirectoryActivity);
            var buttonFileActivity = FindViewById<Button>(Resource.Id.btnFileActivity);
            var buttonDirectoryDialog = FindViewById<Button>(Resource.Id.btnDirectoryDialog);
            var buttonFileDialog = FindViewById<Button>(Resource.Id.btnFileDialog);

            buttonDirectoryActivity.Click += delegate
            {
                var intent = new Intent(this, typeof(FilePickerActivity));
                intent.PutExtra(FilePickerActivity.ExtraNewDirName, "NewFolder");
                intent.PutExtra(FilePickerActivity.ExtraInitialDirectory, SelectedDirectory);
                intent.PutExtra(FilePickerActivity.ExtraMode, (int)FilePickerMode.Directory);
                StartActivityForResult(intent, FilePickerActivity.ResultCodeDirSelected);
            };

            buttonFileActivity.Click += delegate
            {
                var intent = new Intent(this, typeof(FilePickerActivity));
                StartActivityForResult(intent, FilePickerActivity.ResultCodeDirSelected);
            };

            buttonDirectoryDialog.Click += delegate
            {
                filePickerFragment = new FilePickerFragment(null, null, FilePickerMode.Directory);
                filePickerFragment.FileSelected += (sender, path) =>
                    {
                        filePickerFragment.Dismiss();
                        UpdateSelectedText(path);
                    };
                filePickerFragment.Cancel += sender => filePickerFragment.Dismiss();
                filePickerFragment.Show(FragmentManager, "FilePicker");
            };

            buttonFileDialog.Click += delegate
            {
                filePickerFragment = new FilePickerFragment();
                filePickerFragment.FileSelected += (sender, path) =>
                {
                    filePickerFragment.Dismiss();
                    UpdateSelectedText(path);
                };
                filePickerFragment.Cancel += sender => filePickerFragment.Dismiss();
                filePickerFragment.Show(FragmentManager, "FilePicker");
            };
        }

        private void UpdateSelectedText(string text)
        {
            var textSelectedDirectory = FindViewById<TextView>(Resource.Id.textSelectedDirectory);
            if (textSelectedDirectory != null)
            {
                textSelectedDirectory.Text = text;
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            switch (requestCode)
            {
                case FilePickerActivity.ResultCodeDirSelected:
                    switch (resultCode)
                    {
                        case Result.Canceled:
                            break;
                        case Result.FirstUser:
                            break;
                        case Result.Ok:
                            UpdateSelectedText(data.GetStringExtra(FilePickerActivity.ResultSelectedDir));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("resultCode");
                    }
                    break;
            }
        }
    }
}

