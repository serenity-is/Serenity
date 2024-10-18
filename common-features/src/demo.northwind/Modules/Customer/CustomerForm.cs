namespace Serenity.Demo.Northwind.Forms;

[FormScript("Northwind.Customer")]
[BasedOnRow(typeof(CustomerRow), CheckNames = true)]
public class CustomerForm
{
    [Category("General")]
    public string CustomerID { get; set; }
    public string CompanyName { get; set; }
    [Category("Contact")]
    public string ContactName { get; set; }
    public string ContactTitle { get; set; }
    public List<int> Representatives { get; set; }

    [Category("Address")]
    public string Address { get; set; }
    [HalfWidth]
    public string Country { get; set; }
    [HalfWidth]
    public string City { get; set; }
    [HalfWidth]
    public string Region { get; set; }
    [HalfWidth]
    public string PostalCode { get; set; }
    [HalfWidth]
    public string Phone { get; set; }
    [HalfWidth]
    public string Fax { get; set; }
    public List<object> NoteList { get; set; }

    // note: these fields are stored in an extension table (CustomerDetails)
    [Category("Details")]
    [HalfWidth]
    public DateTime? LastContactDate { get; set; }
    [HalfWidth]
    public int? LastContactedBy { get; set; }
    [HalfWidth]
    public string Email { get; set; }
    [HalfWidth]
    public bool? SendBulletin { get; set; }
}