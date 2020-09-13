using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using infoOnTable.Models;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
//using ImageDbApp.Models;

namespace infoOnTable.Controllers
{
    public class HomeController : Controller
    {
        private SpecialForTabletEntities db = new SpecialForTabletEntities();

        public ActionResult Index()
        {

            return View(); //стартовая стр авторизации
        }


        [HttpPost]
        public ActionResult Index(string Error)
        {

            return View(); //возврат на дом. стр. если ошибка данных
        }
        public ActionResult NewRoom()
        {

            return View();

        }

        [HttpPost]
        public async Task<ActionResult> AddRoom(string numberRoom)
        {

            if ((numberRoom != null)) //проверка, что введены все данные
            {
                //проверим числа ли ввели
                string str = numberRoom.Trim();
                int numRoom;
                bool isNum = int.TryParse(str, out numRoom);

                // действие если строка - число
                if (isNum)
                {
                    // проверим, есть ли уже такой кабинет в бд                  
                    var room = db.Кабинет.ToList();
                    foreach (var el in room)
                    {
                        if (el.Номер_кабинета == numRoom) return View("Exist");

                    }
                    var passw = numRoom.ToString() + "room";
                    Кабинет кабинет = new Кабинет();
                    кабинет.Номер_кабинета = numRoom;

                    кабинет.Пароль = passw;

                    db.Кабинет.Add(кабинет);
                    await db.SaveChangesAsync();


                    ViewBag.Login = numRoom;
                    ViewBag.Password = passw;
                    ViewBag.idRoom = кабинет.Id_cab;

                    return View();

                }

                else
                {
                    // действие если строка - не число
                    ViewBag.InfoError = "Номер кабинета должен содержать только цифры!";
                    return View("Error");

                }
            }
            else
            {
                ViewBag.InfoError = "Введены не все данные!";
                return View("Error");
            }


        }

        [HttpGet]
        public ActionResult InputRoom(int? roomNumber, int? idRoom)
        {
            if (roomNumber != null)
            {


                //список врачей из кабинета
                int selectedIndex = 0;


                var listDoctors1 = db.Врач.Where(d => d.id_cab == idRoom).Select(d => new
                {
                    Text = d.Фамилия + " " + d.Имя + " " + d.Отчество + " " + d.Должность,
                    Value = d.Id_doc });

                // ViewBag.listDoctors = new SelectList(listDoctors, "Value", "Text");

                SelectList listDoctors = new SelectList(listDoctors1, "Value", "Text", selectedIndex);
                /*    SelectList listDoctors = new SelectList(db.Врач.Where(d => d.id_cab == idRoom), "Id_doc", "Фамилия"+" "+"Имя"+" "+"Отчество", selectedIndex);*/
                ViewBag.listDoctors = listDoctors;
                ViewBag.room = roomNumber;
                ViewBag.Idroom = idRoom;
                return View();


            }
            return View();
        }


        [HttpPost]
        public ActionResult InputRoom(string Login, string Password)
        {
            if (Login != "" && Password != "" && Login != null && Password != null)
            {
                string str = Login.Trim();
                int numRoom;
                bool isNum = int.TryParse(str, out numRoom);
                Password = Password.Trim();
                // действие если строка - число
                if (isNum)
                {
                    //смотрим , есть ли в бд кабинет
                    var room = db.Кабинет.Where(r => r.Номер_кабинета == numRoom && r.Пароль == Password);
                    if (room.Count() > 0)
                    {
                        //список врачей из кабинета
                        int selectedIndex = 0;

                        //     SelectList listDoctors = new SelectList(db.Врач.Where(d => d.id_cab == room.FirstOrDefault().Id_cab), "Id_doc", "Фамилия", selectedIndex);

                        var listDoctors2 = db.Врач.Where(d => d.id_cab == room.FirstOrDefault().Id_cab).Select(d => new
                        {
                            Text = d.Фамилия + " " + d.Имя + " " + d.Отчество + " " + d.Должность,
                            Value = d.Id_doc
                        });

                        SelectList listDoctors = new SelectList(listDoctors2, "Value", "Text", selectedIndex);
                        //     ViewBag.listDoctors = new SelectList(listDoctors, "Value", "Text");
                        ViewBag.listDoctors = listDoctors;
                        ViewBag.room = room.FirstOrDefault().Номер_кабинета;
                        ViewBag.Idroom = room.FirstOrDefault().Id_cab;

                        return View();
                    }
                    else
                    {
                        ViewBag.InfoError = "Такого кабинета в базе нет либо введен неверный пароль!";
                        return View("Error");
                    }

                }
                else
                {
                    ViewBag.InfoError = "Номер кабинета должен содержать только цифры!";
                    return View("Error");
                }

            }
            else
            {
                ViewBag.InfoError = "Введены не все данные!";
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult AddUser(int? roomNumber, int? idRoom)
        {

            ViewBag.room = roomNumber.ToString();
            ViewBag.IdRoom = idRoom.ToString();

            return View();
        }

        [HttpPost]
         public ActionResult AddUser(string LastName, string FirstName, string Otchectvo, string Position, string Grade, HttpPostedFileBase Foto, int? Room, int? idRoom)
        {

            Врач doctors = new Врач();
            doctors.Фамилия = LastName.TrimEnd();
            doctors.Имя = FirstName.TrimEnd();
            doctors.Отчество = Otchectvo.TrimEnd();
            doctors.Должность = Position.TrimEnd();
            doctors.Квалификация = Grade.TrimEnd();
            doctors.Готовность = true;
           

            Кабинет room = db.Кабинет.Find(idRoom);

            if (Foto != null)
            {
                byte[] imageData = null;

                using (var binaryReader = new BinaryReader(Foto.InputStream))
                {
                    imageData = binaryReader.ReadBytes(Foto.ContentLength);
                }
                doctors.Фото = imageData;
            }
            db.Врач.Add(doctors);
            room.Врач.Add(doctors);

            db.SaveChanges();

            ViewBag.IdRoom = idRoom;
            ViewBag.IdDoctor = doctors.Id_doc;

            return View("SelectMode");


        }

        [HttpPost]
        async public Task<ActionResult> SelectModeAppForTablet(string Mode, int idDoctor, int? idRoom)
        {
            var infoOnTablet = await db.Врач.FindAsync(idDoctor);
            ViewBag.idDoctor = idDoctor;
            ViewBag.idRoom = (int)idRoom;

            if (Mode == "doctor")
            {
                ViewBag.Mode = "doctor";
                
            }
            else if (Mode == "user")
            {
                
                var room = await db.Кабинет.FindAsync((int)idRoom);
                ViewBag.Room = room.Номер_кабинета;
                ViewBag.Mode = "user";


            }

           
            return View(infoOnTablet);
        }

        [HttpPost]
        public ActionResult SelectUser(int? listDoctors, int? idRoom)
        {
            if (listDoctors != null)
            {
                ViewBag.IdDoctor = listDoctors;
                ViewBag.IdRoom = idRoom;
                return View("SelectMode");
            }
            else
            {
                ViewBag.InfoError = "Вы не выбрали пользователя!";
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<ActionResult> infoOnTablet(int idRoom, int idDoctor)
        {  

            var doctor = await db.Врач.FindAsync(idDoctor);
            var room = await db.Кабинет.FindAsync(idRoom);
            ViewBag.Room = room.Номер_кабинета;


            return PartialView(doctor);
        }

        [HttpPost]
        public async Task<ActionResult> infoForPatien(string info, string name, string number, int idRoom, int idDoctor)
        {  //добавить хранение фото врача

            var doctor = await db.Врач.FindAsync(idDoctor);
            bool ex = false;
            if (info.Contains("false"))
            {
                doctor.Готовность = false;
               
            }
            else if (info.Contains("true"))
            {
                doctor.Готовность = true;
            }  
                Пациент patient;
                string str = "";

                if (db.Пациент.Where(p=>p.id_doc==idDoctor).FirstOrDefault() != null)
                {
                    patient = db.Пациент.Where(p => p.id_doc == idDoctor).FirstOrDefault();
                    ex = true;
                }
                else
                {
                    int id = 0;
                    patient = new Пациент();
                    while (await db.Пациент.FindAsync(id)!=null)
                    {
                        id++;
                    }
                                       
                    patient.Id_patient = id;
                }

                if (name != null && name != "") patient.Фамилия = name;
                else patient.Фамилия = "";             

                if (number != null && number != "") patient.Номер_талона = Convert.ToInt32(number);
                else patient.Номер_талона = null;

                str += patient.Фамилия + " " + patient.Номер_талона;

                if (!ex)
                {
                    db.Пациент.Add(patient);
                    doctor.Пациент.Add(patient);
                }
                else
                {
                    db.Entry(patient).State = EntityState.Modified;
                }

            if (info.Contains("false"))
            {
                ViewBag.infoForDoctor = "Установлен статус - \"Идет прием\". Отметьте поле сверху, когда станете готовы принять следующего пациента и нажмите кнопку";
            }
            else if (info.Contains("true"))
            {
                ViewBag.infoForDoctor = "Установлен статус - \"Входите\". Не забудьте снять отметку, после того, как пациент зайдет в кабинет (и нажать кнопку) - это покажет другим пациентам, что идет прием. Информация о " + str + " " + "появилась на планшете";
            }
            

            db.Entry(doctor).State = EntityState.Modified;

            await db.SaveChangesAsync();

            return PartialView();
        }
    }
}