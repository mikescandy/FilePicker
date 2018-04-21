// <copyright file="FilePickerActivity.cs" company="Compass Informatics Ltd.">
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

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;

namespace Compass.FilePicker
{
    /// <summary>
    /// The File picker activity
    /// </summary>
    [Activity(Label = "Pictures folder")]
    public class FilePickerActivity : Activity
    {
        /**
         * Extra to define the path of the directory that will be shown first.
         * If it is not sent or if path denotes a non readable/writable directory
         * or it is not a directory, it defaults to
         * {@link android.os.Environment#getExternalStorageDirectory()}
         */

        /// <summary>
        /// The extra new dir name
        /// </summary>
        public const string ExtraNewDirName = "directory_name";

        /// <summary>
        /// The extra initial directory
        /// </summary>
        public const string ExtraInitialDirectory = "initial_directory";

        /// <summary>
        /// The file picker mode (File or Directory)
        /// </summary>
        public const string ExtraMode = "mode";

        /// <summary>
        /// The result selected dir
        /// </summary>
        public const string ResultSelectedDir = "selected_dir";

        /// <summary>
        /// The result code dir selected
        /// </summary>
        public const int ResultCodeDirSelected = 1;

        private FilePickerFragment fragment;

        /// <inheritdoc />
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetupActionBar();
            SetContentView(Resource.Layout.filepicker_mainactivity);

            var newDirName = Intent.GetStringExtra(ExtraNewDirName);
            var initialDir = Intent.GetStringExtra(ExtraInitialDirectory);
            var filePickerMode = (FilePickerMode)Intent.GetIntExtra(ExtraMode, 1);

            if (bundle == null)
            {
                fragment = new FilePickerFragment(initialDir, newDirName, filePickerMode);
                fragment.Cancel += sender =>
                    {
                        SetResult(Result.Canceled);
                        Finish();
                    };

                fragment.FileSelected += (sender, path) =>
                    {
                        var intent = new Intent();
                        intent.PutExtra(ResultSelectedDir, path);
                        SetResult(Result.Ok, intent); // should be firstuser?
                        Finish();
                    };
                FragmentManager.BeginTransaction().Add(Resource.Id.filepicker_main, fragment).Commit();
            }
        }

        private void SetupActionBar()
        {
            // there might not be an ActionBar, for example when started in Theme.Holo.Dialog.NoActionBar theme
            var actionBar = ActionBar;
            if (actionBar != null)
            {
                actionBar.SetDisplayHomeAsUpEnabled(true);
            }
        }

        /// <inheritdoc />
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            var itemId = item.ItemId;
            if (itemId == Android.Resource.Id.Home)
            {
                SetResult(Result.Canceled);
                Finish();
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}