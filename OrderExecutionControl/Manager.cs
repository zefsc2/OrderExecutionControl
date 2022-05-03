using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OrderExecutionControl
{
    class Manager
    {
        public static Frame MainFrame { get; set; }
        public static Frame SFrame { get; set; }
        public static int UserCode { get; set; }
        public static TextBlock UserName { get; set; }
        public static TextBlock NameCurrentPage { get; set; }
        public static Button ButtonExit { get; set; }
        public static Button Button_Employee { get; set; }
        public static Button Button_Issued_Commission { get; set; }
        public static Button Button_Received_Commission { get; set; }
        public static int currentCommission { get; set; }
    }
}
