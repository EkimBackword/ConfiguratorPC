using ApiServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ApiServer.Controllers
{
    public class PowerController : ApiController
    {
        DatabaseEntities db = new DatabaseEntities();
        // GET: api/Power
        public IEnumerable<string> Get()
        {
            List<string> result = new List<string>();
            Entities e = db.Entities.Where(x => x.NameOfEntity == "Блок питания").Single();

            if (e.Name1 != null) result.Add(e.Name1);
            if (e.Name1 != null) result.Add(e.Name2);
            if (e.Name1 != null) result.Add(e.Name3);
            if (e.Name1 != null) result.Add(e.Name4);
            if (e.Name1 != null) result.Add(e.Name5);
            if (e.Name1 != null) result.Add(e.Name6);
            if (e.Name1 != null) result.Add(e.Name7);
            if (e.Name1 != null) result.Add(e.Name8);
            if (e.Name1 != null) result.Add(e.Name9);
            if (e.Name1 != null) result.Add(e.Name10);
            if (e.Name1 != null) result.Add(e.Name11);
            if (e.Name1 != null) result.Add(e.Name12);
            if (e.Name1 != null) result.Add(e.Name13);
            if (e.Name1 != null) result.Add(e.Name14);
            if (e.Name1 != null) result.Add(e.Name15);

            return result;
        }

        // POST: api/Power
        public IEnumerable<IEnumerable<string>> Post([FromBody]string value)
        {
            var v = from entity in db.Entities
                    from device in db.Devices
                    from characteristic in db.Characteristic
                    where entity.Id == device.IdEntity &&
                    entity.NameOfEntity == "Блок питания" &&
                    characteristic.IdCharacteristic == device.IdCharacteristic
                    select new
                    {
                        device.BrandName,
                        device.Model,
                        characteristic.Value1,
                        characteristic.Value2,
                        characteristic.Value3,
                        characteristic.Value4,
                        characteristic.Value5
                    };

            List<List<string>> t = new List<List<string>>();

            foreach (var i in v)
            {
                List<string> d = new List<string>();
                d.Add(i.BrandName);
                d.Add(i.Model);
                d.Add(i.Value1);
                d.Add(i.Value2);
                d.Add(i.Value3);
                d.Add(i.Value4);
                d.Add(i.Value5);

                t.Add(d);
            }

            return t;
        }
    }
}
