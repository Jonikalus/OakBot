using RestSharp;

namespace OakBot.Helpers
{
    // @author gibletto
    public static class RequestExtensions
    {
        #region Public Methods

        public static void AddSafeParameter(this IRestRequest request, string parameter, object value)
        {
            if (!string.IsNullOrEmpty(parameter) && value != null)
            {
                request.AddParameter(parameter, value);
            }
        }

        #endregion Public Methods
    }
}