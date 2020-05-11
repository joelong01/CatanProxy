using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System;


namespace Catan.Proxy
{
    public class SessionInfo
    {
        public string Description { get; set; }
        public string Id { get; set; }
        public string Creator { get; set; }
    }
    
    public enum CatanError
    {
        DevCardsSoldOut,
        NoMoreResource,
        LimitExceeded,
        NoGameWithThatName,
        NoPlayerWithThatName,
        NotEnoughResourcesToPurchase,
        MissingData,
        BadTradeResources,
        NoResource,
        BadEntitlement,
        BadParameter,
        BadLogRecord,
        PlayerAlreadyRegistered,
        GameAlreadStarted,
        Unknown,
        InsufficientResource,
        Unexpected,
        NoError,
    }

    /// <summary>
    ///  This is the class that we send to the service to synchronize state.
    ///  it is Deserialized in the service.
    ///  
    ///  Data is a LogHeader of some type
    ///  TypeName is the name of the derived LogHeader type
    ///  Sequence is set by the service and is the order of the log it has received
    ///  
    /// </summary>
    public class CatanMessage
    {
        private object _data;
        

        public object Data
        {
            get => _data;
            set
            {
                _data = value;
                TypeName = value.GetType().FullName;
            }
        }
        public string TypeName { get; set; } = "";
        public int Sequence { get; set; } = 0;
        public string Origin { get; set; } = "";

        public CatanMessage() { }

    }

    public class CatanRequest
    {
        public string Url { get; set; } = "";
        public object Body { get; set; } = null;

        public CatanRequest() { }
        public CatanRequest(string u, object b) { Url = u; Body = b; }
        public override string ToString()
        {
            return $"[Url={Url}][Body={Body?.ToString()}]";
        }
    }

    public class CatanResult
    {
        private CatanRequest _request = new CatanRequest();
        private string request;

        public CatanRequest CantanRequest
        {
            get
            {
                return _request;
            }
            set
            {
                if (value != _request)
                {
                    _request = value;
                }
            }
        }

        public List<KeyValuePair<string, object>> ExtendedInformation { get; } = new List<KeyValuePair<string, object>>();
        public string Description { get; set; }
        public string FunctionName { get; set; }
        public string FilePath { get; set; }
        public int LineNumber { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
        public string Request { get => _request.Url; set => request = value; }
        public Guid ID { get; set; } = Guid.NewGuid(); // this gives us an ID at creation time that survives serialization and is globally unique
        public CatanError Error { get; set; } = CatanError.Unknown;
        public string Version { get; set; } = "2.0";
        public CatanResult() // for the Serializer
        {

        }

        public CatanResult(CatanError error, [CallerMemberName] string fName = "", [CallerFilePath] string codeFile = "", [CallerLineNumber] int lineNumber = -1)
        {
            Error = error;
            FunctionName = fName;
            FilePath = codeFile;
            LineNumber = lineNumber;
        }
        public static bool operator ==(CatanResult a, CatanResult b)
        {
            if (a is null || b is null)
            {
                if (b is null && a is null)
                {
                    return true;
                }

                return false;
            }

            if (a.ExtendedInformation?.Count != b.ExtendedInformation?.Count)
            {
                return false;
            }
            if (a.ExtendedInformation != null)
            {
                if (b.ExtendedInformation == null) return false;
                for (int i = 0; i < a.ExtendedInformation?.Count; i++)
                {
                    if (a.ExtendedInformation[i].Key != b.ExtendedInformation[i].Key)
                    {
                        return false;
                    }

                    //
                    //  going to ignore the value for now
                }
            }

            return
                (
                    a.Description == b.Description &&
                    a.FunctionName == b.FunctionName &&
                    a.FilePath == b.FilePath &&
                    a.LineNumber == b.LineNumber &&
                    a.Request == b.Request &&
                    a.Error == b.Error
                 );

        }
        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 97 + Description.GetHashCode();
            hash = hash * 97 + FunctionName.GetHashCode();
            hash = hash * 97 + FilePath.GetHashCode();
            hash = hash * 97 + LineNumber.GetHashCode();
            hash = hash * 97 + Request.GetHashCode();
            hash = hash * 97 + Error.GetHashCode();
            return hash;
        }
        public static bool operator !=(CatanResult a, CatanResult b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            return (CatanResult)obj == this;
        }
    }

}
