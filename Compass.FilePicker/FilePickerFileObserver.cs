// <copyright file="FilePickerFileObserver.cs" company="Compass Informatics Ltd.">
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

using Android.OS;
using Android.Runtime;

namespace Compass.FilePicker
{
    public delegate void OnFileEventHandler(FileObserverEvents e, string path);

    public class FilePickerFileObserver : FileObserver
    {
        /// <inheritdoc />
        public event OnFileEventHandler OnFileEvent;

        /// <inheritdoc />
        public FilePickerFileObserver(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }
            
        /// <inheritdoc />
        public FilePickerFileObserver(string path)
            : base(path)
        {
        }

        /// <inheritdoc />
        public FilePickerFileObserver(string path, FileObserverEvents mask)
            : base(path, mask)
        {
        }

        /// <inheritdoc />
        public override void OnEvent(FileObserverEvents e, string path)
        {
            OnFileEvent(e, path);
        }
    }
}