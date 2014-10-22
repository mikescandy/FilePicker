// <copyright file="FileListAdapter.cs" company="Compass Informatics Ltd.">
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

using System.Collections.Generic;
using System.IO;
using System.Linq;

using Android.Content;
using Android.Views;
using Android.Widget;

namespace Compass.FilePicker
{
    /// <summary>
    /// The file list adapter, used in the file picker
    /// </summary>
    public class FileListAdapter : ArrayAdapter<FileSystemInfo>
    {
        private readonly Context context;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileListAdapter"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="fileSystemInfos">The list of FileSystemInfo containing the directory contents.</param>
        public FileListAdapter(Context context, IList<FileSystemInfo> fileSystemInfos)
            : base(context, Resource.Layout.filepicker_listitem, Android.Resource.Id.Text1, fileSystemInfos)
        {
            this.context = context;
        }

        /// <summary>
        /// We provide this method to get around some of the //todo fix this comment
        /// </summary>
        /// <param name="directoryContents">The directory contents.</param>
        public void AddDirectoryContents(IEnumerable<FileSystemInfo> directoryContents)
        {
            Clear();
            // Notify the adapter that things have changed or that there is nothing to display.
            var fileSystemInfos = directoryContents as IList<FileSystemInfo> ?? directoryContents.ToList();
            if (fileSystemInfos.Any())
            {
                AddAll(fileSystemInfos.ToArray());
                NotifyDataSetChanged();
            }
            else
            {
                NotifyDataSetInvalidated();
            }
        }

        /// <inheritdoc/>
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var fileSystemEntry = GetItem(position);

            FileListRowViewHolder viewHolder;
            View row;
            if (convertView == null)
            {
                row = context.GetLayoutInflater().Inflate(Resource.Layout.filepicker_listitem, parent, false);
                viewHolder = new FileListRowViewHolder(row.FindViewById<TextView>(Resource.Id.file_picker_text), row.FindViewById<ImageView>(Resource.Id.file_picker_image));
                row.Tag = viewHolder;
            }
            else
            {
                row = convertView;
                viewHolder = (FileListRowViewHolder)row.Tag;
            }
            viewHolder.Update(fileSystemEntry.Name, fileSystemEntry.IsDirectory() ? Resource.Drawable.filepicker_folder : Resource.Drawable.filepicker_file);
            return row;
        }
    }
}