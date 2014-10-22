// <copyright file="Helpers.cs" company="Compass Informatics Ltd.">
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

using System.IO;

using Android.Content;
using Android.Runtime;
using Android.Views;

namespace Compass.FilePicker
{
    /// <summary>
    /// Helper extensions
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Will obtain an instance of a LayoutInflater for the specified Context.
        /// </summary>
        /// <param name="context">The Context </param>
        /// <returns> </returns>
        public static LayoutInflater GetLayoutInflater(this Context context)
        {
            return context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
        }

        /// <summary>
        /// This method will tell us if the given FileSystemInfo instance is a directory.
        /// </summary>
        /// <param name="fileSystemInfo">The FileSystemInfo </param>
        /// <returns> </returns>
        public static bool IsDirectory(this FileSystemInfo fileSystemInfo)
        {
            if (fileSystemInfo == null)
            {
                return false;
            }

            return (fileSystemInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
        }

        /// <summary>
        /// This method will tell us if the given FileSystemInfo instance is a file.
        /// </summary>
        /// <param name="fileSystemInfo">The FileSystemInfo </param>
        /// <returns> </returns>
        public static bool IsFile(this FileSystemInfo fileSystemInfo)
        {
            if (fileSystemInfo == null)
            {
                return false;
            }
            return !IsDirectory(fileSystemInfo);
        }

        /// <summary>
        /// This method will tell us if the given FileSystemInfo instance is visible.
        /// </summary>
        /// <param name="fileSystemInfo">The FileSystemInfo </param>
        /// <returns> </returns>
        public static bool IsVisible(this FileSystemInfo fileSystemInfo)
        {
            if (fileSystemInfo == null)
            {
                return false;
            }
            var isHidden = (fileSystemInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
            return !isHidden;
        }
    }
}
