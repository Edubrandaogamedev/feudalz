using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Asyncoroutine;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;
using JWT;
public class ServerRequest
{
    public class ContentTypes
    {
        public const string CONTENT_TYPE_JSON = "application/json";
    }
    public class RequestHeader
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public RequestHeader(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public override string ToString()
        {
            return $"Key: {Key} - Value: {Value}";
        }
    }
    public class Keys
    {
        public const string API_KEY_NAME = "";
        public const string API_KEY = "";
        public const string SECRET_KEY = "";
    }
    public class Payload
    {
        public string payload;
    }
    private static Payload GeneratePayload(object _o)
    {
        try
        {
            byte[] bytesToEncode = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_o));
            string encodedData = Convert.ToBase64String(bytesToEncode);
            var sendPayload = new Payload { payload = encodedData };
            return sendPayload;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            throw ex;
        }
    }
    private static string ConvertToBase64(string _data)
    {
        var dataBytes = Encoding.UTF8.GetBytes(_data);
        var encodedDataString = Convert.ToBase64String(dataBytes);
        return encodedDataString;
    }
    public static async Task<string> HTTPRequestGet(string uri, List<RequestHeader> customHeaders = null)
    {
        // Debug.Log("<color=#ffffffff>GetHTTPRequest  to  </color>" + uri);

        using (var request_ = UnityWebRequest.Get(uri))
        {
            return await HttpRequest(uri, request_, customHeaders);
        }
    }
    // public static async Task<string> HTTPRequestGet(string uri, RequestHeader customHeader = null)
    // {
    //    return await HTTPRequestGet(uri, new List<RequestHeader> { customHeader });
    // }
    public static async Task<string> HTTPRequestPost(string uri, object bodyData, List<RequestHeader> customHeaders = null, string contentType = ContentTypes.CONTENT_TYPE_JSON)
    {
        // Debug.Log("<color=#ffffffff>PostHTTPRequest to </color>" + uri);
        // Debug.Log(JsonConvert.SerializeObject(bodyData) + "\n");

        var jsonBodyData = JsonConvert.SerializeObject(bodyData);
        var bytesBodyData = Encoding.UTF8.GetBytes(jsonBodyData);
        using (var request_ = UnityWebRequest.Post(uri, jsonBodyData))
        {
            var uploadHandler = new UploadHandlerRaw(bytesBodyData) { contentType = contentType };
            request_.uploadHandler = uploadHandler;
            return await HttpRequest(uri, request_, customHeaders);
        }
    }
    public static async Task<string> HTTPRequestPost(string uri, object bodyData, RequestHeader customHeader = null, string contentType = ContentTypes.CONTENT_TYPE_JSON)
    {
        return await HTTPRequestPost(uri, bodyData, new List<RequestHeader> { customHeader }, contentType);
    }
    public static async Task<string> HTTPRequestPost(string uri, object bodyData)
    {
        return await HTTPRequestPost(uri, bodyData, new List<RequestHeader>(), ContentTypes.CONTENT_TYPE_JSON);
    }

    public static async Task<string> HTTPRequestPut(bool patchRequest, string uri, object bodyData, List<RequestHeader> customHeaders = null, string contentType = ContentTypes.CONTENT_TYPE_JSON)
    {
        // Debug.Log("<color=#ffffffff>PostHTTPRequest to </color>" + uri);
        // Debug.Log(JsonConvert.SerializeObject(bodyData) + "\n");
        var jsonBodyData = JsonConvert.SerializeObject(bodyData);
        var bytesBodyData = Encoding.UTF8.GetBytes(jsonBodyData);
        using (var request_ = UnityWebRequest.Put(uri, jsonBodyData))
        {
            if (patchRequest)
                request_.method = "PATCH";
            var uploadHandler = new UploadHandlerRaw(bytesBodyData) { contentType = contentType };
            request_.uploadHandler = uploadHandler;
            return await HttpRequest(uri, request_, customHeaders);
        }
    }
    public static async Task<string> HTTPRequestPut(bool patchRequest, string uri, object bodyData, RequestHeader customHeader = null, string contentType = ContentTypes.CONTENT_TYPE_JSON)
    {
        return await HTTPRequestPut(patchRequest, uri, bodyData, new List<RequestHeader> { customHeader }, contentType);
    }
    public static async Task<string> HTTPRequestDelete(string uri, List<RequestHeader> customHeaders = null, string contentType = ContentTypes.CONTENT_TYPE_JSON, object bodyData = null)
    {
        // Debug.Log("<color=#ffffffff>DeleteHTTPRequest to </color>" + uri + "<color=#f4b433ff> Body: </color>" + JsonConvert.SerializeObject(bodyData) + "\n");
        var jsonBodyData = JsonConvert.SerializeObject(bodyData);
        var bytesBodyData = Encoding.UTF8.GetBytes(jsonBodyData);
        using (var request_ = UnityWebRequest.Put(uri, jsonBodyData))
        {
            request_.method = "DELETE";
            var uploadHandler = new UploadHandlerRaw(bytesBodyData) { contentType = contentType };
            request_.uploadHandler = uploadHandler;
            return await HttpRequest(uri, request_, customHeaders);
        }
    }
    public static async Task<string> HTTPRequestDelete(string uri, RequestHeader customHeader = null, string contentType = ContentTypes.CONTENT_TYPE_JSON)
    {
        return await HTTPRequestDelete(uri, new List<RequestHeader> { customHeader }, contentType);
    }
    public static async Task<string> HTTPRequestDelete(string uri)
    {
        return await HTTPRequestDelete(uri, new List<RequestHeader>(), ContentTypes.CONTENT_TYPE_JSON);
    }
    public static async Task<string> HTTPRequestDelete(string uri, object bodyData, List<RequestHeader> customHeader = null)
    {
        return await HTTPRequestDelete(uri, customHeader, ContentTypes.CONTENT_TYPE_JSON, bodyData);
    }
    private static async Task<string> HttpRequest(string uri, UnityWebRequest _unityWebRequest, List<RequestHeader> customHeaders = null)
    {
        //customHeaders?.ToList().ForEach(r => Debug.Log(r.ToString()));

        using (var request_ = _unityWebRequest)
        {
            customHeaders?.ForEach(header => request_.SetRequestHeader(header.Key, header.Value));
            await request_.SendWebRequest();
            //Debug.Log("<color=#f4f433ff>Response Token: </color>" + request_.downloadHandler?.text + "<color=#f4b433ff>From: </color>" + uri);
            TryThrowAnyRequestException(request_);
            return request_.downloadHandler?.text ?? "";
        }
    }

    private static void TryThrowAnyRequestException(UnityWebRequest _request)
    {
        TryThrowExceptionForWebRequestErrorInResponseText(_request);
        var errorG = TryGetSclGenericError(_request);
        if (errorG != null)
            throw new SclErrorException(errorG);
    }
    private static void TryThrowExceptionForWebRequestErrorInResponseText(UnityWebRequest _request)
    {
        if (_request.result == UnityWebRequest.Result.ProtocolError || _request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError("WebException, HttpWebResponse " + _request.downloadHandler.text);
            throw new WebRequestException("HttpWebResponse " + _request.downloadHandler.text, WebRequestException.Status.Unkown);
        }
    }
    private static SclGenericError TryGetSclGenericError(UnityWebRequest request)
    {
        try
        {
            var error = JsonConvert.DeserializeObject<SclGenericError>(request.downloadHandler?.text);
            if (error.Error != null && error.Error != "")
            {
                //Debug.LogError(error);
                return error;
            }
        }
        catch (Exception ex)
        {
            //Debug.LogError(ex.Message);
        }
        return null;
    }
    public static T DecodeToken<T>(string token)
    {
        byte[] bytesToEncode = Encoding.UTF8.GetBytes(Keys.SECRET_KEY);
        string encodedSecret = Convert.ToBase64String(bytesToEncode);
        var decodedToken = JsonWebToken.Decode(token, Encoding.UTF8.GetBytes(encodedSecret), verify: false);

        // Debug.Log($"Decoded Token: <color=#44cc44ff>{(typeof(T)).ToString()} </color>" + decodedToken);

        return JsonConvert.DeserializeObject<T>(decodedToken);
    }
}
[Serializable]
[JsonObject(ItemRequired = Required.Always)]
public class SclErrorException : Exception
{
    public SclGenericError Error { get; set; }

    public SclErrorException(SclGenericError _error)
    {
        Error = _error;
    }

    public override string Message => Error.Error;
}
[Serializable]
[JsonObject(ItemRequired = Required.Always)]
public class SclGenericError
{
    public string Error { get; set; }

    public override string ToString()
    {
        return $"Error: {Error}";
    }
}
[Serializable]
[JsonObject(ItemRequired = Required.Always)]
public class WebRequestException : Exception
{
    public enum Status { BadRequest, InternalServerError, GatewayTimeout, Unauthorized, Unkown }
    public Status ExceptionStatus { get; private set; }
    public WebRequestException(string message, Status status) : base(message)
    {
        ExceptionStatus = status;
    }
}
