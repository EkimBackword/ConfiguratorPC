using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace ApiServer.Controllers
{
    public class UploadController : ApiController
    {
        // POST: api/Upload
        public HttpResponseMessage Post()
        {
            var file = HttpContext.Current.Request.Files[0];

            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/uploads"), fileName);
                file.SaveAs(path);
                var content = JsonConvert.SerializeObject(fileName, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(content, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {
                var response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                response.Content = new StringContent("error!!!", Encoding.UTF8, "application/json");
                return response;
            }
        }
    }
}
