// <copyright file="FileListRowViewHolder.cs" company="Compass Informatics Ltd.">
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

using System.Diagnostics.CodeAnalysis;

using Android.Widget;

using Java.Lang;

namespace Compass.FilePicker
{
    /// <summary>
    /// This class is used to hold references to the views contained in a list row.
    /// </summary>
    /// <remarks>
    /// This is an optimization so that we don't have to always look up the
    /// ImageView and the TextView for a given row in the ListView.
    /// </remarks>
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Using Java.Lang.Object")]
    public class FileListRowViewHolder : Object
    {
        /// <summary>
        /// Gets the image view to display the file/folder icon.
        /// </summary>
        /// <value>
        /// The image view.
        /// </value>
        public ImageView IconImageView { get; private set; }

        /// <summary>
        /// Gets the text view.
        /// </summary>
        /// <value>
        /// The text view.
        /// </value>
        public TextView NameTextView { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileListRowViewHolder"/> class.
        /// </summary>
        /// <param name="textView">The text view.</param>
        /// <param name="imageView">The image view.</param>
        public FileListRowViewHolder(TextView textView, ImageView imageView)
        {
            NameTextView = textView;
            IconImageView = imageView;
        }

        /// <summary>
        /// This method will update the TextView and the ImageView with the filename and the file/folder icon
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileImageResourceId">The file image resource identifier.</param>
        public void Update(string fileName, int fileImageResourceId)
        {
            NameTextView.Text = fileName;
            IconImageView.SetImageResource(fileImageResourceId);
        }
    }
}