using System;
using Microsoft.Data.SqlClient;
namespace QueenLocalDataHandling
{
    class OrderCRUD
    {
        private const string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=QueenDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
     public void InsertOrder(Order o)
{
    Console.Write("Enter Order ID: ");
    string O_ID = Console.ReadLine();

    // Check if Order ID already exists in the database
    if (OrderExists(O_ID))
    {
        Console.WriteLine("-----------------------------------------------------");
        Console.WriteLine($"Order with ID '{O_ID}' already exists. Cannot insert duplicate order.");
        Console.WriteLine("-----------------------------------------------------");
        return; 
    }

    Console.Write("Enter Customer CNIC: ");
    string Customer_Cnic = Console.ReadLine();

    Console.Write("Enter Customer Name: ");
    string Name = Console.ReadLine();

    Console.Write("Enter Customer Phone: ");
    string Phone = Console.ReadLine();

    Console.Write("Enter Customer Address: ");
    string Address = Console.ReadLine();

    Console.Write("Enter Product ID: ");
    string ProductId = Console.ReadLine();

    Console.Write("Enter Product Price: ");
    string Price = Console.ReadLine();

    Console.Write("Enter Size of Product: ");
    string SizeOfProdcut = Console.ReadLine();

    o.OrderID = O_ID;
    o.GetCNIC = Customer_Cnic;
    o.CustomerName = Name;
    o.GetPhone = Phone;
    o.GetAddress = Address;
    o.GetProductId = ProductId;
    o.GetPrice = Price;
    o.GetSizeOfProduct = SizeOfProdcut;

    SqlConnection conn = new SqlConnection(connectionString);
    conn.Open();
    if (OrderExists(O_ID))
    {
        Console.WriteLine("-----------------------------------------------------");
        Console.WriteLine($"Order with ID '{O_ID}' already exists. Cannot insert duplicate order.");
        Console.WriteLine("-----------------------------------------------------");
        conn.Close();
        return;
    }

    string query = $"INSERT INTO Orders (OrderId, CNIC, CustomerName, CustomerPhone, CustomerAddress, ProductId, Price, SizeofProduct) " +
                   $"VALUES('{o.OrderID}', '{o.GetCNIC}', '{o.CustomerName}', '{o.GetPhone}', '{o.GetAddress}', '{o.GetProductId}', '{o.GetPrice}', '{o.GetSizeOfProduct}')";

    SqlCommand cmd = new SqlCommand(query, conn);
    int rowsAffected = cmd.ExecuteNonQuery();

    if (rowsAffected > 0)
    {
        Console.WriteLine("-----------------------------------------------------");
        Console.WriteLine("Order Inserted Successfully!");
        Console.WriteLine("-----------------------------------------------------");
    }
    else
    {
        Console.WriteLine("-----------------------------------------------------");
        Console.WriteLine("Error! Unable to insert the order.");
        Console.WriteLine("-----------------------------------------------------");
    }

    conn.Close();
}

private bool OrderExists(string orderId)
{
    SqlConnection conn = new SqlConnection(connectionString);
    conn.Open();

    string query = $"SELECT * FROM Orders WHERE OrderId = '{orderId}'";
    SqlCommand cmd = new SqlCommand(query, conn);
    SqlDataReader dr = cmd.ExecuteReader();

    bool orderExists = dr.HasRows;

    conn.Close();

    return orderExists;
}


        public void GetAllOrders()
        {
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            string query = $"Select * from Orders";
            SqlCommand cmd = new SqlCommand(query,conn);
            SqlDataReader dr = cmd.ExecuteReader();
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("Customer Details: ");
            Console.WriteLine("-------------------------------------");
            while(dr.Read())
            {
                Console.WriteLine("--------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine($"OrderId: {dr[0]} CNIC: {dr[1]} CustomerName: {dr[2]} CustomerPhoneNumber: {dr[3]} CustomerAddress: {dr[4]} ProductId: {dr[5]} Price: {dr[6]} SizeOfProduct: {dr[7]}");
                Console.WriteLine("--------------------------------------------------------------------------------------------------------------------------------------------------------------------");
            }
            conn.Close();
        }

        // Update Address Method
        public void UpdateAddress()
        {
            Console.Write("Enter Phone Number to Update Address: ");
            string phoneNumber = Console.ReadLine();
            Console.Write("Enter new Address: ");
            string address = Console.ReadLine();
            SqlConnection conn = new SqlConnection(connectionString);

            conn.Open();

            string query = $"Update Orders set CustomerAddress='{address}' where CustomerPhone='{phoneNumber}'";
            SqlCommand cmd = new SqlCommand(query,conn);

            int check = cmd.ExecuteNonQuery();
            if(check>0)
            {
                Console.WriteLine("---------------------------------");
                Console.WriteLine("Updated! Successfully");
                Console.WriteLine("---------------------------------");
            }
            else
            {
                Console.WriteLine("---------------------------------");
                Console.WriteLine("Error! Wrong Phone Number");
                Console.WriteLine("---------------------------------");
            }
            conn.Close();
        }


        // Delete Order

        public void DeleteOrder()
        {
            Console.Write("Enter Order ID To Remove an Order: ");
            string orderId = Console.ReadLine();

            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            string query = $"Delete from Orders where OrderId='{orderId}'";
            SqlCommand cmd = new SqlCommand(query,conn);
            int deleted = cmd.ExecuteNonQuery();
            if(deleted>0)
            {

                Console.WriteLine("---------------------------------");
                Console.WriteLine("Order Remove Succesfully!");
                Console.WriteLine("---------------------------------");
            }
            else
            {
                Console.WriteLine("---------------------------------");
                Console.WriteLine("Error! Removing Order");
                Console.WriteLine("---------------------------------");
            }
        }
       public void UpdateOrderAddress(string phoneNumber,string updateCustomerAddress)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            string query = $"UPDATE Orders SET CustomerAddress = @UpdateCustomerAddress WHERE CustomerPhone = @PhoneNumber";
            SqlParameter p1 = new SqlParameter("UpdateCustomerAddress",updateCustomerAddress);
            SqlParameter p2 = new SqlParameter("PhoneNumber",phoneNumber);
            SqlCommand cmd = new SqlCommand(query,conn);
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            int rowEffected = cmd.ExecuteNonQuery();
            if(rowEffected>0)
            {
                Console.WriteLine("-------------------------------");
                Console.WriteLine("Updated Successfully!");
                Console.WriteLine("-------------------------------");
            }
            else
            {
                Console.WriteLine("------------------------------");
                Console.WriteLine("Error! Updating");
                Console.WriteLine("------------------------------");
            }
                  
        }
    }
}
