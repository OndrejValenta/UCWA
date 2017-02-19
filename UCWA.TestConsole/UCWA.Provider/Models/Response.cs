using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;

namespace UCWA.Provider.Models
{
	public class Response<T> where T: class 
	{
		public T Data { get; set; }

		public HttpResponseHeaders ResponseHeaders { get; set; }

		public HttpStatusCode StatusCode { get; set; }

		public bool Successful { get; set; }
	}
}