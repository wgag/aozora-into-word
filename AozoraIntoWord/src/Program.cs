// Copyright (c) 2013 Takahiro Nagao
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Windows.Forms;
using System.Reflection;

namespace AozoraIntoWord
{
    internal static class Program
    {

        public static string Title { get; set; }
        public static string Copyright { get; set; }
        public static int VersionMajor { get; set; }
        public static int VersionMinor { get; set; }
        public static int VersionBuild { get; set; }

        static Program()
        {
            Title = ((AssemblyTitleAttribute) Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute))).Title;
            Copyright = ((AssemblyCopyrightAttribute) Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCopyrightAttribute))).Copyright;

            VersionMajor = Assembly.GetExecutingAssembly().GetName().Version.Major;
            VersionMinor = Assembly.GetExecutingAssembly().GetName().Version.Minor;
            VersionBuild = Assembly.GetExecutingAssembly().GetName().Version.Build;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();

            if (!System.IO.File.Exists("template.dot"))
            {
                MessageBox.Show("template.dot が存在しません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!System.IO.File.Exists("jisx0213-2004-std.txt"))
            {
                MessageBox.Show("jisx0213-2004-std.txt が存在しません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Application.Run(new MainForm());
        }
    }
}
