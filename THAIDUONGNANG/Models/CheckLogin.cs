using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOTO.Models
{
    public class CheckLogin
    {
       public static bool CheckFloat(string Check)
        {
            if (Check != "" || Check != null)
            {
                return true;
            }
            else
                return false;
        }
    }
 
}