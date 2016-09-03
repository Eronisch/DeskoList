using System.Web.Mvc;

namespace Web.Messages
{
    /// <summary>
    /// Manager for setting error & success messages in the tempdata
    /// </summary>
    public static class MessageService
    {
        public const string ErrorKey = "Error";
        public const string SuccessKey = "Success";

        /// <summary>
        /// Add an error message to the tempdata
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="errorMessage"></param>
        public static void SetError(this Controller controller, string errorMessage)
        {
            controller.TempData[ErrorKey] = errorMessage;
        }

        /// <summary>
        /// Add an error message to the tempdata
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="errorMessage"></param>
        public static void SetError(this ControllerBase controller, string errorMessage)
        {
            controller.TempData[ErrorKey] = errorMessage;
        }

        /// <summary>
        /// Add an success message to the tempdata
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="succesMessage"></param>
        public static void SetSuccess(this Controller controller, string succesMessage)
        {
            controller.TempData[SuccessKey] = succesMessage;
        }

        /// <summary>
        /// Add an success message to the tempdata
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="errorMessage"></param>
        public static void SetSuccess(this ControllerBase controller, string errorMessage)
        {
            controller.TempData[SuccessKey] = errorMessage;
        }

        /// <summary>
        /// Check if the tempdata contains an error message
        /// </summary>
        /// <param name="tempData"></param>
        /// <returns></returns>
        public static bool HasError(TempDataDictionary tempData)
        {
            return tempData.ContainsKey(ErrorKey);
        }

        /// <summary>
        /// Checks if the tempdata contains an success message
        /// </summary>
        /// <param name="tempData"></param>
        /// <returns></returns>
        public static bool HasSuccess(TempDataDictionary tempData)
        {
            return tempData.ContainsKey(SuccessKey);
        }

        /// <summary>
        /// Get the error message from the tempdata
        /// </summary>
        /// <param name="tempData"></param>
        /// <returns>Error message or null</returns>
        public static string GetErrorMessage(TempDataDictionary tempData)
        {
            return HasError(tempData) ? tempData[ErrorKey].ToString() : null;
        }

        /// <summary>
        /// Get the success message from the tempdata
        /// </summary>
        /// <param name="tempData"></param>
        /// <returns>Success message or null</returns>
        public static string GetSuccessMessage(TempDataDictionary tempData)
        {
            return HasSuccess(tempData) ? tempData[SuccessKey].ToString() : null;
        }
    }
}
