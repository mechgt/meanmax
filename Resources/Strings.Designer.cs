﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MeanMax.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MeanMax.Resources.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Upgrade to enable multiple charts.
        /// </summary>
        internal static string Label_EvalCharts {
            get {
                return ResourceManager.GetString("Label_EvalCharts", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Trial version supports previous {0} days.
        ///Upgrade for unlimited access..
        /// </summary>
        internal static string Label_EvalDays {
            get {
                return ResourceManager.GetString("Label_EvalDays", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Upgrade to enable more charts and view activities older than {0} days.
        /// </summary>
        internal static string Label_EvalReport {
            get {
                return ResourceManager.GetString("Label_EvalReport", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Mean-Maximum.
        /// </summary>
        internal static string Label_MeanMax {
            get {
                return ResourceManager.GetString("Label_MeanMax", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Selected Charts.
        /// </summary>
        internal static string Label_SelectedCharts {
            get {
                return ResourceManager.GetString("Label_SelectedCharts", resourceCulture);
            }
        }
    }
}
