using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Comm100.Framework.Common
{
    public class VersionHelper
    {
        public string GetVersion()
        {

            try
            {
                var version = "";
                var entryAssembly = Assembly.GetEntryAssembly();
                var attr = entryAssembly.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(AssemblyDescriptionAttribute));
                if (attr != null)
                {
                    var argument = attr.ConstructorArguments.FirstOrDefault();
                    if (argument != null && argument.Value != null)
                    {
                        version = argument.Value.ToString();
                    }
                }

                var lastWriteTime = System.IO.File.GetLastWriteTime(entryAssembly.Location);

                if (string.IsNullOrEmpty(version))
                {
                    version = "unknown version";
                }

                return $"{version} ,  {lastWriteTime}";
            }
            catch (Exception)
            {
                return "";
            }

        }

    }
}
