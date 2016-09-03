using System.Web.Mvc;

namespace Web.Seo
{
    public static class SeoService
    {
        private const string KeySeoTitle = "seoTitle";
        private const string KeySeoDescription = "seoDescription";
        private const string KeySeoKeywords = "seoKeywords";

        /// <summary>
        /// Sets the seo, overwrites previous written seo
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="keywords"></param>
        public static void SetSeo(this ControllerBase controller, string title, string description, string keywords)
        {
            SetTitle(controller.ControllerContext, title);
            SetDescription(controller.ControllerContext, description);
            SetKeywords(controller.ControllerContext, keywords);
        }

        /// <summary>
        /// Sets the seo, overwrites previous written seo
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        public static void SetSeo(this ControllerBase controller, string title, string description)
        {
            SetTitle(controller.ControllerContext, title);
            SetDescription(controller.ControllerContext, description);
        }

        /// <summary>
        /// Uses string.format if a previous seo is found. If no previous seo is found it will set it without using string.format
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="replaceTitle"></param>
        /// <param name="replaceDescription"></param>
        public static void UpdateSeo(this ControllerBase controller, string replaceTitle, string replaceDescription)
        {
            UpdateTitle(controller, replaceTitle);

            UpdateDescription(controller, replaceDescription);
        }

        /// <summary>
        /// Uses string.format if a previous seo is found. If no previous seo is found it will set it without using string.format
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="replaceTitle"></param>
        /// <param name="replaceDescription"></param>
        /// <param name="replaceKeywords"></param>
        public static void UpdateSeo(this ControllerBase controller, string replaceTitle, string replaceDescription, string replaceKeywords)
        {
            UpdateTitle(controller, replaceTitle);

            UpdateDescription(controller, replaceDescription);

            UpdateKeywords(controller, replaceKeywords);
        }

        /// <summary>
        /// Uses string.format if a previous seo is found. If no previous seo is found it will set it without using string.format
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="replaceTitle"></param>
        public static void UpdateSeo(this ControllerBase controller, string replaceTitle)
        {
            UpdateTitle(controller, replaceTitle);
        }


        public static string GetSeoTitle(TempDataDictionary tempData)
        {
            var title = tempData[KeySeoTitle];

            return (string) (title ?? string.Empty);
        }

        public static string GetSeoDescription(TempDataDictionary tempData)
        {
            var description = tempData[KeySeoDescription];

            return (string)(description ?? string.Empty);
        }

        public static string GetSeoKeywords(TempDataDictionary tempData)
        {
            var keywords = tempData[KeySeoKeywords];

            return (string)(keywords ?? string.Empty);
        }

        private static void UpdateDescription(ControllerBase controller, string replaceDescription)
        {
            var description = (string) controller.TempData[KeySeoDescription];

            controller.TempData[KeySeoDescription] = string.IsNullOrEmpty(description)
                ? replaceDescription : string.Format(description, replaceDescription);
        }

        private static void UpdateTitle(ControllerBase controller, string replaceTitle)
        {
            var title = (string) controller.TempData[KeySeoTitle];

            controller.TempData[KeySeoTitle] = string.IsNullOrEmpty(title)
                ? replaceTitle : string.Format(title, replaceTitle);
        }

        private static void UpdateKeywords(ControllerBase controller, string replaceKeywords)
        {
            var keywords = (string) controller.TempData[KeySeoKeywords];

            controller.TempData[KeySeoKeywords] = string.IsNullOrEmpty(keywords)
                ? replaceKeywords : string.Format(keywords, replaceKeywords);
        }

        private static void SetKeywords(ControllerContext controller, string keywords)
        {
            controller.Controller.TempData[KeySeoKeywords] = keywords;
        }

        private static void SetDescription(ControllerContext controller, string description)
        {
            controller.Controller.TempData[KeySeoDescription] = description;
        }

        private static void SetTitle(ControllerContext controller, string title)
        {
            controller.Controller.TempData[KeySeoTitle] = title;
        }
    }
}
