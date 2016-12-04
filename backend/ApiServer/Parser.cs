using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Text.RegularExpressions;
using ApiServer.Models;

namespace ApiServer
{
    class Parser
    {
        DatabaseEntities db;
        static Parser instance = null;

        public static Parser GetInstance()
        {
            if (instance == null) instance = new Parser();
            return instance;
        }

        private Parser()
        {
            db = new DatabaseEntities();
        }

        XSSFWorkbook priceList;

        public void Read(string path, string sheetName)
        {
            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                priceList = new XSSFWorkbook(file);
            }

            var sheet = priceList.GetSheet(sheetName);

            for (int row = 12; row <= sheet.LastRowNum; row++)
            {
                var currentRow = sheet.GetRow(row);

                bool isToDB = true; //нужно ли просматривать (добавлять) информацию из данной строки currentRow в БД
                string group2 = "";
                string group3 = "";
                string brend = "";
                string number = "";
                string addnumber = "";
                string code = "";
                string info = "";
                string price = "";
                string volume = "";
                string weight = "";

                for (int column = 0; column < 23; column++)
                {
                    string stringCellValue = "";

                    try
                    {
                        stringCellValue = currentRow.GetCell(column).StringCellValue;
                    }
                    catch (InvalidOperationException)
                    {
                        stringCellValue = currentRow.GetCell(column).NumericCellValue.ToString();
                    }
                    catch { }

                    if (column == 0 && !stringCellValue.Equals("КОМПЛЕКТУЮЩИЕ ДЛЯ КОМПЬЮТЕРОВ")) return;
                    if (column == 1)
                    {
                        if (stringCellValue.Equals("Аксессуары") || stringCellValue.Equals("Кабели и шлейфы") || stringCellValue.Equals("Устройства охлаждения") || stringCellValue.Equals("Товар под заказ"))
                        {
                            isToDB = false;
                            break;
                        }

                        group2 = stringCellValue;
                    }
                    if (column == 2)
                    {
                        if (stringCellValue.Equals("") || stringCellValue.Equals("TV-Tuner"))
                        {
                            isToDB = false;
                            break;
                        }
                        else group3 = stringCellValue;
                    }
                    if (column == 3)
                    {
                        if (stringCellValue.Equals(""))
                        {
                            isToDB = false;
                            break;
                        }
                        else brend = stringCellValue;
                    }
                    if (column == 4) number = stringCellValue;
                    if (column == 5) addnumber = stringCellValue;
                    if (column == 6) code = stringCellValue;
                    if (column == 7) info = stringCellValue;
                    if (column == 10) price = stringCellValue;
                    if (column == 17) volume = stringCellValue;
                    if (column == 18) weight = stringCellValue;
                }

                if (!isToDB) continue;

                if(db.Devices.Count(x => x.BrandName == brend && x.Model == code) > 0) continue;

                //TODO: Ниже место для кода парсинга перменной name
                Regex reg = new Regex(@"\w+");
                MatchCollection mc = reg.Matches(info);
                string[] m = new string[mc.Count];

                for (int l = 0; l < mc.Count; l++)
                {
                    m[l] = mc[l].ToString();
                }
                
                if(group2.Equals("Блоки питания"))
                {
                    string type = "Блок питания";
                    string addInfo = "";
                    int countSata = 0;
                    int i = Array.IndexOf(m, group3) + 1;
                    int w = 0;
                    for (; i < m.Length; i++)
                    {
                        if(Regex.IsMatch(m[i], @"(\d+)W"))
                        {
                           w = int.Parse(Regex.Replace(m[i], "W", ""));
                        }
                        if (Regex.IsMatch(m[i], @"(\d+)xSATA"))
                        {
                            countSata = int.Parse(Regex.Replace(m[i], "xSATA", ""));
                        }
                        else
                        {
                            addInfo += m[i] + " ";
                        }
                    }
                    //
                    CheckType("SATA");

                    if(db.Entities.Count(x => x.NameOfEntity == type) == 0)
                    {
                        Entities e = new Entities
                        {
                            NameOfEntity = type,
                            Name1 = "Power",
                            Name2 = "Info",  
                            Name3 = "Price", 
                            Name4 = "Volume",
                            Name5 = "Weight"        
                        };

                        db.Entities.Add(e);

                        db.SaveChanges();        
                    }

                    Characteristic c = new Characteristic
                    {
                        Value1 = w.ToString(),
                        Value2 = info,
                        Value3 = price,
                        Value4 = volume,
                        Value5 = weight
                    };

                    db.Characteristic.Add(c);

                    db.SaveChanges();

                    Devices d = new Devices
                    {
                        IdEntity = db.Entities.First(x => x.NameOfEntity == type).Id,
                        IdCharacteristic = db.Characteristic.First(x => x.Value2 == info).IdCharacteristic,
                        BrandName = brend,
                        Model = code
                    };

                    db.Devices.Add(d);

                    db.SaveChanges();

                    SetTypesForDevice(brend, code, "SATA", countSata);
                }
                else if (group2.Equals("Видеокарты"))
                {
                    string type = "Видеокарта";
                    string socket = "";
                    string addInfo = "";
                    int i = Array.IndexOf(m, group3) + reg.Matches(code).Count;
                    string name = m[i++];
                    addInfo += name;

                    for (; i < m.Length; i++)
                    {
                        if (Regex.IsMatch(m[i], @"G?DDR(\d)X?"))
                        {
                            socket = m[i];
                        }
                        else
                        {
                            addInfo += m[i] + " ";
                        }
                    }
                    //
                    CheckType(socket);

                    if (db.Entities.Count(x => x.NameOfEntity == type) == 0)
                    {
                        Entities e = new Entities
                        {
                            NameOfEntity = type,
                            Name1 = "Info",
                            Name2 = "Price",
                            Name3 = "Volume",
                            Name4 = "Weight"
                        };

                        db.Entities.Add(e);

                        db.SaveChanges();
                    }

                    Characteristic c = new Characteristic
                    {
                        Value1 = info,
                        Value2 = price,
                        Value3 = volume,
                        Value4 = weight
                    };

                    db.Characteristic.Add(c);

                    db.SaveChanges();

                    Devices d = new Devices
                    {
                        IdEntity = db.Entities.First(x => x.NameOfEntity == type).Id,
                        IdCharacteristic = db.Characteristic.First(x => x.Value1 == info).IdCharacteristic,
                        BrandName = brend,
                        Model = code
                    };

                    db.Devices.Add(d);

                    db.SaveChanges();

                    SetTypesForDevice(brend, code, socket, 1);
                }
                else if (group2.Equals("Флоппи-дисководы"))
                {
                    string type = "Флоппи-дисковод";
                    string addInfo = "";
                    int i = 1;
                    string socket = m[i++];

                    for (; i < m.Length; i++)
                    {
                        addInfo += m[i] + " ";
                    }
                    //
                    CheckType(socket);

                    if (db.Entities.Count(x => x.NameOfEntity == type) == 0)
                    {
                        Entities e = new Entities
                        {
                            NameOfEntity = type,
                            Name1 = "Info",
                            Name2 = "Price",
                            Name3 = "Volume",
                            Name4 = "Weight"
                        };

                        db.Entities.Add(e);

                        db.SaveChanges();
                    }

                    Characteristic c = new Characteristic
                    {
                        Value1 = info,
                        Value2 = price,
                        Value3 = volume,
                        Value4 = weight
                    };

                    db.Characteristic.Add(c);

                    db.SaveChanges();

                    Devices d = new Devices
                    {
                        IdEntity = db.Entities.First(x => x.NameOfEntity == type).Id,
                        IdCharacteristic = db.Characteristic.First(x => x.Value1 == info).IdCharacteristic,
                        BrandName = brend,
                        Model = code
                    };

                    db.Devices.Add(d);

                    db.SaveChanges();

                    SetTypesForDevice(brend, code, socket, 1);
                }
                else if (group2.Equals("Жесткие Диски"))
                {
                    string type = "Жесткий диск";
                    string addInfo = "";
                    int i = 3;
                    string socket = "";
                    double d = 0;

                    for (; i < m.Length; i++)
                    {
                        if (Regex.IsMatch(m[i], @"SATA-(I{1,3})"))
                        {
                            socket = m[i++];
                            break;
                        }
                        else if(m[i].Equals("SAS"))
                        {
                            socket = m[i++];

                            if(Regex.IsMatch(m[i], @"\d\.\d")) socket += " " + m[i++];

                            break;
                        }
                    }

                    for (; i < m.Length; i++)
                    {
                        if (Regex.IsMatch(m[i], "\\d\\.\\d\""))
                        {
                            d = int.Parse(Regex.Replace(m[i], "\"", ""));
                        }
                        else
                        {
                            addInfo += m[i] + " ";
                        }
                    }
                    //
                    CheckType(socket);

                    if (db.Entities.Count(x => x.NameOfEntity == type) == 0)
                    {
                        Entities e = new Entities
                        {
                            NameOfEntity = type,
                            Name1 = "Info",
                            Name2 = "Price",
                            Name3 = "Volume",
                            Name4 = "Weight",
                            Name5 = "Size"
                        };

                        db.Entities.Add(e);

                        db.SaveChanges();
                    }

                    Characteristic c = new Characteristic
                    {
                        Value1 = info,
                        Value2 = price,
                        Value3 = volume,
                        Value4 = weight,
                        Value5 = d.ToString()
                    };

                    db.Characteristic.Add(c);

                    db.SaveChanges();

                    Devices dev = new Devices
                    {
                        IdEntity = db.Entities.First(x => x.NameOfEntity == type).Id,
                        IdCharacteristic = db.Characteristic.First(x => x.Value1 == info).IdCharacteristic,
                        BrandName = brend,
                        Model = code
                    };

                    db.Devices.Add(dev);

                    db.SaveChanges();

                    SetTypesForDevice(brend, code, socket, 1);
                }
                else if (group2.Equals("Звуковые Карты"))
                {
                    string type = "Звуковая карта";
                    
                    string addInfo = "";
                    int i = 3;
                    string socket = "";

                    for (; i < m.Length; i++)
                    {
                        if (!group2.Equals("Другое") && Regex.IsMatch(m[i], @"PCI(-E)?|USB"))
                        {
                            socket = m[i++];
                            break;
                        }
                        else if (group2.Equals("Другое"))
                        {
                            addInfo += m[i] + " ";
                        }
                    }

                    for (; i < m.Length; i++)
                    {
                        addInfo += m[i] + " ";
                    }
                    //
                    CheckType(socket);

                    if (db.Entities.Count(x => x.NameOfEntity == type) == 0)
                    {
                        Entities e = new Entities
                        {
                            NameOfEntity = type,
                            Name1 = "Info",
                            Name2 = "Price",
                            Name3 = "Volume",
                            Name4 = "Weight"
                        };

                        db.Entities.Add(e);

                        db.SaveChanges();
                    }

                    Characteristic c = new Characteristic
                    {
                        Value1 = info,
                        Value2 = price,
                        Value3 = volume,
                        Value4 = weight
                    };

                    db.Characteristic.Add(c);

                    db.SaveChanges();

                    Devices d = new Devices
                    {
                        IdEntity = db.Entities.First(x => x.NameOfEntity == type).Id,
                        IdCharacteristic = db.Characteristic.First(x => x.Value1 == info).IdCharacteristic,
                        BrandName = brend,
                        Model = code
                    };

                    db.Devices.Add(d);

                    db.SaveChanges();

                    SetTypesForDevice(brend, code, socket, 1);
                }
                else if (group2.Equals("Контроллеры"))
                {
                    string type = "Контроллер";
                    string addInfo = "";
                    int countSata = 0;
                    int countIde = 0;
                    int countLtp = 0;
                    int countCom = 0;
                    int countUsb2 = 0;
                    int countUsb3 = 0;
                    int countI4p = 0;
                    int countI6p = 0;
                    int i = 1;
                    string socket = m[i++];

                    if (group3.Equals("Internal Card Reader")) { socket = m[4]; i = 5; }

                    for (; i < m.Length; i++)
                    {
                        if (Regex.IsMatch(m[i], @"(\d+)xIDE"))
                        {
                            countIde = int.Parse(Regex.Replace(m[i], "xIDE", ""));
                        }
                        else if (Regex.IsMatch(m[i], @"(\d+)xSATA"))
                        {
                            countSata = int.Parse(Regex.Replace(m[i], "xSATA", ""));
                        }
                        else if (Regex.IsMatch(m[i], @"(\d+)xLTP"))
                        {
                            countLtp = int.Parse(Regex.Replace(m[i], "xLTP", ""));
                        }
                        else if (Regex.IsMatch(m[i], @"(\d+)xCOM"))
                        {
                            countCom = int.Parse(Regex.Replace(m[i], "xCOM", ""));
                        }
                        else if (Regex.IsMatch(m[i], @"(\d+)xUSB2\.0"))
                        {
                            countUsb2 = int.Parse(Regex.Replace(m[i], "xUSB2\\.0", ""));
                        }
                        else if (Regex.IsMatch(m[i], @"(\d+)xUSB3\.0"))
                        {
                            countUsb3 = int.Parse(Regex.Replace(m[i], "xUSB3\\.0", ""));
                        }
                        else if (Regex.IsMatch(m[i], @"(\d+)xIEEE1394\(4p\)"))
                        {
                            countI4p = int.Parse(Regex.Replace(m[i], "IEEE1394\\(4p\\)", ""));
                        }
                        else if (Regex.IsMatch(m[i], @"(\d+)xIEEE1394\(6p\)"))
                        {
                            countI6p = int.Parse(Regex.Replace(m[i], "IEEE1394\\(6p\\)", ""));
                        }
                        else
                        {
                            addInfo += m[i] + " ";
                        }
                    }
                    //
                    CheckType(socket);
                    CheckType("SATA");
                    CheckType("IDE");
                    CheckType("LTP");
                    CheckType("COM");
                    CheckType("USB2.0");
                    CheckType("USB3.0");
                    CheckType("IEEE1394(4p)");
                    CheckType("IEEE1394(6p)");

                    if (db.Entities.Count(x => x.NameOfEntity == type) == 0)
                    {
                        Entities e = new Entities
                        {
                            NameOfEntity = type,
                            Name1 = "Info",
                            Name2 = "Price",
                            Name3 = "Volume",
                            Name4 = "Weight"
                        };

                        db.Entities.Add(e);

                        db.SaveChanges();
                    }

                    Characteristic c = new Characteristic
                    {
                        Value1 = info,
                        Value2 = price,
                        Value3 = volume,
                        Value4 = weight
                    };

                    db.Characteristic.Add(c);

                    db.SaveChanges();

                    Devices d = new Devices
                    {
                        IdEntity = db.Entities.First(x => x.NameOfEntity == type).Id,
                        IdCharacteristic = db.Characteristic.First(x => x.Value1 == info).IdCharacteristic,
                        BrandName = brend,
                        Model = code
                    };

                    db.Devices.Add(d);

                    db.SaveChanges();

                    SetTypesForDevice(brend, code, socket, 1);
                    if(countSata > 0) SetTypesForDevice(brend, code, "SATA", countSata);
                    if(countIde > 0) SetTypesForDevice(brend, code, "IDE", countIde);
                    if(countLtp > 0) SetTypesForDevice(brend, code, "LTP", countLtp);
                    if(countCom > 0) SetTypesForDevice(brend, code, "COM", countCom);
                    if(countUsb2 > 0) SetTypesForDevice(brend, code, "USB2.0", countUsb2);
                    if(countUsb3 > 0) SetTypesForDevice(brend, code, "USB3.0", countUsb3);
                    if(countI4p > 0) SetTypesForDevice(brend, code, "IEEE1394(4p)", countI4p);
                    if(countI6p > 0) SetTypesForDevice(brend, code, "IEEE1394(6p)", countI6p);
                }
                else if (group2.Equals("Корпуса"))
                {
                    string type = "Корпус";
                    string addInfo = "";
                    int countSata = 0;
                    int countUsb2 = 0;
                    int countUsb3 = 0;
                    int i = 1 + reg.Matches(brend).Count + reg.Matches(code).Count;
                    
                    for (; i < m.Length; i++)
                    {
                        if (Regex.IsMatch(m[i], @"(\d+)xE-SATA"))
                        {
                            countSata = int.Parse(Regex.Replace(m[i], "xE-SATA", ""));
                        }
                        else if (Regex.IsMatch(m[i], @"(\d+)xUSB2\.0"))
                        {
                            countUsb2 = int.Parse(Regex.Replace(m[i], "xUSB2\\.0", ""));
                        }
                        else if (Regex.IsMatch(m[i], @"(\d+)xUSB3\.0"))
                        {
                            countUsb3 = int.Parse(Regex.Replace(m[i], "xUSB3\\.0", ""));
                        }
                        else
                        {
                            addInfo += m[i] + " ";
                        }
                    }
                    //
                    CheckSocket("E-SATA");
                    CheckSocket("USB2.0");
                    CheckSocket("USB3.0");

                    if (db.Entities.Count(x => x.NameOfEntity == type) == 0)
                    {
                        Entities e = new Entities
                        {
                            NameOfEntity = type,
                            Name1 = "Info",
                            Name2 = "Price",
                            Name3 = "Volume",
                            Name4 = "Weight",
                            Name5 = "Type"
                        };

                        db.Entities.Add(e);

                        db.SaveChanges();
                    }

                    Characteristic c = new Characteristic
                    {
                        Value1 = info,
                        Value2 = price,
                        Value3 = volume,
                        Value4 = weight,
                        Value5 = group3
                    };

                    db.Characteristic.Add(c);

                    db.SaveChanges();

                    Devices d = new Devices
                    {
                        IdEntity = db.Entities.First(x => x.NameOfEntity == type).Id,
                        IdCharacteristic = db.Characteristic.First(x => x.Value1 == info).IdCharacteristic,
                        BrandName = brend,
                        Model = code
                    };

                    db.Devices.Add(d);

                    db.SaveChanges();
                    
                    if (countSata > 0) SetSocketsForDevice(brend, code, "SATA", countSata);
                    if (countUsb2 > 0) SetSocketsForDevice(brend, code, "USB2.0", countUsb2);
                    if (countUsb3 > 0) SetSocketsForDevice(brend, code, "USB3.0", countUsb3);
                }
                else if (group2.Equals("Материнские Платы"))
                {
                    string type = "Материнская плата";
                    string addInfo = "";
                    string iora = "";
                    bool isAtx = false;
                    bool isMAtx = false;
                    bool isMiniItx = false;
                    int countDdr3 = 0;
                    int countDdr4 = 0;
                    int i = 2 + reg.Matches(brend).Count + reg.Matches(code).Count;

                    if (Array.IndexOf(m, "Intel") != -1)
                    {
                        iora = "Intel";
                        i = Array.IndexOf(m, "Intel");
                    }
                    else if (Array.IndexOf(m, "AMD") != -1)
                    {
                        iora = "AMD";
                        i = Array.IndexOf(m, "AMD");
                    }
                    else if (Array.IndexOf(m, "nVidia") != -1)
                    {
                        iora = "nVidia";
                        i = Array.IndexOf(m, "nVidia");
                    }


                    for (; i < m.Length; i++)
                    {
                        if (m[i].Equals("ATX"))
                        {
                            isAtx = true;
                        }
                        else if(m[i].Equals("mATX"))
                        {
                            isMAtx = true;
                        }
                        else if (m[i].Equals("mini-ITX"))
                        {
                            isMiniItx = true;
                        }
                        else if (Regex.IsMatch(m[i], @"(\d+)xDDR3"))
                        {
                            countDdr3 = int.Parse(Regex.Replace(m[i], "xDDR3", ""));
                        }
                        else if (Regex.IsMatch(m[i], @"(\d+)xDDR4"))
                        {
                            countDdr4 = int.Parse(Regex.Replace(m[i], "xDDR4", ""));
                        }
                        else
                        {
                            addInfo += m[i] + " ";
                        }
                    }
                    //
                    CheckType(group3);
                    CheckType("DDR3");
                    CheckType("DDR4");

                    if (db.Entities.Count(x => x.NameOfEntity == type) == 0)
                    {
                        Entities e = new Entities
                        {
                            NameOfEntity = type,
                            Name1 = "Info",
                            Name2 = "Price",
                            Name3 = "Volume",
                            Name4 = "Weight",
                            Name5 = "Producer",
                            Name6 = "CorpType"
                        };

                        db.Entities.Add(e);

                        db.SaveChanges();
                    }

                    Characteristic c = new Characteristic
                    {
                        Value1 = info,
                        Value2 = price,
                        Value3 = volume,
                        Value4 = weight,
                        Value5 = iora,
                        Value6 = isAtx ? "ATX" : (isMAtx ? "mATX" : "mini-ITX")
                    };

                    db.Characteristic.Add(c);

                    db.SaveChanges();

                    Devices d = new Devices
                    {
                        IdEntity = db.Entities.First(x => x.NameOfEntity == type).Id,
                        IdCharacteristic = db.Characteristic.First(x => x.Value1 == info).IdCharacteristic,
                        BrandName = brend,
                        Model = code
                    };

                    db.Devices.Add(d);

                    db.SaveChanges();

                    SetTypesForDevice(brend, code, group3, 1);
                    if (countDdr3 > 0) SetTypesForDevice(brend, code, "DDR3", countDdr3);
                    if (countDdr4 > 0) SetTypesForDevice(brend, code, "DDR4", countDdr4);
                }
                else if (group2.Equals("Память оперативная"))
                {
                    string type = "Память оперативная";
                    bool isDdr2 = false;
                    bool isDdr3 = false;
                    bool isDdr3l = false;
                    bool isDdr4 = false;

                    if (Array.IndexOf(m, "DDR2") != -1)
                    {
                        isDdr2 = true;
                    }
                    else if (Array.IndexOf(m, "DDR3") != -1)
                    {
                        isDdr3 = true;
                    }
                    else if (Array.IndexOf(m, "DDR3L") != -1)
                    {
                        isDdr3l = true;
                    }
                    else if (Array.IndexOf(m, "DDR4") != -1)
                    {
                        isDdr4 = true;
                    }
                    //
                    string tmp = isDdr2 ? "DDR2" : (isDdr3 ? "DDR3" : (isDdr3l ? "DDR3L" : "DDR4"));
                    CheckType(tmp);

                    if (db.Entities.Count(x => x.NameOfEntity == type) == 0)
                    {
                        Entities e = new Entities
                        {
                            NameOfEntity = type,
                            Name1 = "Info",
                            Name2 = "Price",
                            Name3 = "Volume",
                            Name4 = "Weight"
                        };

                        db.Entities.Add(e);

                        db.SaveChanges();
                    }

                    Characteristic c = new Characteristic
                    {
                        Value1 = info,
                        Value2 = price,
                        Value3 = volume,
                        Value4 = weight
                    };

                    db.Characteristic.Add(c);

                    db.SaveChanges();

                    Devices d = new Devices
                    {
                        IdEntity = db.Entities.First(x => x.NameOfEntity == type).Id,
                        IdCharacteristic = db.Characteristic.First(x => x.Value1 == info).IdCharacteristic,
                        BrandName = brend,
                        Model = code
                    };

                    db.Devices.Add(d);

                    db.SaveChanges();

                    SetTypesForDevice(brend, code, tmp, 1);
                }
                else if (group2.Equals("Приводы"))
                {
                    string type = "Привод";
                    bool isUsb = false;
                    bool isUsb3 = false;
                    bool isSata = false;

                    if (Array.IndexOf(m, "USB") != -1)
                    {
                        isUsb = true;
                    }
                    else if (Array.IndexOf(m, "USB3.0") != -1)
                    {
                        isUsb3 = true;
                    }
                    else if (Array.IndexOf(m, "SATA") != -1)
                    {
                        isSata = true;
                    }
                    //
                    string tmp = isUsb ? "USB" : (isUsb3 ? "USB3.0" : "SATA");
                    CheckType(tmp);

                    if (db.Entities.Count(x => x.NameOfEntity == type) == 0)
                    {
                        Entities e = new Entities
                        {
                            NameOfEntity = type,
                            Name1 = "Info",
                            Name2 = "Price",
                            Name3 = "Volume",
                            Name4 = "Weight"
                        };

                        db.Entities.Add(e);

                        db.SaveChanges();
                    }

                    Characteristic c = new Characteristic
                    {
                        Value1 = info,
                        Value2 = price,
                        Value3 = volume,
                        Value4 = weight
                    };

                    db.Characteristic.Add(c);

                    db.SaveChanges();

                    Devices d = new Devices
                    {
                        IdEntity = db.Entities.First(x => x.NameOfEntity == type).Id,
                        IdCharacteristic = db.Characteristic.First(x => x.Value1 == info).IdCharacteristic,
                        BrandName = brend,
                        Model = code
                    };

                    db.Devices.Add(d);

                    db.SaveChanges();

                    SetTypesForDevice(brend, code, tmp, 1);
                }
                else if (group2.Equals("Процессоры"))
                {
                    string type = "Процессор";
                    
                    bool isAmd = false;
                    bool isIntel = false;

                    if (Array.IndexOf(m, "AMD") != -1)
                    {
                        isAmd = true;
                    }
                    else if (Array.IndexOf(m, "Intel") != -1)
                    {
                        isIntel = true;
                    }

                    //
                    CheckType(group3);

                    if (db.Entities.Count(x => x.NameOfEntity == type) == 0)
                    {
                        Entities e = new Entities
                        {
                            NameOfEntity = type,
                            Name1 = "Info",
                            Name2 = "Price",
                            Name3 = "Volume",
                            Name4 = "Weight",
                            Name5 = "Producer"
                        };

                        db.Entities.Add(e);

                        db.SaveChanges();
                    }

                    Characteristic c = new Characteristic
                    {
                        Value1 = info,
                        Value2 = price,
                        Value3 = volume,
                        Value4 = weight,
                        Value5 = isAmd ? "AMD" : "Intel"
                    };

                    db.Characteristic.Add(c);

                    db.SaveChanges();

                    Devices d = new Devices
                    {
                        IdEntity = db.Entities.First(x => x.NameOfEntity == type).Id,
                        IdCharacteristic = db.Characteristic.First(x => x.Value1 == info).IdCharacteristic,
                        BrandName = brend,
                        Model = code
                    };

                    db.Devices.Add(d);

                    db.SaveChanges();

                    SetTypesForDevice(brend, code, group3, 1);
                }
            }
        }

        private void CheckType(string type)
        {
            if (db.Types.Count(x => x.Name == type) == 0)
            {
                Types t = new Types { Name = type };

                db.Types.Add(t);

                db.SaveChanges();
            }
        } 

        private void SetTypesForDevice(string brend, string code, string type, int count)
        {
            DeviceToType dt = new DeviceToType
            {
                IdDevice = db.Devices.First(x => x.BrandName == brend && x.Model == code).IdDevice,
                IdType = db.Types.First(x => x.Name == type).IdType,
                Count = count
            };

            db.DeviceToType.Add(dt);

            //try
            {
                db.SaveChanges();
            }
            //catch(Exception e)
            {
               // Console.WriteLine(e.Message);
            }
        }

        private void CheckSocket(string socket)
        {
            if (db.Sockets.Count(x => x.Name == socket) == 0)
            {
                Sockets s = new Sockets { Name = socket };

                db.Sockets.Add(s);

                db.SaveChanges();
            }
        }

        private void SetSocketsForDevice(string brend, string code, string socket, int count)
        {
            DeviceToSocket ds = new DeviceToSocket
            {
                IdDevice = db.Devices.First(x => x.BrandName == brend && x.Model == code).IdDevice,
                IdSocket = db.Sockets.First(x => x.Name == socket).IdSocket,
                Count = count
            };

            db.DeviceToSocket.Add(ds);

            db.SaveChanges();
        }
    }
}
