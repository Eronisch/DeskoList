using System.Collections.Generic;
using System.Linq;

namespace Web.Bootstrap
{
    public enum BootstrapButtonType
    {
        Warning, Error, Info, Success, Default, Primary
    }

    public enum BootstrapSize
    {
        Large,
        Small,
        ExtraSmall
    }

    /// <summary>
    /// Bootstrap HTML Helper
    /// </summary>
    public static class BootstrapHelper
    {
        /// <summary>
        /// Returns a string containing a bootstrap link button
        /// </summary>
        /// <param name="content"></param>
        /// <param name="link"></param>
        /// <param name="buttonType"></param>
        /// <param name="buttonSize"></param>
        /// <param name="dataOptions"></param>
        /// <returns></returns>
        public static string GetLinkButton(string content, string link, BootstrapButtonType buttonType, BootstrapSize buttonSize, Dictionary<string, string> dataOptions = null)
        {
            return string.Format("<a class='btn {2} {3}' href='{0}' {4}>{1}</a>", link, content, GetClass(buttonType), GetSize(buttonSize), GetOptions(dataOptions));
        }

        private static string GetSize(BootstrapSize bootstrapSize)
        {
            switch (bootstrapSize)
            {
                case BootstrapSize.Large:
                {
                    return "btn-lg";
                }
                case BootstrapSize.Small:
                {
                    return "btn-sm";
                }
                default:
                {
                    return "btn-xs";
                }
            }
        }

        private static string GetOptions(Dictionary<string, string> options = null)
        {
            if (options == null) { return string.Empty;}

            return options.Aggregate(string.Empty, (current, option) => current + string.Format("data-{0}='{1}' ", option.Key, option.Value));
        }

        private static string GetClass(BootstrapButtonType buttonType)
        {
            switch (buttonType)
            {
                case BootstrapButtonType.Warning:
                    {
                        return "btn-warning";
                    }
                case BootstrapButtonType.Error:
                    {
                        return "btn-danger";
                    }
                case BootstrapButtonType.Info:
                    {
                        return "btn-info";
                    }
                case BootstrapButtonType.Success:
                    {
                        return "btn-success";
                    }
                case BootstrapButtonType.Default:
                    {
                        return "btn-default";
                    }
                default:
                    {
                        return "btn-primary";
                    }
            }
        }
    }
}
