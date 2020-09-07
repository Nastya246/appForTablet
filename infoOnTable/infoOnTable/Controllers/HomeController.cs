using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using infoOnTable.Models;
using System.Threading.Tasks;

namespace infoOnTable.Controllers
{
    public class HomeController : Controller
    {
        private SpecialForTabletEntities db = new  SpecialForTabletEntities();

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
                    var room =  db.Кабинет.ToList();
                    foreach (var el in room)
                    {
                        if (el.Номер_кабинета == numRoom) return View ("Exist");
                        
                    }
                    var passw= numRoom.ToString() + "room";
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

                    return View("Error");
                 
                }                                       
            }
            else
            {
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
                        Text = d.Фамилия + " " + d.Имя+" "+d.Отчество+" "+d.Должность,
                        Value = d.Id_doc});

                   // ViewBag.listDoctors = new SelectList(listDoctors, "Value", "Text");

                  SelectList listDoctors=new SelectList(listDoctors1, "Value", "Text", selectedIndex);
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
                        return View("Error");
                    }
                  
                }
                else
                {
                    return View("Error");
                }             
               
            }
            else
            {
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult AddUser(int? roomNumber , int? idRoom )
        {

            ViewBag.room = roomNumber.ToString();
            ViewBag.IdRoom = idRoom.ToString();

            return View();
        }

        [HttpPost]
        async public Task<ActionResult> AddUser(string LastName, string FirstName, string Otchectvo, string Position, string Grade, HttpPostedFileBase Foto, int? Room, int? idRoom)
        {
         
            Врач doctors = new Врач();
            doctors.Фамилия = LastName.TrimEnd();
            doctors.Имя = FirstName.TrimEnd();
            doctors.Отчество = Otchectvo.TrimEnd();
            doctors.Должность = Position.TrimEnd();
            doctors.Квалификация = Grade.TrimEnd();
            doctors.Готовность = false;
            db.Врач.Add(doctors);

            Кабинет room = await db.Кабинет.FindAsync(idRoom);

            if (Foto!= null)
            {
                // получаем имя файла
              //  string fileName = System.IO.Path.GetFileName(upload.FileName);
                // сохраняем файл в папку Files в проекте
           //     upload.SaveAs(Server.MapPath("~/Files/" + fileName));
            }

            room.Врач.Add(doctors);
                         
            await db.SaveChangesAsync();

            ViewBag.IdRoom = idRoom;

            return View("SelectMode");
            
          
        }

        [HttpPost]
        async public Task<ActionResult> SelectModeAppForTablet(string Mode, int idDoctor, int? idRoom)
        {

            if (Mode == "doctor")
            {
                ViewBag.Mode = "doctor";
            }
            else if (Mode == "user")
            {
                ViewBag.Mode = "user";
            }

            var infoOnTablet = await db.Врач.FindAsync(idDoctor);
            return View(infoOnTablet);
        }

        [HttpPost]
        async public Task<ActionResult> SelectUser(int listDoctors, int? idRoom)
        {
            ViewBag.IdDoctor = listDoctors;
            ViewBag.IdRoom = idRoom;
            return View("SelectMode");
        }
    }
}