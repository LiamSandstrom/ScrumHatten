using System;
using System.Collections.Generic;
using System.Text;
using Models;

namespace Models
{
    /// <summary>
    /// Denna modell används för att hantera omsättningsinformation på statistiksidan.
    /// </summary>
    public class SalesMonth
    {
        public string MonthName { get; set; }

        public int AmountOfOrders { get; set; }

        public decimal Totalsales { get; set; }
    }
}
