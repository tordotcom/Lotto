﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lotto.Models
{
    public class User_Rate_Discount
    {
        public string ID { get; set; }
        public string rateID { get; set; }
        public string discountID { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string ThreeUP { get; set; }
        public string ThreeDown { get; set; }
        public string ThreeOod { get; set; }
        public string FirstThree { get; set; }
        public string FirstThreeOod { get; set; }
        public string TwoUp { get; set; }
        public string TwoOod { get; set; }
        public string TwoDown { get; set; }
        public string Up { get; set; }
        public string Down { get; set; }
        public string ThreeUP_discount { get; set; }
        public string ThreeDown_discount { get; set; }
        public string ThreeOod_discount { get; set; }
        public string FirstThree_discount { get; set; }
        public string FirstThreeOod_discount { get; set; }
        public string TwoUp_discount { get; set; }
        public string TwoOod_discount { get; set; }
        public string TwoDown_discount { get; set; }
        public string Up_discount { get; set; }
        public string Down_discount { get; set; }
    }
}