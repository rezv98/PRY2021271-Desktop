﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ActMon.ResFiles {
    using System;
    
    
    /// <summary>
    ///   Clase de recurso fuertemente tipado, para buscar cadenas traducidas, etc.
    /// </summary>
    // StronglyTypedResourceBuilder generó automáticamente esta clase
    // a través de una herramienta como ResGen o Visual Studio.
    // Para agregar o quitar un miembro, edite el archivo .ResX y, a continuación, vuelva a ejecutar ResGen
    // con la opción /str o recompile su proyecto de VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class GlobalRes {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal GlobalRes() {
        }
        
        /// <summary>
        ///   Devuelve la instancia de ResourceManager almacenada en caché utilizada por esta clase.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ActMon.ResFiles.GlobalRes", typeof(GlobalRes).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Reemplaza la propiedad CurrentUICulture del subproceso actual para todas las
        ///   búsquedas de recursos mediante esta clase de recurso fuertemente tipado.
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
        ///   Busca una cadena traducida similar a Idle Time.
        /// </summary>
        internal static string caption_IdleTime {
            get {
                return ResourceManager.GetString("caption.IdleTime", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a &amp;Activity.
        /// </summary>
        internal static string traymenu_Activity {
            get {
                return ResourceManager.GetString("traymenu.Activity", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a E&amp;xit.
        /// </summary>
        internal static string traymenu_Exit {
            get {
                return ResourceManager.GetString("traymenu.Exit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a &amp;RegisterActivity.
        /// </summary>
        internal static string traymenu_RegisterActivity {
            get {
                return ResourceManager.GetString("traymenu.RegisterActivity", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a &amp;Settings.
        /// </summary>
        internal static string traymenu_Settings {
            get {
                return ResourceManager.GetString("traymenu.Settings", resourceCulture);
            }
        }
    }
}
