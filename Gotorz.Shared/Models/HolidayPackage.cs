using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gotorz.Shared.Models
{
    public class HolidayPackage
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        //public DateOnly DateFrom { get; set; }
        //public DateOnly DateTo { get; set; }
        // public List<FlightTicket> FlightTickets { get; set; }
        // public Hotel Hotel { get; set; }
        public Decimal CostPrice { get; set; }
        public Decimal MarkupPercentage {  get; set; }

        /*
        public Decimal CalculateCostPrice()
        {
            if (this.Hotel != null && this.FlightTickets != null) {
               
                Decimal Total = this.Hotel.GetPrice();
                
                for (this.FlightTicket : this.FlightTickets)
                {
                    Total += this.FlightTicket.GetPrice();
                }
                
                this.CostPrice = Total;

                return Total;
            }
            else
            {
                throw new Exception("Hotel or FlightTickets are null");
            }
        }

        public Decimal CalculateTotalPrice()
        {
            if (this.CostPrice == null)
            {
                this.CalculateCostPrice();
            }
            Decimal Total = this.CostPrice * (1 + this.Markup);

            this.TotalPrice = Total;

            return Total;
        }
        */
    }
}
