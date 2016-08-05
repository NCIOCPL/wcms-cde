﻿using System;
using System.IO;
using System.Collections.Generic;
using NCI.Logging;
using NVelocity;
using NVelocity.App;
using NVelocity.Context;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Globalization;

namespace NCI.Web.CDE.Modules
{
    public class VelocityTemplate
    {
        private static VelocityEngineManager _engineManager = new VelocityEngineManager();

        [Obsolete("Use MergeTemplateWithResultsByFilepath() instead.")]
        public static string MergeTemplateWithResults(string template, object obj)
        {
            try
            {
                Velocity.Init();
                VelocityContext context = new VelocityContext();
                context.Put("DynamicSearch", obj);
                context.Put("Tools", new VelocityTools());
                StringWriter writer = new StringWriter();
                Velocity.Evaluate(context, writer, "", template);
                return writer.GetStringBuilder().ToString();
            }
            catch (Exception ex)
            {
                Logger.LogError("VelocityTemplate:MergeTemplateWithResults", "Failed when evaluating results template and object.", NCIErrorLevel.Error, ex);
                throw (ex);
            }
        }

        public static string MergeTemplateWithResultsByFilepath(string filepath, object obj)
        {
            try
            {
                VelocityEngine velocity = _engineManager.Engine;
                _engineManager.WatchTemplateDirectory(filepath);

                velocity.Init();
                VelocityContext context = new VelocityContext();
                context.Put("SearchResults", obj);
                context.Put("CDEContext", new CDEContext());
                context.Put("PageContext", HttpContext.Current);
                context.Put("Tools", new VelocityTools());
                StreamReader sr = new StreamReader(HttpContext.Current.Server.MapPath(filepath));
                string template = sr.ReadToEnd();
                sr.Close();
                StringWriter writer = new StringWriter();
                velocity.Evaluate(context, writer, "", template);
                return writer.GetStringBuilder().ToString();
            }
            catch (Exception ex)
            {
                Logger.LogError("VelocityTemplate:MergeTemplateWithResultsByFilepath", "Failed when evaluating results template and object.", NCIErrorLevel.Error, ex);
                throw;
            }
        }

        class CDEContext
        {
            public string Language { get; set; }
            public CDEContext()
            {
                if (System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "es")
                    //CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "es")
                {
                    Language = "es";
                }
                else
                {
                    Language = "en";
                }
            }
        }

        class VelocityTools
        {
            public bool IsNull(object obj)
            {
                return obj == null;
            }

            public bool IsNullOrWhitespace(string str)
            {
                return String.IsNullOrWhiteSpace(str);
            }
            public List<string> CreateEmptyStringList()
            {
                return new List<string>();
            }

            public string Replace(string str, string pattern1, string pattern2)
            {
                string rtn = str.Replace(pattern1, pattern2);
                return rtn;
            }

            public string Join(string[] strArr)
            {
                if(strArr != null) 
                {
                   return string.Join("",strArr);
                }
                else 
                { 
                    return string.Empty;
                }
            }
		}

        /// <summary>
        /// Helper class to manage use of a single instance of the Velocity Engine.
        /// The managed instance of the engine may be released by either calling 
        /// ResetVelocityEngine() directly, or by using WatchTemplateDirectory() to
        /// set a file watcher on the directory where the templates are stored.
        /// 
        /// </summary>
        /// <remarks>
        /// If WatchTemplateDirectory() is used, it is assumed that all templates
        /// are stored in the same directory structure.  Although changes to files
        /// in the directory structure will trigger replacement of the managed engine,
        /// create, delete, rename operations on directories will not.
        /// </remarks>
        private class VelocityEngineManager
        {
            /// <summary>
            /// The instance. Not available for access outside this helper class.
            /// </summary>
            private static VelocityEngine _velocityEngine = null;

            /// <summary>
            ///  Used by WatchTemplateDirectory() to watch for changes in the
            ///  directory structure where templates are stored.
            /// </summary>
            private static FileSystemWatcher templateDirectoryWatcher;

            /// <summary>
            /// Read-only property for accessing the managed instance of VelocityEngine.
            /// By design, use of this property enforces thread-safe access to the engine.
            /// All callers will have their own reference to the managed instance, which
            /// remains valid after a call to ResetVelocityEngine().
            /// </summary>
            public VelocityEngine Engine
            {
                get
                {
                    lock (this)
                    {
                        if (_velocityEngine == null)
                            _velocityEngine = new VelocityEngine();

                        return _velocityEngine;
                    }
                }
            }

            /// <summary>
            /// Thread-safe mechanism for removing the managed VelocityEngine instance.
            /// </summary>
            public void ResetVelocityEngine()
            {
                lock (this)
                {
                    _velocityEngine = null;
                }
            }

            /// <summary>
            /// Sets a file system watcher in the directory which contains filepath. In the event that any files
            /// are created, deleted, modified, or renamed, the current velocity engine will be released and the
            /// next access to the Engine property will result in a new one being allocated.
            /// </summary>
            /// <param name="filepath">The path to a velocity template.</param>
            public void WatchTemplateDirectory(string filepath)
            {
                if (templateDirectoryWatcher == null)
                {
                    lock (typeof(VelocityTemplate.VelocityEngineManager))
                    {
                        if(templateDirectoryWatcher == null)
                        {
                            filepath = HttpContext.Current.Server.MapPath(filepath);

                            // Set the watcher on the directory.
                            templateDirectoryWatcher = new FileSystemWatcher((Path.GetDirectoryName(filepath)));
                            templateDirectoryWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Size | NotifyFilters.LastAccess | NotifyFilters.Attributes;
                            templateDirectoryWatcher.EnableRaisingEvents = true;

                            templateDirectoryWatcher.Changed += new FileSystemEventHandler(TemplatesChanged);
                            templateDirectoryWatcher.Created += new FileSystemEventHandler(TemplatesChanged);
                            templateDirectoryWatcher.Deleted += new FileSystemEventHandler(TemplatesChanged);
                            templateDirectoryWatcher.Renamed += new RenamedEventHandler(TemplatesChanged);
                        }
                    }
                }
            }

            /// <summary>
            /// Event handler for files in the template directory being modified, created, or deleted.
            /// Removes the reference to the managed velocity template
            /// </summary>
            /// <param name="src">event source (not used)</param>
            /// <param name="e">event arguments (not used)</param>
            private void TemplatesChanged(object src, FileSystemEventArgs e)
            {
                ResetVelocityEngine();
            }

            /// <summary>
            /// Event handler for files in the template directory being renamed.
            /// Removes the reference to the managed velocity template
            /// </summary>
            /// <param name="src">event source (not used)</param>
            /// <param name="e">event arguments (not used)</param>
            private void TemplatesRenamed(object src, RenamedEventArgs e)
            {
                ResetVelocityEngine();
            }

        }
    }
}
