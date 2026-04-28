using Models;
using System;
using System.Collections.Generic;

namespace MVC.ViewModels
{
    public class ReturnIndexViewModel
    {
        public List<ReturnItemDto> ReturnedItems { get; set; } = new();
        public List<ReturnItemDto> ReclaimedItems { get; set; } = new();
    }

}