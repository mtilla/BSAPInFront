using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace BS.API.Models
{
    public class DocumentDBStyle
    {
        //document.json mappas mot denna class i DocumentRepository Retrieve()!!!!
        //OBS! om du ist ska använda den namngivning som är lika som DB kolumnnamn så måste contractresolver i webconfig kommenteras bort
        //och i Fronts documentListView <td> ändras till samma som här då
        //public String ID { get; set; } = Guid.NewGuid().ToString();  Behövs denna? borde ha den..
        public string PartID { get; set; }
        public string CUSTOMER_NAME { get; set; }    
        public int CUSTOMER_NR { get; set; }
        public int FAKTURA_NR { get; set; }
        public DateTime DUE_DATE { get; set; }
        //public Image DocumentData { get; set; } //Kanske måste byta till typ string?
        public DateTime? CreateDate { get; set; } //Testegenskap

        public Dictionary<String, String> Rows { get; set; } = new Dictionary<string, string>();
    }
}