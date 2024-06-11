using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;

namespace Systems.IHS.KeyboardSender.Controllers
{
    // https://learn.microsoft.com/en-us/dotnet/desktop/winforms/input-keyboard/how-to-simulate-events?view=netdesktop-8.0
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class KeyboardController : ControllerBase
    {
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);



        private readonly ILogger<KeyboardController> _logger;

        public KeyboardController(ILogger<KeyboardController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public ActionResult<object> SendKeys(string key)
        {
            IntPtr calcWindow = FindWindow(null, Program.AppName);

            if (SetForegroundWindow(calcWindow))
            {
                if (Program.Data.ContainsKey(key)) 
                {
                    System.Windows.Forms.SendKeys.SendWait(Program.Data[key]);
                    return Ok(Program.Data[key]);
                }
                else
                {
                    return NotFound($"Key : {key} don't exsist!");
                }
            } else {
                return NotFound($"Window name {Program.AppName} don't exsist!");
            }

        }

        [HttpGet]
        public ActionResult<object> GetAppName()
        {
            return Ok(Program.AppName);
        }

        [HttpPost]
        public ActionResult<object> SetAppName(string text)
        {
            Program.AppName = text;
            return Ok(text);
        }

        [HttpGet]
        public ActionResult<object> GetData()
        {
            return Ok(Program.Data);
        }

        [HttpPost]
        public ActionResult<object> AddOrEditKey(string key, string value)
        {
            if (Program.Data.ContainsKey(key))
            {
                Program.Data[key] = value;
            } else {
                Program.Data.Add(key, value);
            }
            return Ok("OK");
        }

        [HttpDelete]
        public ActionResult<object> DeleteKey(string key)
        {
            if (Program.Data.ContainsKey(key))
            {
                Program.Data.Remove(key);
                return Ok("OK");
            }
            else
            {
                return NotFound("Key don't exsist");
            }
        }

    }
}
