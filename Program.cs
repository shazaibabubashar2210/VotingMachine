using System;
namespace QueenLocalDataHandling
{
    class Program
    {
        static void Main(string[] args)
        {
            Order o = new Order();
            OrderCRUD oc = new OrderCRUD();
            while(true)
            {
                Console.WriteLine("-------------------------Welcome to Online Fashion Shop--------------------------------");
                Console.WriteLine("1. Insert Order");
                Console.WriteLine("2. View Order");
                Console.WriteLine("3. Update Address");
                Console.WriteLine("4. Remove Address");
                Console.WriteLine("5. Use for Sql Injection Prevention Update Address ");
                Console.Write("Enter Your choice: ");
                int choice=Convert.ToInt32(Console.ReadLine());
                if(choice==1)
                {
                    oc.InsertOrder(o);
                }
                else if(choice==2)
                {
                    oc.GetAllOrders();
                }
                else if(choice==3)
                {
                    oc.UpdateAddress();
                }
                else if(choice==4)
                {
                    oc.DeleteOrder();
                }
                else if(choice==5)
                {
                    Console.Write("Enter Phone Number: ");
                    string phone = Console.ReadLine();
                    Console.Write("Enter new Address To Update: ");
                    string address = Console.ReadLine();
                    oc.UpdateOrderAddress(phone,address);
                }
            }
        }
    }
}
