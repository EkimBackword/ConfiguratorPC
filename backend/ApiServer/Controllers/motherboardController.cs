using ApiServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace ApiServer.Controllers
{
    public class motherboardController : ApiController
    {
        DatabaseEntities db = new DatabaseEntities();
        // GET: api/motherboard
        public IEnumerable<string> Get()
        {
            List<string> result = new List<string>();
            Entities e = db.Entities.Where(x => x.NameOfEntity == "Материнская плата").Single();

            if (e.Name1 != null) result.Add(e.Name1);
            if (e.Name2 != null) result.Add(e.Name2);
            if (e.Name3 != null) result.Add(e.Name3);
            if (e.Name4 != null) result.Add(e.Name4);
            if (e.Name5 != null) result.Add(e.Name5);
            if (e.Name6 != null) result.Add(e.Name6);
            if (e.Name7 != null) result.Add(e.Name7);
            if (e.Name8 != null) result.Add(e.Name8);
            if (e.Name9 != null) result.Add(e.Name9);
            if (e.Name10 != null) result.Add(e.Name10);
            if (e.Name11 != null) result.Add(e.Name11);
            if (e.Name12 != null) result.Add(e.Name12);
            if (e.Name13 != null) result.Add(e.Name13);
            if (e.Name14 != null) result.Add(e.Name14);
            if (e.Name15 != null) result.Add(e.Name15);

            return result;
        }

        // POST: api/motherboard
        public IEnumerable<IEnumerable<string>> Post(JsonDataModel value)
        {
            var v = from entity in db.Entities
                    from device in db.Devices
                    from characteristic in db.Characteristic
                    where entity.Id == device.IdEntity &&
                    entity.NameOfEntity == "Материнская плата" &&
                    characteristic.IdCharacteristic == device.IdCharacteristic
                    select new
                    {
                        device.IdDevice,
                        device.BrandName,
                        device.Model,
                        characteristic.Value1,
                        characteristic.Value2,
                        characteristic.Value3,
                        characteristic.Value4,
                        characteristic.Value5,
                        characteristic.Value6
                    };

            List<List<string>> t = new List<List<string>>();

            string[] s1 = null;

            if (value.CPU != null || value.coolingSystem != null)
            {
                s1 = (from dtt in db.DeviceToType
                      from tp in db.Types
                      where dtt.IdDevice == (value.CPU ?? value.coolingSystem) && dtt.IdType == tp.IdType
                      select tp.Name).ToArray();
            }

            string s2 = null;
            if (value.body != null)
                s2 = (from d in db.Devices
                     from c in db.Characteristic
                     where d.IdDevice == value.body && d.IdCharacteristic == c.IdCharacteristic
                     select c.Value5).Single();

            string[] s3 = null;
            if (value.RAM != null)
                s3 = (from dtt in db.DeviceToType
                     from tp in db.Types
                     where dtt.IdDevice == value.RAM && dtt.IdType == tp.IdType
                     select tp.Name).ToArray();

            foreach (var i in v)
            {
                if (value.CPU != null || value.coolingSystem != null)
                {
                    string[] tmp = (from dtt in db.DeviceToType
                                    from tp in db.Types
                                    where dtt.IdDevice == i.IdDevice && dtt.IdType == tp.IdType
                                    select tp.Name).ToArray();
                    string st = null;

                    for (int j = 0; j < tmp.Length; j++)
                    {
                        if (Regex.IsMatch(s1[j], @"Socket-\w+"))
                        {
                            st = tmp[j];
                            break;
                        }
                    }

                    if (Array.IndexOf(s1, st) == -1) continue;
                }

                if (value.body != null)
                {
                    string tmp = (from dr in db.Devices
                                  from c in db.Characteristic
                                  where dr.IdDevice == i.IdDevice && dr.IdCharacteristic == c.IdCharacteristic
                                  select c.Value6).Single();

                    if (!s2.Equals(tmp)) continue;
                }

                if (value.RAM != null)
                {
                    string tmp = (from dtt in db.DeviceToType
                                  from tp in db.Types
                                  where dtt.IdDevice == i.IdDevice && dtt.IdType == tp.IdType
                                  select tp.Name).Single();

                    if (Array.IndexOf(s3, tmp) == -1) continue;
                }

                List<string> d = new List<string>();
                d.Add(i.IdDevice.ToString());
                d.Add(i.BrandName);
                d.Add(i.Model);
                d.Add(i.Value1);
                d.Add(i.Value2);
                d.Add(i.Value3);
                d.Add(i.Value4);
                d.Add(i.Value5);
                d.Add(i.Value6);

                t.Add(d);
            }

            return t;
        }
    }
}
