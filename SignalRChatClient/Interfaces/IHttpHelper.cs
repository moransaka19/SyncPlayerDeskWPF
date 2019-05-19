namespace SyncPlayer.Interfaces
{
    public interface IHttpHelper
    {
        #region Public Methods

        TAnswer Request<TAnswer, TModel>(TModel bodyObject, string requestMethod, string barearToken, string requestType, string contentType, string encoding) where TAnswer : class where TModel : class;
        bool Request<TModel>(TModel bodyObject, string requestMethod, string barearToken, string requestType, string contentType, string encoding) where TModel : class;

        #endregion Public Methods
    }
}