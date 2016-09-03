using System.Web.Mvc;

namespace Web.Search
{
    public static class SearchHelper
    {
        private const string SearchKey = "searchedText";

        public static string GetSearchText(TempDataDictionary tempDataDictionary)
        {
            return tempDataDictionary.ContainsKey(SearchKey) ? tempDataDictionary[SearchKey].ToString() : string.Empty;
        }

        public static void SetSearchedText(string searchText, TempDataDictionary tempDataDictionary)
        {
            tempDataDictionary[SearchKey] = searchText;
        }
    }
}
