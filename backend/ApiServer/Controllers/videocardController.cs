﻿using ApiServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ApiServer.Controllers
{
    public class videocardController : ApiController
    {
        DatabaseEntities db = new DatabaseEntities();
        // GET: api/videocard
        public IEnumerable<string> Get()
        {
            List<string> result = new List<string>();
            Entities e = db.Entities.Where(x => x.NameOfEntity == "Видеокарта").Single();

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

        // POST: api/videocard
        public IEnumerable<IEnumerable<string>> Post(JsonDataModel value)
        {
            var v = from entity in db.Entities
                    from device in db.Devices
                    from characteristic in db.Characteristic
                    where entity.Id == device.IdEntity &&
                    entity.NameOfEntity == "Видеокарта" &&
                    characteristic.IdCharacteristic == device.IdCharacteristic
                    select new
                    {
                        device.IdDevice,
                        device.BrandName,
                        device.Model,
                        characteristic.Value1,
                        characteristic.Value2,
                        characteristic.Value3,
                        characteristic.Value4
                    };

            List<List<string>> t = new List<List<string>>();

            foreach (var i in v)
            {
                List<string> d = new List<string>();
                d.Add(i.IdDevice.ToString());
                d.Add(i.BrandName);
                d.Add(i.Model);
                d.Add(i.Value1);
                d.Add(i.Value2);
                d.Add(i.Value3);
                d.Add(i.Value4);

                t.Add(d);
            }

            return t;
        }
    }
}
