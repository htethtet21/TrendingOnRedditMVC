using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DataMining_MVC.Controllers
{
    public class Error : Controller
    {
        
        public IActionResult Index(String errorMessage)
        {
            ViewData["ErrorMessage"] = errorMessage;
          
            return View();
        }
    }
}

