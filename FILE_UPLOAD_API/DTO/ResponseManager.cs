using System.Net;

namespace FILE_UPLOAD_API.DTO
{
    public class ResponseManager
    {
        public bool Succeded => (!Errors.Any() && !Warnings.Any());
        public int? ErrorQuery { get; set; }
        public long? Identity { get; set; }
        public Guid? GuidReturn { get; set; }
        public List<string> Errors { get; set; } = new List<string>(0);
        public List<string> Warnings { get; set; } = new List<string>(0);

        public int? StatusCode{ get; set;}
    }

    public class ResponseManager<T> : ResponseManager where T : class
    {
        public IEnumerable<T> DataList { get; set; } = Enumerable.Empty<T>();
        public T? SingleData { get; set; }
    }

}
